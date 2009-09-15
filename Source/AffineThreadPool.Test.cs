#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the CPU core-affine thread pool</summary>
  [TestFixture]
  public class AffineThreadPoolTest {

    #region class TestTask

    /// <summary>ThreadPool task that can be used for testing</summary>
    private class TestTask : IDisposable {

      /// <summary>Initializes a new test task</summary>
      public TestTask() {
        this.callbackEvent = new ManualResetEvent(false);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.callbackEvent != null) {
          this.callbackEvent.Close();
          this.callbackEvent = null;
        }
      }

      /// <summary>Callback that can be added to the thread pool as a task</summary>
      /// <param name="state">User defined state</param>
      public void Callback(object state) {
        this.LastCallbackState = state;
        this.callbackEvent.Set();
      }

      /// <summary>Event that will be set when the callback is executed</summary>
      public ManualResetEvent CallbackEvent {
        get { return this.callbackEvent; }
      }

      /// <summary>
      ///   State parameter that was provide when the callback was called
      /// </summary>
      public volatile object LastCallbackState;

      /// <summary>Event that will be set when the callback is invoked</summary>
      private ManualResetEvent callbackEvent;

    }

    #endregion // class TestTask

    #region class WaitTask

    /// <summary>ThreadPool task that can be used for testing</summary>
    private class WaitTask : IDisposable {

      /// <summary>Initializes a new test task</summary>
      public WaitTask() {
        this.startEvent = new ManualResetEvent(false);
        this.finishEvent = new ManualResetEvent(false);
        this.waitEvent = new ManualResetEvent(false);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.waitEvent != null) {
          this.waitEvent.Close();
          this.waitEvent = null;
        }
        if(this.finishEvent != null) {
          this.finishEvent.Close();
          this.finishEvent = null;
        }
        if(this.startEvent != null) {
          this.startEvent.Close();
          this.startEvent = null;
        }
      }

      /// <summary>Callback that can be added to the thread pool as a task</summary>
      /// <param name="state">User defined state</param>
      public void Callback(object state) {
        this.LastCallbackState = state;
        this.startEvent.Set();
        this.waitEvent.WaitOne();
        this.finishEvent.Set();
      }

      /// <summary>Event that will be set when the callback has started</summary>
      public ManualResetEvent StartEvent {
        get { return this.startEvent; }
      }

      /// <summary>Event that will be set when the callback has finished</summary>
      public ManualResetEvent FinishEvent {
        get { return this.finishEvent; }
      }

      /// <summary>Event that blocks the callback</summary>
      public ManualResetEvent WaitEvent {
        get { return this.waitEvent; }
      }

      /// <summary>
      ///   State parameter that was provide when the callback was called
      /// </summary>
      public volatile object LastCallbackState;

      /// <summary>Event that will be set when the callback has started</summary>
      private ManualResetEvent startEvent;
      /// <summary>Event that will be set when the callback has finished</summary>
      private ManualResetEvent finishEvent;
      /// <summary>Event used to block the callback</summary>
      private ManualResetEvent waitEvent;

    }

    #endregion // class WaitTask

    #region class ThrowingDisposable

    /// <summary>Throws an exception when it is disposed</summary>
    private class ThrowingDisposable : IDisposable {

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        throw new ArithmeticException("Simulated exception for unit testing");
      }

    }

    #endregion // class ThrowingDisposable

    /// <summary>
    ///   Verifies that the Thread Pool's default assertion handler is working
    /// </summary>
    [Test]
    public void TestDefaultAssertionHandler() {

      // We can't test a failing assertion because our tests need to run
      // unattended on a build server without blocking for user input.
      AffineThreadPool.DefaultAssertionHandler(
        true, "Unit test", "This should not fail"
      );

    }

    /// <summary>Tests whether the QueueUserWorkItem() method is working</summary>
    [Test]
    public void TestQueueUserWorkItem() {
      using(TestTask task = new TestTask()) {
        AffineThreadPool.QueueUserWorkItem(task.Callback);
        Assert.IsTrue(task.CallbackEvent.WaitOne(1000));
      }
    }

    /// <summary>
    ///   Verifies that the QueueUserWorkItem() method is passing the state parameter
    ///   on to the callback
    /// </summary>
    [Test]
    public void TestQueueUserWorkItemWithState() {
      using(TestTask task = new TestTask()) {
        object state = new object();

        AffineThreadPool.QueueUserWorkItem(task.Callback, state);

        Assert.IsTrue(task.CallbackEvent.WaitOne(1000));
        Assert.AreSame(state, task.LastCallbackState);
      }
    }

    /// <summary>
    ///   Tests whether the thread pool can handle an exception from a user work item
    /// </summary>
    [Test]
    public void TestExceptionFromUserWorkItem() {
      using(ManualResetEvent assertEvent = new ManualResetEvent(false)) {
        AffineThreadPool.AssertionDelegate oldAssertionHandler =
          AffineThreadPool.AssertionHandler;

        AffineThreadPool.AssertionHandler = delegate(
          bool condition, string message, string details
        ) { assertEvent.Set(); };
        try {
          AffineThreadPool.QueueUserWorkItem(
            delegate(object state) { throw new KeyNotFoundException(); }
          );
          Assert.IsTrue(assertEvent.WaitOne(1000));
        }
        finally {
          AffineThreadPool.AssertionHandler = oldAssertionHandler;
        }
      }
    }

    /// <summary>
    ///   Verifies that the affine thread pool's maximum thread count equals
    ///   the number of logical processors in the system
    /// </summary>
    [Test]
    public void TestMaxThreadsProperty() {
      Assert.AreEqual(Environment.ProcessorCount, AffineThreadPool.MaxThreads);
    }

    /// <summary>
    ///   Tests whether the thread pool can handle an exception from a user work item
    /// </summary>
    [Test]
    public void TestExceptionFromDisposableState() {
      using(ManualResetEvent assertEvent = new ManualResetEvent(false)) {
        AffineThreadPool.AssertionDelegate oldAssertionHandler =
          AffineThreadPool.AssertionHandler;

        AffineThreadPool.AssertionHandler = delegate(
          bool condition, string message, string details
        ) { assertEvent.Set(); };

        try {
          int eventCount = AffineThreadPool.CpuCores;
          WaitTask[] tasks = new WaitTask[eventCount];

          int createdTasks = 0;
          try {

            // Create the tasks, counting up the created task counter. If an exception
            // occurs, we will roll back from there.
            for(createdTasks = 0; createdTasks < eventCount; ++createdTasks) {
              tasks[createdTasks] = new WaitTask();
            }

            // Schedule the blocking tasks in the thread pool so it will not be able
            // to process the next task we add to the queue
            for(int index = 0; index < eventCount; ++index) {
              AffineThreadPool.QueueUserWorkItem(tasks[index].Callback);
            }

            // Wait for the tasks to start so they aren't aborted by EmptyQueue()
            for(int index = 0; index < eventCount; ++index) {
              Assert.IsTrue(tasks[index].StartEvent.WaitOne(1000));
            }
            Assert.AreEqual(createdTasks, AffineThreadPool.ActiveThreads);
            Assert.AreEqual(0, AffineThreadPool.WaitingCallbacks);

            // Add a task to the queue whose state implements a faulty IDisposable
            AffineThreadPool.QueueUserWorkItem(
              delegate(object state) { }, new ThrowingDisposable()
            );

            Assert.AreEqual(1, AffineThreadPool.WaitingCallbacks);

            // Now clear the thread pool. This should cause the faulty IDisposable
            // to be disposed and then throw its exception.
            AffineThreadPool.EmptyQueue();

            // Make sure our custom assertion handler has been triggered
            Assert.IsTrue(assertEvent.WaitOne(1000));

            Assert.AreEqual(createdTasks, AffineThreadPool.ActiveThreads);
            Assert.AreEqual(0, AffineThreadPool.WaitingCallbacks);

            // Let the thread pool finish its active tasks
            for(int index = 0; index < eventCount; ++index) {
              tasks[index].WaitEvent.Set();
            }

            // Wait for the tasks to end before we dispose them
            for(int index = 0; index < eventCount; ++index) {
              Assert.IsTrue(tasks[index].FinishEvent.WaitOne(1000));
            }
          }
          finally {
            for(--createdTasks; createdTasks >= 0; --createdTasks) {
              tasks[createdTasks].Dispose();
            }
          }
        }
        finally {
          AffineThreadPool.AssertionHandler = oldAssertionHandler;
        }
      } // using
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
