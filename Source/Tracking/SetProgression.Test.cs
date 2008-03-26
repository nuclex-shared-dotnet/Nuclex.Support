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

  /// <summary>Unit Test for the progression set class</summary>
  [TestFixture]
  public class SetProgressionTest {

    #region interface ISetProgressionSubscriber

    /// <summary>Interface used to test the set progression.</summary>
    public interface ISetProgressionSubscriber {

      /// <summary>Called when the set progression's progress changes</summary>
      /// <param name="sender">Set progression whose progress has changed</param>
      /// <param name="e">Contains the new progress achieved</param>
      void ProgressChanged(object sender, ProgressReportEventArgs e);

      /// <summary>Called when the set progression has ended</summary>
      /// <param name="sender">Set progression that as ended</param>
      /// <param name="e">Not used</param>
      void Ended(object sender, EventArgs e);

    }

    #endregion // interface ISetProgressionSubscriber

    #region class ProgressUpdateEventArgsMatcher

    /// <summary>Compares two ProgressUpdateEventArgsInstances for NMock validation</summary>
    private class ProgressUpdateEventArgsMatcher : Matcher {

      /// <summary>Initializes a new ProgressUpdateEventArgsMatcher </summary>
      /// <param name="expected">Expected progress update event arguments</param>
      public ProgressUpdateEventArgsMatcher(ProgressReportEventArgs expected) {
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

    #region class TestWaitable

    /// <summary>Progression used for testing in this unit test</summary>
    private class TestWaitable : Waitable, IProgressReporter {

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

    #endregion // class TestWaitable

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the set progression properly sums the progress</summary>
    [Test]
    public void TestSummedProgress() {
      SetProgression<TestWaitable> testSetProgression =
        new SetProgression<TestWaitable>(
          new TestWaitable[] { new TestWaitable(), new TestWaitable() }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestWaitable>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.25f))
          }
        );

      testSetProgression.Children[0].Waitable.ChangeProgress(0.5f);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Validates that the set progression respects the weights</summary>
    [Test]
    public void TestWeightedSummedProgress() {
      SetProgression<TestWaitable> testSetProgression =
        new SetProgression<TestWaitable>(
          new WeightedWaitable<TestWaitable>[] {
            new WeightedWaitable<TestWaitable>(new TestWaitable(), 1.0f),
            new WeightedWaitable<TestWaitable>(new TestWaitable(), 2.0f)
          }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestWaitable>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.5f / 3.0f))
          }
        );

      testSetProgression.Children[0].Waitable.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestWaitable>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.5f))
          }
        );

      testSetProgression.Children[1].Waitable.ChangeProgress(0.5f);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the ended event is triggered when the last progression ends
    /// </summary>
    [Test]
    public void TestEndedEvent() {
      SetProgression<TestWaitable> testSetProgression =
        new SetProgression<TestWaitable>(
          new TestWaitable[] { new TestWaitable(), new TestWaitable() }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Exactly(2).On(mockedSubscriber).
        Method("ProgressUpdated").
        WithAnyArguments();

      Expect.Once.On(mockedSubscriber).
        Method("Ended").
        WithAnyArguments();
      
      testSetProgression.Children[0].Waitable.End();
      testSetProgression.Children[1].Waitable.End();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of a progression</summary>
    /// <param name="progression">Progression to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private ISetProgressionSubscriber mockSubscriber(Waitable progression) {
      ISetProgressionSubscriber mockedSubscriber =
        this.mockery.NewMock<ISetProgressionSubscriber>();

      progression.AsyncEnded += new EventHandler(mockedSubscriber.Ended);
      (progression as IProgressReporter).AsyncProgressChanged +=
        new EventHandler<ProgressReportEventArgs>(mockedSubscriber.ProgressChanged);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
