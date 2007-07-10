#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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
using System.Collections.Generic;
using System.IO;

#if UNITTEST

using NUnit.Framework;
using NMock2;

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the queue operation class</summary>
  [TestFixture]
  public class QueueOperationTest {

    #region interface IQueueOperationSubscriber

    /// <summary>Interface used to test the set progression.</summary>
    public interface IQueueOperationSubscriber {

      /// <summary>Called when the queue operations's progress changes</summary>
      /// <param name="sender">Queue operation whose progress has changed</param>
      /// <param name="e">Contains the new progress achieved</param>
      void ProgressUpdated(object sender, ProgressUpdateEventArgs e);

      /// <summary>Called when the queue operation has ended</summary>
      /// <param name="sender">Queue operation that as ended</param>
      /// <param name="e">Not used</param>
      void Ended(object sender, EventArgs e);

    }

    #endregion // interface IQueueOperationSubscriber

    #region class ProgressUpdateEventArgsMatcher

    /// <summary>Compares two ProgressUpdateEventArgsInstances for NMock validation</summary>
    private class ProgressUpdateEventArgsMatcher : Matcher {

      /// <summary>Initializes a new ProgressUpdateEventArgsMatcher </summary>
      /// <param name="expected">Expected progress update event arguments</param>
      public ProgressUpdateEventArgsMatcher(ProgressUpdateEventArgs expected) {
        this.expected = expected;
      }

      /// <summary>
      ///   Called by NMock to verfiy the ProgressUpdateEventArgs match the expected value
      /// </summary>
      /// <param name="actualAsObject">Actual value to compare to the expected value</param>
      /// <returns>
      ///   True if the actual value matches the expected value; otherwise false
      /// </returns>
      public override bool Matches(object actualAsObject) {
        ProgressUpdateEventArgs actual = (actualAsObject as ProgressUpdateEventArgs);
        if(actual == null)
          return false;

        return (actual.Progress == this.expected.Progress);
      }

      /// <summary>Creates a string representation of the expected value</summary>
      /// <param name="writer">Writer to write the string representation into</param>
      public override void DescribeTo(TextWriter writer) {
        writer.Write(this.expected.Progress.ToString());
      }

      /// <summary>Expected progress update event args value</summary>
      private ProgressUpdateEventArgs expected;

    }

    #endregion // class ProgressUpdateEventArgsMatcher

    #region class TestOperation

    /// <summary>Progression used for testing in this unit test</summary>
    private class TestOperation : Operation {

      /// <summary>Begins executing the operation. Yeah, sure :)</summary>
      public override void Begin() { }

      /// <summary>Moves the operation into the ended state</summary>
      public void SetEnded() {
        SetEnded(null);
      }

      /// <summary>Moves the operation into the ended state with an exception</summary>
      /// <param name="exception">Exception</param>
      public void SetEnded(Exception exception) {
        SetException(exception);
        OnAsyncEnded();
      }

      /// <summary>Changes the testing progression's indicated progress</summary>
      /// <param name="progress">
      ///   New progress to be reported by the testing progression
      /// </param>
      public void ChangeProgress(float progress) {
        OnAsyncProgressUpdated(progress);
      }

    }

    #endregion // class TestOperation

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the queue executes operations sequentially</summary>
    [Test]
    public void TestSequentialExecution() {
      TestOperation operation1 = new TestOperation();
      TestOperation operation2 = new TestOperation();

      QueueOperation<TestOperation> testQueueOperation =
        new QueueOperation<TestOperation>(
          new TestOperation[] { operation1, operation2 }
        );

      IQueueOperationSubscriber mockedSubscriber = mockSubscriber(testQueueOperation);

      testQueueOperation.Begin();

      Expect.Once.On(mockedSubscriber).
        Method("ProgressUpdated").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(QueueOperation<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressUpdateEventArgs(0.25f))
          }
        );

      operation1.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressUpdated").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(QueueOperation<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressUpdateEventArgs(0.5f))
          }
        );

      operation1.SetEnded();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of an operation</summary>
    /// <param name="operation">Operation to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IQueueOperationSubscriber mockSubscriber(Operation operation) {
      IQueueOperationSubscriber mockedSubscriber =
        this.mockery.NewMock<IQueueOperationSubscriber>();

      operation.AsyncEnded += new EventHandler(mockedSubscriber.Ended);
      operation.AsyncProgressUpdated +=
        new EventHandler<ProgressUpdateEventArgs>(mockedSubscriber.ProgressUpdated);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;
  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
