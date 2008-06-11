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
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Forms a single waitable from a group of waitables</summary>
  /// <typeparam name="WaitableType">Type of waitables to manage as a set</typeparam>
  public class WaitableGroup<WaitableType> : Waitable, IDisposable, IProgressReporter
    where WaitableType : Waitable {

    /// <summary>will be triggered to report when progress has been achieved</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new waitable group</summary>
    /// <param name="childs">Waitables to track with this group</param>
    /// <remarks>
    ///   Uses a default weighting factor of 1.0 for all waitables.
    /// </remarks>
    public WaitableGroup(IEnumerable<WaitableType> childs)
      : this() {

      // Construct a WeightedWaitable with the default weight for each
      // waitable and wrap it in an ObservedWaitable
      foreach(WaitableType waitable in childs) {
        this.children.Add(
          new ObservedWeightedWaitable<WaitableType>(
            new WeightedWaitable<WaitableType>(waitable),
            new ObservedWeightedWaitable<WaitableType>.ReportDelegate(asyncProgressUpdated),
            new ObservedWeightedWaitable<WaitableType>.ReportDelegate(asyncChildEnded)
          )
        );
      }

      // Since all waitables have a weight of 1.0, the total weight is
      // equal to the number of waitables in our list
      this.totalWeight = (float)this.children.Count;

    }

    /// <summary>Initializes a new waitable group</summary>
    /// <param name="childs">Waitables to track with this group</param>
    public WaitableGroup(
      IEnumerable<WeightedWaitable<WaitableType>> childs
    )
      : this() {

      // Construct an ObservedWaitable around each of the WeightedWaitables
      foreach(WeightedWaitable<WaitableType> waitable in childs) {
        this.children.Add(
          new ObservedWeightedWaitable<WaitableType>(
            waitable,
            new ObservedWeightedWaitable<WaitableType>.ReportDelegate(asyncProgressUpdated),
            new ObservedWeightedWaitable<WaitableType>.ReportDelegate(asyncChildEnded)
          )
        );

        // Sum up the total weight
        this.totalWeight += waitable.Weight;
      }

    }

    /// <summary>Performs common initialization for the public constructors</summary>
    private WaitableGroup() {
      this.children = new List<ObservedWeightedWaitable<WaitableType>>();
    }

    /// <summary>Immediately releases all resources owned by the object</summary>
    public void Dispose() {

      if(this.children != null) {

        // Dispose all the observed waitables, disconnecting the events from the
        // actual waitables so the GC can more easily collect this class
        for(int index = 0; index < this.children.Count; ++index)
          this.children[index].Dispose();

        this.children = null;
        this.wrapper = null;

      }

    }

    /// <summary>Childs contained in the waitable set</summary>
    public IList<WeightedWaitable<WaitableType>> Children {
      get {

        // The wrapper is constructed only when needed. Most of the time, users will
        // just create a waitable group and monitor its progress without ever using
        // the Childs collection.
        if(this.wrapper == null) {

          // This doesn't need a lock because it's a stateless wrapper.
          // If it is constructed twice, then so be it, no problem at all.
          this.wrapper = new WeightedWaitableWrapperCollection<WaitableType>(
            this.children
          );

        }

        return this.wrapper;

      }
    }

    /// <summary>Fires the progress update event</summary>
    /// <param name="progress">Progress to report (ranging from 0.0 to 1.0)</param>
    /// <remarks>
    ///   Informs the observers of this waitables about the achieved progress.
    /// </remarks>
    protected virtual void OnAsyncProgressChanged(float progress) {
      OnAsyncProgressChanged(new ProgressReportEventArgs(progress));
    }

    /// <summary>Fires the progress update event</summary>
    /// <param name="eventArguments">Progress to report (ranging from 0.0 to 1.0)</param>
    /// <remarks>
    ///   Informs the observers of this waitable about the achieved progress.
    ///   Allows for classes derived from the Waitable class to easily provide
    ///   a custom event arguments class that has been derived from the
    ///   waitable's ProgressUpdateEventArgs class.
    /// </remarks>
    protected virtual void OnAsyncProgressChanged(ProgressReportEventArgs eventArguments) {
      EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
      if(copy != null)
        copy(this, eventArguments);
    }

    /// <summary>
    ///   Called when the progress of one of the observed waitables changes
    /// </summary>
    private void asyncProgressUpdated() {
      float totalProgress = 0.0f;

      // Calculate the sum of the progress reported by our child waitables,
      // scaled to the weight each waitable has assigned to it.
      for(int index = 0; index < this.children.Count; ++index) {
        totalProgress +=
          this.children[index].Progress * this.children[index].WeightedWaitable.Weight;
      }

      // Calculate the actual combined progress
      if(this.totalWeight > 0.0f)
        totalProgress /= this.totalWeight;

      // Send out the progress update
      OnAsyncProgressChanged(totalProgress);
    }

    /// <summary>
    ///   Called when an observed waitable ends
    /// </summary>
    private void asyncChildEnded() {

      // If there's still at least one waitable going, don't report that
      // the waitable group has finished yet.
      for(int index = 0; index < this.children.Count; ++index)
        if(!this.children[index].WeightedWaitable.Waitable.Ended)
          return;

      // All child waitables have ended, so the set has now ended as well
      OnAsyncEnded();

    }

    /// <summary>Waitables being managed in the set</summary>
    private List<ObservedWeightedWaitable<WaitableType>> children;
    /// <summary>
    ///   Wrapper collection for exposing the child waitables under the
    ///   WeightedWaitable interface
    /// </summary>
    private volatile WeightedWaitableWrapperCollection<WaitableType> wrapper;
    /// <summary>Summed weight of all waitables in the set</summary>
    private float totalWeight;

  }

} // namespace Nuclex.Support.Tracking
