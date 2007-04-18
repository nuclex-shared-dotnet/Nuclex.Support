using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Progression being observed by another class</summary>
  /// <typeparam name="ProgressionType">
  ///   Type of the progression that is being observed
  /// </typeparam>
  internal class ObservedProgression<ProgressionType>
    where ProgressionType : Progression {

    /// <summary>Initializes a new observed progression</summary>
    /// <param name="weightedProgression">Weighted progression being observed</param>
    internal ObservedProgression(
      WeightedProgression<ProgressionType> weightedProgression
    ) {
      this.weightedProgression = weightedProgression;
    }
    
    /// <summary>Weighted progression being observed</summary>
    public WeightedProgression<ProgressionType> WeightedProgression {
      get { return this.weightedProgression; }
    }
/*

    internal void AsyncProgressUpdated(object sender, ProgressUpdateEventArgs e) {
      this.Progress = e.Progress;
    }
    internal void AsyncEnded(object sender, EventArgs e) { }
*/
    /// <summary>The weighted progression that is being observed</summary>
    private WeightedProgression<ProgressionType> weightedProgression;
/*
    /// <summary>Amount of progress this progression has achieved so far</summary>
    private volatile float progress;
*/
  }

} // namespace Nuclex.Support.Tracking
