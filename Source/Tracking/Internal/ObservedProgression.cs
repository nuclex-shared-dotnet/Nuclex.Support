using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Progression being observed by another class</summary>
  /// <typeparam name="ProgressionType">
  ///   Type of the progression that is being observed
  /// </typeparam>
  internal class ObservedProgression<ProgressionType> : IDisposable
    where ProgressionType : Progression {

    /// <summary>Delegate for reporting progress updates</summary>
    public delegate void ReportDelegate();

    /// <summary>Initializes a new observed progression</summary>
    /// <param name="weightedProgression">Weighted progression being observed</param>
    /// <param name="progressUpdateCallback">
    ///   Callback to invoke when the progression's progress changes
    /// </param>
    /// <param name="endedCallback">
    ///   Callback to invoke when the progression has ended
    /// </param>
    internal ObservedProgression(
      WeightedProgression<ProgressionType> weightedProgression,
      ReportDelegate progressUpdateCallback,
      ReportDelegate endedCallback
    ) {
      this.weightedProgression = weightedProgression;
      this.endedCallback = endedCallback;
      this.progressUpdateCallback = progressUpdateCallback;

      this.weightedProgression.Progression.AsyncEnded +=
        new EventHandler(asyncEnded);

      this.weightedProgression.Progression.AsyncProgressUpdated +=
        new EventHandler<ProgressUpdateEventArgs>(asyncProgressUpdated);
    }

    /// <summary>Immediately releases all resources owned by the object</summary>
    public void Dispose() {
      asyncDisconnectEvents();
    }

    /// <summary>Weighted progression being observed</summary>
    public WeightedProgression<ProgressionType> WeightedProgression {
      get { return this.weightedProgression; }
    }

    /// <summary>Amount of progress this progression has achieved so far</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Called when the observed progression has ended</summary>
    /// <param name="sender">Progression that has ended</param>
    /// <param name="e">Not used</param>
    private void asyncEnded(object sender, EventArgs e) {
      ReportDelegate endedCallback = this.endedCallback;
      ReportDelegate progressUpdateCallback = this.progressUpdateCallback;

      asyncDisconnectEvents(); // We don't need those anymore!

      this.progress = 1.0f;
      progressUpdateCallback();

      endedCallback();
    }

    /// <summary>Called when the progress of the observed progression changes</summary>
    /// <param name="sender">Progression whose progress has changed</param>
    /// <param name="e">Contains the updated progress</param>
    private void asyncProgressUpdated(object sender, ProgressUpdateEventArgs e) {
      this.progress = e.Progress;

      this.progressUpdateCallback();
    }

    /// <summary>Unscribes from all events of the observed progression</summary>
    private void asyncDisconnectEvents() {

      // Make use of the double check locking idiom to avoid the costly lock when
      // the events have already been unsubscribed
      if(this.endedCallback != null) {

        // This is an internal class with special knowledge that there
        // is no risk of deadlock involved, so we don't need a fancy syncRoot!
        lock(this) {
          if(this.endedCallback != null) {
            this.weightedProgression.Progression.AsyncEnded -=
              new EventHandler(asyncEnded);

            this.weightedProgression.Progression.AsyncProgressUpdated -=
              new EventHandler<ProgressUpdateEventArgs>(asyncProgressUpdated);

            this.endedCallback = null;
            this.progressUpdateCallback = null;
          }
        }

      } // endedCallback != null

    }

    /// <summary>The weighted progression that is being observed</summary>
    private WeightedProgression<ProgressionType> weightedProgression;
    /// <summary>Callback to invoke when the progress updates</summary>
    private volatile ReportDelegate progressUpdateCallback;
    /// <summary>Callback to invoke when the progression ends</summary>
    private volatile ReportDelegate endedCallback;
    /// <summary>Progress achieved so far</summary>
    private volatile float progress;
  }

} // namespace Nuclex.Support.Tracking
