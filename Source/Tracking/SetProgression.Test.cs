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
      void ProgressUpdated(object sender, ProgressUpdateEventArgs e);

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

    #region class TestProgression

    /// <summary>Progression used for testing in this unit test</summary>
    private class TestProgression : Progression {

      /// <summary>Changes the testing progression's indicated progress</summary>
      /// <param name="progress">
      ///   New progress to be reported by the testing progression
      /// </param>
      public void ChangeProgress(float progress) {
        OnAsyncProgressUpdated(progress);
      }

      /// <summary>Transitions the progression into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

    }

    #endregion // class TestProgression

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the set progression properly sums the progress</summary>
    [Test]
    public void TestSummedProgress() {
      SetProgression<TestProgression> testSetProgression =
        new SetProgression<TestProgression>(
          new TestProgression[] { new TestProgression(), new TestProgression() }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressUpdated").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestProgression>)),
            new ProgressUpdateEventArgsMatcher(new ProgressUpdateEventArgs(0.25f))
          }
        );

      testSetProgression.Childs[0].Progression.ChangeProgress(0.5f);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Validates that the set progression respects the weights</summary>
    [Test]
    public void TestWeightedSummedProgress() {
      SetProgression<TestProgression> testSetProgression =
        new SetProgression<TestProgression>(
          new WeightedProgression<TestProgression>[] {
            new WeightedProgression<TestProgression>(new TestProgression(), 1.0f),
            new WeightedProgression<TestProgression>(new TestProgression(), 2.0f)
          }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressUpdated").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestProgression>)),
            new ProgressUpdateEventArgsMatcher(new ProgressUpdateEventArgs(0.5f / 3.0f))
          }
        );

      testSetProgression.Childs[0].Progression.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressUpdated").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(SetProgression<TestProgression>)),
            new ProgressUpdateEventArgsMatcher(new ProgressUpdateEventArgs(0.5f))
          }
        );

      testSetProgression.Childs[1].Progression.ChangeProgress(0.5f);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of a progression</summary>
    /// <param name="progression">Progression to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private ISetProgressionSubscriber mockSubscriber(Progression progression) {
      ISetProgressionSubscriber mockedSubscriber =
        this.mockery.NewMock<ISetProgressionSubscriber>();

      progression.AsyncEnded += new EventHandler(mockedSubscriber.Ended);
      progression.AsyncProgressUpdated +=
        new EventHandler<ProgressUpdateEventArgs>(mockedSubscriber.ProgressUpdated);

      return mockedSubscriber;
    }

    /// <summary>
    ///   Validates that the ended event is triggered when the last progression ends
    /// </summary>
    [Test]
    public void TestEndedEvent() {
      SetProgression<TestProgression> testSetProgression =
        new SetProgression<TestProgression>(
          new TestProgression[] { new TestProgression(), new TestProgression() }
        );

      ISetProgressionSubscriber mockedSubscriber = mockSubscriber(testSetProgression);

      Expect.Exactly(2).On(mockedSubscriber).
        Method("ProgressUpdated").
        WithAnyArguments();

      Expect.Once.On(mockedSubscriber).
        Method("Ended").
        WithAnyArguments();
      
      testSetProgression.Childs[0].Progression.End();
      testSetProgression.Childs[1].Progression.End();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
