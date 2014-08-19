#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2014 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

#if !NO_CONCURRENT_COLLECTIONS

﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclex.Support {

  /// <summary>Processes tasks in parallel using many threads</summary>
  /// <typeparam name="TTask">Type of tasks the class will process</typeparam>
  public abstract class ParallelBackgroundWorker<TTask> : IDisposable {

    /// <summary>Number of CPU cores available on the system</summary>
    public static readonly int ProcessorCount = Environment.ProcessorCount;

    /// <summary>
    ///   Timeout after which Dispose() will stop waiting for unfinished tasks and
    ///   free resources anyway
    /// </summary>
    private static readonly int DefaultDisposeTimeoutMilliseconds = 1500; // milliseconds

    /// <summary>Initializes a new parallel background worker with unlimited threads</summary>
    public ParallelBackgroundWorker() : this(int.MaxValue) { }

    /// <summary>
    ///   Initializes a new parallel background worker running the specified number
    ///   of tasks in parallel
    /// </summary>
    /// <param name="threadCount">
    ///   Number of tasks to run in parallel (if positive) or number of CPU cores to leave
    ///   unused (if negative).
    /// </param>
    /// <remarks>
    ///   If a negative number of threads is used, at least one thread will be always
    ///   be created, so specifying -2 on a single-core system will still occupy
    ///   the only core.
    /// </remarks>
    public ParallelBackgroundWorker(int threadCount) {
      if(threadCount > 0) {
        this.threadCount = threadCount;
      } else {
        threadCount = Math.Max(1, ProcessorCount + threadCount);
      }

      this.queueSynchronizationRoot = new object();
      this.runQueuedTasksInThreadDelegate = new Action(runQueuedTasksInThread);
      this.tasks = new Queue<TTask>();
      this.threadTerminatedEvent = new AutoResetEvent(false);
      this.cancellationTokenSource = new CancellationTokenSource();
      this.exceptions = new ConcurrentBag<Exception>();
    }

    /// <summary>
    ///   Initializes a new parallel background worker that uses the specified name for
    ///   its worker threads.
    /// </summary>
    /// <param name="name">Name that will be assigned to the worker threads</param>
    public ParallelBackgroundWorker(string name) :
      this(int.MaxValue) {
      threadName = name;
    }

    /// <summary>
    ///   Initializes a new parallel background worker that uses the specified name for
    ///   its worker threads and running the specified number of tasks in parallel.
    /// </summary>
    /// <param name="name">Name that will be assigned to the worker threads</param>
    /// <param name="threadCount">
    ///   Number of tasks to run in parallel (if positive) or number of CPU cores to leave
    ///   unused (if negative).
    /// </param>
    /// <remarks>
    ///   If a negative number of threads is used, at least one thread will be always
    ///   be created, so specifying -2 on a single-core system will still occupy
    ///   the only core.
    /// </remarks>
    public ParallelBackgroundWorker(string name, int threadCount) :
      this(threadCount) {
      threadName = name;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.threadTerminatedEvent != null) {
        CancelPendingTasks();
        CancelRunningTasks();

        Wait(DefaultDisposeTimeoutMilliseconds);

        this.threadTerminatedEvent.Dispose();
        this.threadTerminatedEvent = null;
      }
      lock(this.queueSynchronizationRoot) {
        if(this.cancellationTokenSource != null) {
          this.cancellationTokenSource.Dispose();
          this.cancellationTokenSource = null;
        }
      }
    }

    /// <summary>Adds a task for processing by the background worker threads</summary>
    /// <param name="task">Task that will be processed in the background</param>
    public void AddTask(TTask task) {
      if(this.cancellationTokenSource.IsCancellationRequested) {
        return;
      }

      bool needNewThread;

      lock(this.queueSynchronizationRoot) {
        this.tasks.Enqueue(task);

        needNewThread = (this.runningThreadCount < this.threadCount);
        if(needNewThread) {
          ++this.runningThreadCount;
        }
      }

      if(needNewThread) {
        Task newThread = new Task(
          this.runQueuedTasksInThreadDelegate,
          // this.cancellationTokenSource.Token, // DO NOT PASS THIS!
          // Passing the cancellation token makes tasks that have been queued but
          // not started yet cease to execute at all - meaning our runningThreadCount
          // goes out of sync and Dispose() / Wait() / Join() sit around endlessly!
          TaskCreationOptions.LongRunning
        );
        newThread.Start();
      }
    }

    /// <summary>Cancels all tasks that are currently executing</summary>
    /// <remarks>
    ///   It is valid to call this method after Dispose()
    /// </remarks>
    public void CancelRunningTasks() {
      lock(this.queueSynchronizationRoot) {
        if(this.cancellationTokenSource != null) {
          this.cancellationTokenSource.Cancel();
        }
      }
    }

    /// <summary>Cancels all queued tasks waiting to be executed</summary>
    /// <remarks>
    ///   It is valid to call this method after Dispose()
    /// </remarks>
    public void CancelPendingTasks() {
      lock(this.queueSynchronizationRoot) {
        this.tasks.Clear();
      }
    }

    /// <summary>
    ///   Waits until all executing and queued tasks have been processed and throws an
    ///   exception if any errors have occurred
    /// </summary>
    public void Join() {
      while(Thread.VolatileRead(ref this.runningThreadCount) > 0) {
        this.threadTerminatedEvent.WaitOne();
      }

      if(this.exceptions.Count > 0) {
        throw new AggregateException(this.exceptions);
      }
    }

    /// <summary>
    ///   Waits until all executing and queued tasks have been processed or
    ///   the timeout is reached
    /// </summary>
    /// <param name="timeoutMilliseconds">Milliseconds after which the wait times out</param>
    /// <returns>
    ///   True if all tasks have been processed, false if the timeout was reached
    /// </returns>
    public bool Wait(int timeoutMilliseconds) {

      // Wait until the task queue has become empty
      while(queuedTaskCount > 0) {
        if(this.threadTerminatedEvent.WaitOne(timeoutMilliseconds) == false) {
          return false;
        }
      }

      // Now wait until all running tasks have finished
      while(Thread.VolatileRead(ref this.runningThreadCount) > 0) {
        if(this.threadTerminatedEvent.WaitOne(timeoutMilliseconds) == false) {
          return false;
        }
      }

      return true;

    }

    /// <summary>Called in a thread to execute a single task</summary>
    /// <param name="task">Task that should be executed</param>
    /// <param name="cancellationToken">
    ///   Cancellation token through which the method can be signalled to cancel
    /// </param>
    protected abstract void Run(TTask task, CancellationToken cancellationToken);

    /// <summary>
    ///   Runs queued tasks of the parallel background worker until the queue is empty
    /// </summary>
    private void runQueuedTasksInThread() {
      string previousThreadName = null;
      if(!string.IsNullOrEmpty(this.threadName)) {
        previousThreadName = Thread.CurrentThread.Name;
        Thread.CurrentThread.Name = this.threadName;
      }
      try {
        for(;;) {
          TTask task;
          lock(this.queueSynchronizationRoot) {
            if(this.tasks.Count == 0) {
              --this.runningThreadCount;
              break;
            }
            task = this.tasks.Dequeue();
          }

          try {
            Run(task, this.cancellationTokenSource.Token);
          }
          catch(Exception exception) {
            this.exceptions.Add(exception);
          }
        }

        this.threadTerminatedEvent.Set();
      }
      finally {
        if(!string.IsNullOrEmpty(this.threadName)) {
          Thread.CurrentThread.Name = previousThreadName;
        }
      }
    }

    /// <summary>Number of task still waiting to be executed</summary>
    private int queuedTaskCount {
      get {
        lock(this.queueSynchronizationRoot) {
          return this.tasks.Count;
        }
      }
    }

    /// <summary>
    ///   Name that will be assigned to the worker threads while they're processing tasks
    ///   for the parallel background worker
    /// </summary>
    private string threadName;
    /// <summary>Number of threads to use simultaneously</summary>
    private int threadCount;

    /// <summary>Synchronization root used to access the task queue and thread list</summary>
    /// <remarks>
    ///   <para>
    ///     Both lists are intentionally using a shared synchronization root because this will
    ///     guarantee that there's no race condition between adding work and a thread finishing:
    ///   </para>
    ///   <para>
    ///     Main thread
    ///     <code>
    ///       lock(this.queueSynchronizationRoot) {
    ///         this.tasks.Add(newTask);
    ///         
    ///         if(this.runningThreads.Count &lt; maximumThreadCount) {
    ///           startAdditionalTaskThread();
    ///         }
    ///       }
    ///     </code>
    ///   </para>
    ///   <para>
    ///     Task thread
    ///     <code>
    ///       TTask myTask;
    ///       lock(this.queueSynchronizationRoot) {
    ///         this.tasks.TryGet(out myTask);
    ///         
    ///         // RACE CONDITION WITHOUT LOCK: if this wasn't inside the same lock as the task
    ///         // checking, the main thread might check at this point, see that there are already
    ///         // enough threads running!
    ///         if(myTask == null) {
    ///           this.runningThreads.Remove(thisThread);
    ///           return;
    ///         }
    ///       }
    ///     </code>
    ///   </para>
    ///   <para>
    ///     The race condition mentioned above could completely stall the background worker by
    ///     leaving tasks in the queue but no threads active to execute them, so it is vital to
    ///     use this method of synchronization!
    ///   </para>
    /// </remarks>
    /// 
    private object queueSynchronizationRoot;

    /// <summary>Delegate for the runQueuedTasksInThread() method</summary>
    private Action runQueuedTasksInThreadDelegate;
    /// <summary>Tasks remaining to be processed</summary>
    private Queue<TTask> tasks;
    /// <summary>Threads that are currently executing tasks</summary>
    private int runningThreadCount;

    /// <summary>Exceptions that have occurred while executing tasks</summary>
    private ConcurrentBag<Exception> exceptions;
    /// <summary>Event that is triggered whenever a task gets completed</summary>
    private AutoResetEvent threadTerminatedEvent;

    /// <summary>
    ///   Provides a cancellation token that can be used to signal a thread to terminate
    /// </summary>
    private CancellationTokenSource cancellationTokenSource;

  }

} // namespace Nuclex.Support

#endif // !NO_CONCURRENT_COLLECTIONS
