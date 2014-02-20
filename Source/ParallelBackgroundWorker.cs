using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclex.Support {

  /// <summary>Processes tasks in parallel using many threads</summary>
  /// <typeparam name="TTask">Type of tasks the class will process</typeparam>
  public abstract class ParallelBackgroundWorker<TTask> : IDisposable {

    /// <summary>Number of CPU cores available on the system</summary>
#if XBOX360
    public static readonly int Processors = 4;
#else
    public static readonly int Processors = Environment.ProcessorCount;
#endif

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
        threadCount = Math.Max(1, Processors + threadCount);
      }

      this.runQueuedTasksInThreadDelegate = new Action<object>(runQueuedTasksInThread);
      this.runningThreads = new List<Task>();
      this.tasks = new Queue<TTask>();
      this.threadTerminatedEvent = new AutoResetEvent(false);
      this.cancellationTokenSource = new CancellationTokenSource();
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
        Join();

        this.threadTerminatedEvent.Dispose();
        this.threadTerminatedEvent = null;
      }
      if(this.cancellationTokenSource != null) {
        this.cancellationTokenSource.Dispose();
        this.cancellationTokenSource = null;
      }
    }


    /// <summary>Adds a task for processing by the background worker threads</summary>
    /// <param name="task">Task that will be processed in the background</param>
    public void AddTask(TTask task) {
      if(this.cancellationTokenSource.IsCancellationRequested) {
        return;
      }

      lock(this.queueSynchronizationRoot) {
        this.tasks.Enqueue(task);

        if(this.runningThreads.Count < this.threadCount) {
          //Task newThread = new Task(this.runQueuedTasksInThreadDelegate, );
        }
      }

      // Thread 1:
      //     lock() {
      //       - take task
      //       - or deregister and exit
      //     }
      //
      // Thread 2:
      //     lock() {
      //       - put task
      //       - if too few threads, register and add
      //     }

    }

    /// <summary>Cancels all tasks that are currently executing</summary>
    public void CancelRunningTasks() {
      this.cancellationTokenSource.Cancel();
    }

    /// <summary>Cancels all queued tasks waiting to be executed</summary>
    public void CancelPendingTasks() {
      lock(this.queueSynchronizationRoot) {
        this.tasks.Clear();
      }
    }

    /// <summary>Waits until all executing and queued tasks have been processed</summary>
    public void Join() {
      while(this.runningThreads.Count > 0) {
        this.threadTerminatedEvent.WaitOne();
      }
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
    /// <param name="thisTaskAsObject">Threading task in which this worker is running</param>
    private void runQueuedTasksInThread(object thisTaskAsObject) {
      string previousThreadName = null;
      if(!string.IsNullOrEmpty(this.threadName)) {
        previousThreadName = Thread.CurrentThread.Name;
        Thread.CurrentThread.Name = this.threadName;
      }
      try {
#if false
        TTask task;
        while(this.tasks.TryDequeue(out task)) {
          try {
            Run(task, this.cancellationTokenSource.Token);
          }
          catch(Exception exception) {
            this.exceptions.Add(exception);
          }
        }

        lock(this.runningThreads) {
          this.runningThreads.Remove((Task)thisTaskAsObject);
        }
        this.threadTerminatedEvent.Set();
#endif
      }
      finally {
        if(!string.IsNullOrEmpty(this.threadName)) {
          Thread.CurrentThread.Name = previousThreadName;
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
    private Action<object> runQueuedTasksInThreadDelegate;
    /// <summary>Tasks remaining to be processed</summary>
    private Queue<TTask> tasks;
    /// <summary>Threads that are currently executing tasks</summary>
    private IList<Task> runningThreads;

    // Idea:
    // private int runningThreadCount;
    // Before the task looks for new work, it will decrement this
    // if the task gets new work, it will increment this again.
    //   - if it would be above threadCount now, put work back in the queue
    // AddTask() increments this after pushing new work
    //   - if it would be above threadCount, do not create a new thread

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
