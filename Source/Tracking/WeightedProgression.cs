using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Progression with an associated weight for the total progress</summary>
  internal class WeightedProgression<ProgressionType> : IDisposable
    where ProgressionType : Progression {

    /// <summary>Initializes a new weighted progression</summary>
    /// <param name="callback">Callback to pass progress updates on to</param>
    /// <param name="progression">Progression whose progress to monitor</param>
    /// <param name="weight">Weighting of the progression's progress</param>
    public WeightedProgression(
      ProgressionType progression,
      EventHandler<ProgressUpdateEventArgs> callback,
      float weight
    ) {
      this.progression = progression;
      this.weight = weight;
      this.callback = callback;

      progression.AsyncProgressUpdated += new EventHandler<ProgressUpdateEventArgs>(
        asyncProgressUpdated
      );
    }

    /// <summary>Disposes of the resources used by this instance immediately</summary>
    public void Dispose() {
      if(this.progression != null) {
        progression.AsyncProgressUpdated -= new EventHandler<ProgressUpdateEventArgs>(
          asyncProgressUpdated
        );
        this.progression = null;
      }

    }

    /// <summary>Progression being wrapped by this weighted progression</summary>
    public ProgressionType Progression {
      get { return this.progression; }
    }

    /// <summary>Progress this progression has achieved so far</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>The weighting of this progression in the total progress</summary>
    public float Weight {
      get { return this.weight; }
    }

    /// <summary>Handles progress reports by the progression</summary>
    /// <param name="sender">Progression that has made progress</param>
    /// <param name="e">Contains the currently achieved progress</param>
    private void asyncProgressUpdated(object sender, ProgressUpdateEventArgs e) {
      this.progress = e.Progress;
      this.callback(sender, e);
    }

    /// <summary>Progression whose progress we're tracking</summary>
    private ProgressionType progression;
    /// <summary>Callback to which any progress reports will be passed on</summary>
    private EventHandler<ProgressUpdateEventArgs> callback;
    /// <summary>Most recent progress reported by the progression</summary>
    private volatile float progress;
    /// <summary>Weighting of this progression in the total progress</summary>
    private float weight;

  }

} // namespace Nuclex.Support.Tracking
