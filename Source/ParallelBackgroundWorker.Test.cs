#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2013 Nuclex Development Labs

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

using System;
using System.Threading;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the parallel background worker class</summary>
  [TestFixture]
  internal class ParallelBackgroundWorkerTest {

    #region class TestWorker

    /// <summary>Implementation of a background worker used for unit testing</summary>
    private class TestWorker : ParallelBackgroundWorker<object> {

      /// <summary>Initializes a new parallel background worker with unlimited threads</summary>
      public TestWorker() : base() { }

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
      public TestWorker(int threadCount) : base(threadCount) { }

      /// <summary>
      ///   Initializes a new parallel background worker that uses the specified name for
      ///   its worker threads.
      /// </summary>
      /// <param name="name">Name that will be assigned to the worker threads</param>
      public TestWorker(string name) : base(name) { }

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
      public TestWorker(string name, int threadCount) : base(name, threadCount) { }

      /// <summary>Called in a thread to execute a single task</summary>
      /// <param name="task">Task that should be executed</param>
      /// <param name="cancellationToken">
      ///   Cancellation token through which the method can be signalled to cancel
      /// </param>
      protected override void Run(object task, CancellationToken cancellationToken) {
        if(this.ThrowException) {
          throw new Exception("Something went wrong");
        }

        if(this.WaitEvent != null) {
          this.WaitEvent.WaitOne();
        }

        this.WasCancelled = cancellationToken.IsCancellationRequested;

        if(this.Tasks != null) {
          lock(this.Tasks) {
            this.Tasks.Add(task);
          }
        }
      }

      /// <summary>Whether the work tasks should throw exceptions</summary>
      public bool ThrowException;
      /// <summary>Event that can be used to stop work tasks from completing</summary>
      public ManualResetEvent WaitEvent;

      /// <summary>Set by work tasks if they have been cancelled</summary>
      public bool WasCancelled;
      /// <summary>Work tasks that have reached execution</summary>
      public ICollection<object> Tasks;

    }

    #endregion // class TestWorker

    /// <summary>Verifies that the background worker has a default constructor</summary>
    [Test]
    public void CanBeDefaultConstructed() {
      using(new TestWorker()) { }
    }

    /// <summary>
    ///   Verifies that a background worker can be constructed that uses a fixed number
    ///   of threads
    /// </summary>
    [Test]
    public void CanUseFixedNumberOfThreads() {
      using(new TestWorker(4)) { }
    }

    /// <summary>
    ///   Verifies that a background worker can be constructed that leaves free a fixed
    ///   number of CPU cores
    /// </summary>
    [Test]
    public void CanPreserveFixedNumberOfCores() {
      using(new TestWorker(-2)) { }
    }

    /// <summary>
    ///   Verifies that a background worker can be constructed using a specific name
    ///   for its worker threads
    /// </summary>
    [Test]
    public void CanUseNamedThreads() {
      using(new TestWorker("Test Task Thread")) { }
    }

    /// <summary>
    ///   Verifies that a background worker can be constructed that uses a fixed number
    ///   of threads using a specific name
    /// </summary>
    [Test]
    public void CanUseFixedNumberOfNamedThreads() {
      using(new TestWorker("Test Task Thread", 4)) { }
    }

    /// <summary>
    ///   Verifies that a background worker can be constructed that leaves free a fixed
    ///   number of CPU cores and uses a specific name for its worker threads.
    /// </summary>
    [Test]
    public void CanPreserveFixedNumberOfCoresAndUseNamedThreads() {
      using(new TestWorker("Test Task Thread", -2)) { }
    }

    /// <summary>
    ///   Verifies that exceptions happening inside the tasks are collected and re-thrown
    ///   in the Join() method.
    /// </summary>
    [Test]
    public void ExceptionsAreReThrownInJoin() {
      using(var testWorker = new TestWorker()) {
        testWorker.ThrowException = true;
        testWorker.AddTask(new object());
        testWorker.AddTask(new object());

        Assert.Throws<AggregateException>(
          () => {
            testWorker.Join();
          }
        );

        try {
          testWorker.Join();
          Assert.Fail(
            "Calling ParallelBackgroundWorker.Join() multiple times should re-throw " +
            "exceptions multiple times"
          );
        }
        catch(AggregateException aggregateException) {
          Assert.AreEqual(2, aggregateException.InnerExceptions.Count);
        }
      }
    }

    /// <summary>
    ///   Verifies that tasks can be cancelled while they are running
    /// </summary>
    [Test]
    public void TasksCanBeCancelled() {
      using(var waitEvent = new ManualResetEvent(false)) {
        using(var testWorker = new TestWorker()) {
          testWorker.WaitEvent = waitEvent;

          testWorker.AddTask(new object());
          testWorker.CancelRunningTasks();

          waitEvent.Set();

          Assert.IsTrue(testWorker.Wait(1000));

          Assert.IsTrue(testWorker.WasCancelled);
        }
      } // disposes waitEvent
    }

    /// <summary>Verifies that calling Join() waits for all queued tasks</summary>
    [Test]
    public void JoinWaitsForQueuedTasks() {
      var tasks = new List<object>(100);
      for(int index = 0; index < 100; ++index) {
        tasks.Add(new object());
      }

      using(var waitEvent = new ManualResetEvent(false)) {
        using(var testWorker = new TestWorker(2)) {
          testWorker.WaitEvent = waitEvent;
          testWorker.Tasks = new List<object>();
          for(int index = 0; index < 100; ++index) {
            testWorker.AddTask(tasks[index]);
          }

          CollectionAssert.IsEmpty(testWorker.Tasks);

          waitEvent.Set();
          testWorker.Join();

          lock(testWorker.Tasks) {
            CollectionAssert.AreEquivalent(tasks, testWorker.Tasks);
          }
        }
      } // disposes waitEvent
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
