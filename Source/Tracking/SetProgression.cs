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
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Forms a single progression from a set of progressions</summary>
  /// <typeparam name="ProgressionType">Type of progressions to manage as a set</typeparam>
  public class SetProgression<ProgressionType> : Progression, IDisposable
    where ProgressionType : Progression {

    /// <summary>Performs common initialization for the public constructors</summary>
    private SetProgression() {
      this.childs = new List<ObservedProgression<ProgressionType>>();
    }

    /// <summary>Initializes a new set progression</summary>
    /// <param name="childs">Progressions to track with this set</param>
    /// <remarks>
    ///   Uses a default weighting factor of 1.0 for all progressions.
    /// </remarks>
    public SetProgression(IEnumerable<ProgressionType> childs)
      : this() {

      // Construct a WeightedProgression with the default weight for each
      // progression and wrap it in an ObservedProgression
      foreach(ProgressionType progression in childs) {
        this.childs.Add(
          new ObservedProgression<ProgressionType>(
            new WeightedProgression<ProgressionType>(progression),
            new ObservedProgression<ProgressionType>.ReportDelegate(asyncProgressUpdated),
            new ObservedProgression<ProgressionType>.ReportDelegate(asyncEnded)
          )
        );
      }

      // Since all progressions have a weight of 1.0, the total weight is
      // equal to the number of progressions in our list
      this.totalWeight = (float)this.childs.Count;

    }

    /// <summary>Initializes a new set progression</summary>
    /// <param name="childs">Progressions to track with this set</param>
    public SetProgression(
      IEnumerable<WeightedProgression<ProgressionType>> childs
    )
      : this() {

      // Construct an ObservedProgression around each of the WeightedProgressions
      float totalWeight = 0.0f;
      foreach(WeightedProgression<ProgressionType> progression in childs) {
        this.childs.Add(
          new ObservedProgression<ProgressionType>(
            progression,
            new ObservedProgression<ProgressionType>.ReportDelegate(asyncProgressUpdated),
            new ObservedProgression<ProgressionType>.ReportDelegate(asyncEnded)
          )
        );

        // Sum up the total weight
        totalWeight += progression.Weight;
      }

      // Take over the summed weight of all progressions we were given
      this.totalWeight = totalWeight;

    }

    /// <summary>Immediately releases all resources owned by the object</summary>
    public void Dispose() {

      if(this.childs != null) {

        // Dispose all the observed progressions, disconnecting the events from the
        // actual progressions so the GC can more easily collect this class
        for(int index = 0; index < this.childs.Count; ++index)
          this.childs[index].Dispose();

        this.childs = null;
        this.wrapper = null;

      }

    }

    /// <summary>Childs contained in the progression set</summary>
    public IList<WeightedProgression<ProgressionType>> Childs {
      get {

        // The wrapper is constructed only when needed. Most of the time, users will
        // just create a SetProgression and monitor its progress without ever using
        // the Childs collection.
        if(this.wrapper == null) {

          // This doesn't need a lock because it's only a stateless wrapper. If it
          // is constructed twice, then so be it.
          this.wrapper = new WeightedProgressionWrapperCollection<ProgressionType>(
            this.childs
          );

        }

        return this.wrapper;

      }
    }

    /// <summary>
    ///   Called when the progress of one of the observed progressions changes
    /// </summary>
    private void asyncProgressUpdated() {

      // Calculate the sum of the progress reported by our child progressions,
      // scaled to the weight each progression has assigned to it.
      float totalProgress = 0.0f;
      for(int index = 0; index < this.childs.Count; ++index) {
        totalProgress +=
          this.childs[index].Progress * this.childs[index].WeightedProgression.Weight;
      }

      // Calculate the actual combined progress
      if(this.totalWeight > 0.0f)
        totalProgress /= this.totalWeight;

      // Send out the progress update
      OnAsyncProgressUpdated(totalProgress);

    }

    /// <summary>
    ///   Called when an observed progressions ends
    /// </summary>
    private void asyncEnded() {

      // If there's still at least one progression going, don't report that
      // the SetProgression has finished yet.
      for(int index = 0; index < this.childs.Count; ++index)
        if(!this.childs[index].WeightedProgression.Progression.Ended)
          return;

      // All child progressions have ended, so the set has now ended as well
      OnAsyncEnded();

    }

    /// <summary>Progressions being managed in the set</summary>
    private List<ObservedProgression<ProgressionType>> childs;
    /// <summary>
    ///   Wrapper collection for exposing the child progressions under the
    ///   WeightedProgression interface
    /// </summary>
    private volatile WeightedProgressionWrapperCollection<ProgressionType> wrapper;
    /// <summary>Summed weight of all progression in the set</summary>
    private float totalWeight;

  }

} // namespace Nuclex.Support.Tracking
