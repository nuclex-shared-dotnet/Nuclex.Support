#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

#if !NO_CONCURRENT_COLLECTIONS

using System;
using System.Threading;
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support.Threading {

  /// <summary>Unit Test for the thread runner class</summary>
  [TestFixture]
  internal class ThreadRunnerTest {

    #region class DefaultDisposeRunner

    /// <summary>Implementation of a thread runner to check default dispose behavior</summary>
    private class DefaultDisposeRunner : ThreadRunner {

      /// <summary>Reports an error</summary>
      /// <param name="exception">Error that will be reported</param>
      protected override void ReportError(Exception exception) { }

      /// <summary>Called when the status of the busy flag changes</summary>
      protected override void BusyChanged() { }

    }

    #endregion // class DefaultDisposeRunner

    #region class DummyRunner

    /// <summary>Implementation of a thread runner used for unit testing</summary>
    private class DummyRunner : ThreadRunner {

      /// <summary>Initializes a new dummy thread runner</summary>
      public DummyRunner() : base() {
        this.completionGate = new ManualResetEvent(initialState: false);
      }

      /// <summary>Immediately frees all resources used by the instance</summary>
      public new void Dispose() {
        base.Dispose(100);

        if(this.completionGate != null) {
          this.completionGate.Dispose();
          this.completionGate = null;
        }
      }

      /// <summary>Waits for the task for complete (all of 100 milliseconds)</summary>
      /// <returns>True if the task completed, false if it continues running</returns>
      public bool WaitForCompletion() {
        return this.completionGate.WaitOne(100);
      }

      /// <summary>How often the status of the busy flag has changed</summary>
      public int BusyChangeCount {
        get { return this.busyChangeCount; }
      }

      /// <summary>Error that has been reported the last time a task was run</summary>
      public Exception ReportedError {
        get { return this.reportedError; }
      }

      /// <summary>Reports an error</summary>
      /// <param name="exception">Error that will be reported</param>
      protected override void ReportError(Exception exception) {
        this.reportedError = exception;
      }

      /// <summary>Called when the status of the busy flag changes</summary>
      protected override void BusyChanged() {
        ++busyChangeCount;
        if((busyChangeCount >= 2) && (base.IsBusy == false)) {
          this.completionGate.Set();
        }
      }

      /// <summary>Last error that was reported in the thread</summary>
      private Exception reportedError;
      /// <summary>Number of times the busy state of the runner has changed</summary>
      private int busyChangeCount;
      /// <summary>Triggered when the busy event has performed a double flank</summary>
      private ManualResetEvent completionGate;

    }

    #endregion // class DummyRunner

    #region class DummyTask

    /// <summary>Dummy task that can be executed by a thread runner</summary>
    private class DummyTask : IDisposable {

      /// <summary>Initializes a new dummy task</summary>
      /// <param name="delayMilliseconds">How long the task shoudl take to execute</param>
      public DummyTask(int delayMilliseconds) {
        this.startGate = new ManualResetEvent(initialState: false);
        this.delayMilliseconds = delayMilliseconds;
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.startGate != null) {
          this.startGate.Dispose();
          this.startGate = null;
        }
      }

      /// <summary>Waits for the task to start (all of 100 milliseconds)</summary>
      /// <returns>True if the start started, false if it didn't</returns>
      public bool WaitForStart() {
        return this.startGate.WaitOne(100);
      }

      /// <summary>Sets the task up to fail with the specified error</summary>
      /// <param name="error">Error the task will fail with</param>
      public void FailWith(Exception error) {
        this.error = error;
      }

      /// <summary>Runs the task with no arguments</summary>
      public void Run() {
        this.startGate.Set();

        ++this.executionCount;
        Thread.Sleep(this.delayMilliseconds);
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>Runs the task with one argument</summary>
      /// <param name="firstArgument">First argument passed from the runner</param>
      public void Run(float firstArgument) {
        this.startGate.Set();

        ++this.executionCount;
        this.firstArgument = firstArgument;
        Thread.Sleep(this.delayMilliseconds);
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>Runs the task with two argument</summary>
      /// <param name="firstArgument">First argument passed from the runner</param>
      /// <param name="secondArgument">Second argument passed from the runner</param>
      public void Run(float firstArgument, string secondArgument) {
        this.startGate.Set();

        ++this.executionCount;
        this.firstArgument = firstArgument;
        this.secondArgument = secondArgument;
        Thread.Sleep(this.delayMilliseconds);
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>Runs the task with no arguments</summary>
      /// <param name="cancellationToken">Token by which cancellation can be signalled</param>
      public void RunCancellable(CancellationToken cancellationToken) {
        this.startGate.Set();

        ++this.executionCount;

        if(delayMilliseconds == 0) {
          Thread.Sleep(0);
        } else {
          if(cancellationToken.WaitHandle.WaitOne(delayMilliseconds)) {
            this.wasCancelled = cancellationToken.IsCancellationRequested;
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>Runs the task with one argument</summary>
      /// <param name="firstArgument">First argument passed from the runner</param>
      /// <param name="cancellationToken">Token by which cancellation can be signalled</param>
      public void RunCancellable(float firstArgument, CancellationToken cancellationToken) {
        this.startGate.Set();

        ++this.executionCount;
        this.firstArgument = firstArgument;

        if(delayMilliseconds == 0) {
          Thread.Sleep(0);
        } else {
          if(cancellationToken.WaitHandle.WaitOne(delayMilliseconds)) {
            this.wasCancelled = cancellationToken.IsCancellationRequested;
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>Runs the task with two argument</summary>
      /// <param name="firstArgument">First argument passed from the runner</param>
      /// <param name="secondArgument">Second argument passed from the runner</param>
      /// <param name="cancellationToken">Token by which cancellation can be signalled</param>
      public void RunCancellable(
        float firstArgument, string secondArgument, CancellationToken cancellationToken
      ) {
        this.startGate.Set();

        ++this.executionCount;
        this.firstArgument = firstArgument;
        this.secondArgument = secondArgument;

        if(delayMilliseconds == 0) {
          Thread.Sleep(0);
        } else {
          if(cancellationToken.WaitHandle.WaitOne(delayMilliseconds)) {
            this.wasCancelled = cancellationToken.IsCancellationRequested;
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
        if(this.error != null) {
          throw this.error;
        }
      }

      /// <summary>How many times the task was run</summary>
      public int ExecutionCount {
        get { return this.executionCount; }
      }

      /// <summary>Whether the task was cancelled by the runner itself</summary>
      public bool WasCancelled {
        get { return this.wasCancelled; }
      }

      /// <summary>What the first argument was during the last call</summary>
      public float FirstArgument {
        get { return this.firstArgument; }
      }

      /// <summary>What the second argument was during the last call</summary>
      public string SecondArgument {
        get { return this.secondArgument; }
      }

      /// <summary>Last error that was reported in the thread</summary>
      private Exception error;
      /// <summary>Triggered when the task has started</summary>
      private ManualResetEvent startGate;
      /// <summary>How long the task should take to execute in milliseconds</summary>
      private int delayMilliseconds;
      /// <summary>How many times the task has been executed</summary>
      private volatile int executionCount;
      /// <summary>Whether the task has been cancelled</summary>
      private volatile bool wasCancelled;
      /// <summary>First argument that was passed to the task</summary>
      private volatile float firstArgument;
      /// <summary>Second argument that was passed to the task</summary>
      private volatile string secondArgument;

    }

    #endregion // class DummyRunner

    /// <summary>Verifies that the thread runner has a default constructor</summary>
    [Test]
    public void CanBeDefaultConstructed() {
      using(new DummyRunner()) { }
    }

    /// <summary>Checks that the runner sets and unsets its busy flag</summary>
    [Test]
    public void BusyFlagIsToggled() {
      using(var runner = new DummyRunner()) {
        int busyFlagChangeCount = runner.BusyChangeCount;
        Assert.IsFalse(runner.IsBusy);

        runner.RunInBackground((Action)delegate() { });
        Assert.IsTrue(runner.WaitForCompletion());

        Assert.GreaterOrEqual(busyFlagChangeCount + 2, runner.BusyChangeCount);
        Assert.IsFalse(runner.IsBusy);
      }
    }

    /// <summary>Lets the thread runner run a simple task in the background</summary>
    [Test]
    public void CanRunSimpleTaskInBackground() {
      using(var task = new DummyTask(0)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new Action(task.Run));

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.IsFalse(task.WasCancelled);
      }
    }

    /// <summary>
    ///   Checks that the thread runner is able to pass a single argument to a task
    /// </summary>
    [Test]
    public void CanPassSingleArgumentToSimpleTask() {
      using(var task = new DummyTask(0)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new Action<float>(task.Run), 12.43f);

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.AreEqual(12.43f, task.FirstArgument);
        Assert.IsFalse(task.WasCancelled);
      }
    }

    /// <summary>
    ///   Checks that the thread runner is able to pass two arguments to a task
    /// </summary>
    [Test]
    public void CanPassTwoArgumentsToSimpleTask() {
      using(var task = new DummyTask(0)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new Action<float, string>(task.Run), 98.67f, "Hello");

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.AreEqual(98.67f, task.FirstArgument);
        Assert.AreEqual("Hello", task.SecondArgument);
        Assert.IsFalse(task.WasCancelled);
      }
    }

    /// <summary>
    ///   Verifies that an error happening in a simple task is reported correctly
    /// </summary>
    [Test]
    public void SimpleTaskErrorIsReported() {
      using(var task = new DummyTask(0)) {
        var error = new InvalidOperationException("Mooh!");
        task.FailWith(error);

        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new Action(task.Run));

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.AreSame(error, runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.IsFalse(task.WasCancelled);
      }
    }

    /// <summary>Lets the thread runner run a cancellable task in the background</summary>
    [Test]
    public void CanRunCancellableTaskInBackground() {
      using(var task = new DummyTask(100)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new CancellableAction(task.RunCancellable));
          Assert.IsTrue(task.WaitForStart());
          runner.CancelAllBackgroundOperations();

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.IsTrue(task.WasCancelled);
      }
    }

    /// <summary>
    ///   Checks that the thread runner is able to pass a single argument to a task
    ///   that can be cancelled
    /// </summary>
    [Test]
    public void CanPassSingleArgumentToCancellableTask() {
      using(var task = new DummyTask(100)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(new CancellableAction<float>(task.RunCancellable), 12.43f);
          Assert.IsTrue(task.WaitForStart());
          runner.CancelAllBackgroundOperations();

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.AreEqual(12.43f, task.FirstArgument);
        Assert.IsTrue(task.WasCancelled);
      }
    }

    /// <summary>
    ///   Checks that the thread runner is able to pass two arguments to a task
    ///   that can be cancelled
    /// </summary>
    [Test]
    public void CanPassTwoArgumentsToCancellableTask() {
      using(var task = new DummyTask(100)) {
        using(var runner = new DummyRunner()) {
          runner.RunInBackground(
            new CancellableAction<float, string>(task.RunCancellable), 98.67f, "Hello"
          );
          Assert.IsTrue(task.WaitForStart());
          runner.CancelAllBackgroundOperations();

          Assert.IsTrue(runner.WaitForCompletion());
          Assert.IsNull(runner.ReportedError);
        }

        Assert.AreEqual(1, task.ExecutionCount);
        Assert.AreEqual(98.67f, task.FirstArgument);
        Assert.AreEqual("Hello", task.SecondArgument);
        Assert.IsTrue(task.WasCancelled);
      }
    }

  }

} // namespace Nuclex.Support.Threading

#endif // !NO_CONCURRENT_COLLECTIONS
