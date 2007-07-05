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
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Helps tracking the progress of one or more progressions</summary>
  /// <remarks>
  ///   <para>
  ///     This is useful if you want to display a progress bar for multiple
  ///     progressions but can not guarantee that no additional progressions
  ///     will appear inmidst of execution.
  ///   </para>
  ///   <para>
  ///     This class does not implement the IProgression interface itself in
  ///     order to not violate the design principles of progressions which
  ///     guarantee that a progression will only finish once (whereas the
  ///     progression tracking might finish any number of times).
  ///   </para>
  /// </remarks>
  public class ProgressionTracker : IDisposable {

    /// <summary>Triggered when the idle state of the tracker changes</summary>
    /// <remarks>
    ///   The tracker is idle when no progressions are being tracked in it. If you're
    ///   using this class to feed a progress bar, this would be the event to use for
    ///   showing or hiding the progress bar. The tracker starts off as idle because,
    ///   upon construction, its list of progressions will be empty.
    /// </remarks>
    public event EventHandler<IdleStateEventArgs> AsyncIdleStateChanged;

    /// <summary>Triggered when the total progress has changed</summary>
    public event EventHandler<ProgressUpdateEventArgs> AsyncProgressUpdated;

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      // TODO: Untrack all
    }

    /// <summary>Begins tracking the specified progression</summary>
    /// <param name="progression">Progression to be tracked</param>
    public void Track(Progression progression) {
      Track(progression, 1.0f);
    }

    /// <summary>Begins tracking the specified progression</summary>
    /// <param name="progression">Progression to be tracked</param>
    /// <param name="weight">Weight to assign to this progression</param>
    public void Track(Progression progression, float weight) {
      this.trackedProgressions.Add(
        new WeightedProgression<Progression>(progression, weight)
      );
      this.totalWeight += weight;

      recalculateProgress();
    }

    /// <summary>Stops tracking the specified progression</summary>
    /// <param name="progression">Progression to stop tracking of</param>
    public void Untrack(Progression progression) { }

    /// <summary>Fires the AsyncIdleStateChanged event</summary>
    /// <param name="idle">New idle state to report</param>
    protected virtual void OnAsyncIdleStateChanged(bool idle) {
      EventHandler<IdleStateEventArgs> copy = AsyncIdleStateChanged;
      if(copy != null)
        copy(this, new IdleStateEventArgs(idle));
    }

    /// <summary>Fires the AsyncProgressUpdated event</summary>
    /// <param name="progress">New progress to report</param>
    protected virtual void OnAsyncProgressUpdated(float progress) {
      EventHandler<ProgressUpdateEventArgs> copy = AsyncProgressUpdated;
      if(copy != null)
        copy(this, new ProgressUpdateEventArgs(progress));
    }

    /// <summary>Recalculates the total progress of the tracker</summary>
    private void recalculateProgress() {
      float totalProgress;

      for(int index = 0; index < trackedProgressions.Count; ++index) {
        float weight = this.trackedProgressions[index].WeightedProgression;
        totalProgress = this.trackedProgressions[index].Progress * weight;
      }

      totalProgress /= this.totalWeight;

      //OnAsyncProgressUpdated(
    }

    /// <summary>Total weight of all progressions being tracked</summary>
    private float totalWeight;
    /// <summary>Progressions being tracked by this tracker</summary>
    private List<ObservedWeightedProgression<ProgressionType>> trackedProgressions;

  }

} // namespace Nuclex.Support.Tracking
