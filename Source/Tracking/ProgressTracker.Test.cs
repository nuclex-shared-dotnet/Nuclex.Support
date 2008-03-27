#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the progression tracker class</summary>
  [TestFixture]
  public class ProgressionTrackerTest {

    #region interface IProgressionTrackerSubscriber

    /// <summary>Interface used to test the progression tracker.</summary>
    public interface IProgressionTrackerSubscriber {

      /// <summary>Called when the progression tracker's progress changes</summary>
      /// <param name="sender">Progression tracker whose progress has changed</param>
      /// <param name="e">Contains the new progress achieved</param>
      void ProgressChanged(object sender, ProgressReportEventArgs e);

      /// <summary>Called when the progression tracker's idle state changes</summary>
      /// <param name="sender">Progression tracker whose idle state has changed</param>
      /// <param name="e">Contains the new idle state of the tracker</param>
      void IdleStateChanged(object sender, IdleStateEventArgs e);

    }

    #endregion // interface IProgressionTrackerSubscriber

    #region class ProgressUpdateEventArgsMatcher

    /// <summary>Compares two ProgressUpdateEventArgsInstances for NMock validation</summary>
    private class ProgressReportEventArgsMatcher : Matcher {

      /// <summary>Initializes a new ProgressUpdateEventArgsMatcher </summary>
      /// <param name="expected">Expected progress update event arguments</param>
      public ProgressReportEventArgsMatcher(ProgressReportEventArgs expected) {
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
        ProgressReportEventArgs actual = (actualAsObject as ProgressReportEventArgs);
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
      private ProgressReportEventArgs expected;

    }

    #endregion // class ProgressUpdateEventArgsMatcher

    #region class TestProgression

    /// <summary>Progression used for testing in this unit test</summary>
    private class TestProgression : Waitable, IProgressReporter {

      /// <summary>will be triggered to report when progress has been achieved</summary>
      public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

      /// <summary>Changes the testing progression's indicated progress</summary>
      /// <param name="progress">
      ///   New progress to be reported by the testing progression
      /// </param>
      public void ChangeProgress(float progress) {
        OnAsyncProgressChanged(progress);
      }

      /// <summary>Transitions the progression into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="progress">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this progression about the achieved progress.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(float progress) {
        OnAsyncProgressChanged(new ProgressReportEventArgs(progress));
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="eventArguments">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this progression about the achieved progress.
      ///   Allows for classes derived from the Progression class to easily provide
      ///   a custom event arguments class that has been derived from the
      ///   Progression's ProgressUpdateEventArgs class.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(ProgressReportEventArgs eventArguments) {
        EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
        if(copy != null)
          copy(this, eventArguments);
      }

    }

    #endregion // class TestProgression

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the tracker properly sums the progress</summary>
    [Test]
    public void TestSummedProgress() {
      ProgressTracker tracker = new ProgressTracker();

      IProgressionTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

      Expect.Once.On(mockedSubscriber).
        Method("IdleStateChanged").
        WithAnyArguments();

      Expect.Exactly(2).On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
          }
        );

      TestProgression test1 = new TestProgression();
      tracker.Track(test1);
      TestProgression test2 = new TestProgression();
      tracker.Track(test2);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.25f))
          }
        );

      test1.ChangeProgress(0.5f);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker only removes progressions when the whole
    ///   tracking list has reached the 'ended' state.
    /// </summary>
    /// <remarks>
    ///   If the tracker would remove ended progressions right when they finished,
    ///   the total progress would jump back each time. This is unwanted, of course.
    /// </remarks>
    [Test]
    public void TestDelayedRemoval() {
      ProgressTracker tracker = new ProgressTracker();

      IProgressionTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

      Expect.Once.On(mockedSubscriber).
        Method("IdleStateChanged").
        WithAnyArguments();

      Expect.Exactly(2).On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
          }
        );

      TestProgression test1 = new TestProgression();
      tracker.Track(test1);
      TestProgression test2 = new TestProgression();
      tracker.Track(test2);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.25f))
          }
        );

      test1.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.75f))
          }
        );

      // Total progress should be 0.75 after this call (one progression at 1.0,
      // the other one at 0.5). If the second progression would be removed,
      // the progress would jump to 0.5 instead.
      test2.End();

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(1.0f))
          }
        );

      Expect.Once.On(mockedSubscriber).
        Method("IdleStateChanged").
        WithAnyArguments();

      test1.End();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker behaves correctly if it is fed with progressions
    ///   that have already ended.
    /// </summary>
    [Test]
    public void TestSoleEndedProgression() {
      ProgressTracker tracker = new ProgressTracker();

      IProgressionTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

      tracker.Track(Waitable.EndedDummy);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker behaves correctly if it is fed with progressions
    ///   that have already ended in addition to progressions that are actively executing.
    /// </summary>
    [Test]
    public void TestEndedProgression() {
      ProgressTracker tracker = new ProgressTracker();

      IProgressionTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

      Expect.Once.On(mockedSubscriber).
        Method("IdleStateChanged").
        WithAnyArguments();

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
          }
        );

      TestProgression test1 = new TestProgression();
      tracker.Track(test1);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.5f))
          }
        );

      tracker.Track(Waitable.EndedDummy);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
            new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(1.0f))
          }
        );

      Expect.Once.On(mockedSubscriber).
        Method("IdleStateChanged").
        WithAnyArguments();

      test1.End();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Tries to provoke a deadlock by re-entering the tracker from one of
    ///   its own events.
    /// </summary>
    [Test]
    public void TestProvokedDeadlock() {
      ProgressTracker tracker = new ProgressTracker();

      TestProgression test1 = new TestProgression();
      tracker.Track(test1);

      tracker.AsyncIdleStateChanged +=
        (EventHandler<IdleStateEventArgs>)delegate(object sender, IdleStateEventArgs arguments) {
          tracker.Track(Waitable.EndedDummy);
        };

      test1.End();
    }

    /// <summary>Mocks a subscriber for the events of a tracker</summary>
    /// <param name="tracker">Tracker to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IProgressionTrackerSubscriber mockSubscriber(ProgressTracker tracker) {
      IProgressionTrackerSubscriber mockedSubscriber =
        this.mockery.NewMock<IProgressionTrackerSubscriber>();

      tracker.AsyncIdleStateChanged +=
        new EventHandler<IdleStateEventArgs>(mockedSubscriber.IdleStateChanged);

      tracker.AsyncProgressChanged +=
        new EventHandler<ProgressReportEventArgs>(mockedSubscriber.ProgressChanged);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
