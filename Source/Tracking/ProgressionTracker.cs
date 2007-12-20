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
using System.Threading;

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
  ///     progression tracker might 'finish' any number of times).
  ///   </para>
  /// </remarks>
  public class ProgressionTracker : IDisposable {

    #region class ProgressionMatcher

    /// <summary>Matches a progression to a fully wrapped one</summary>
    private class ProgressionMatcher {

      /// <summary>
      ///   Initializes a new progression matcher that matches against
      ///   the specified progression
      /// </summary>
      /// <param name="toMatch">Progression to match against</param>
      public ProgressionMatcher(Progression toMatch) {
        this.toMatch = toMatch;
      }

      /// <summary>
      ///   Checks whether the provided progression matches the comparison
      ///   progression of the instance
      /// </summary>
      /// <param name="other">Progression to match to the comparison progression</param>
      public bool Matches(ObservedWeightedProgression<Progression> other) {
        return ReferenceEquals(other.WeightedProgression.Progression, this.toMatch);
      }

      /// <summary>Progression this instance compares against</summary>
      private Progression toMatch;

    }

    #endregion // class ProgressionMatcher

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

    /// <summary>Initializes a new progression tracker</summary>
    public ProgressionTracker() {

      this.trackedProgressions = new List<ObservedWeightedProgression<Progression>>();
      this.idle = true;

      this.asyncEndedDelegate =
        new ObservedWeightedProgression<Progression>.ReportDelegate(asyncEnded);
      this.asyncProgressUpdatedDelegate =
        new ObservedWeightedProgression<Progression>.ReportDelegate(asyncProgressUpdated);

    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      lock(this.trackedProgressions) {

        // Get rid of all progression we're tracking. This unsubscribes the
        // observers from the events of the progressions and stops us from
        // being kept alive and receiving any further events if some of the
        // tracked progressions are still executing.
        for(int index = 0; index < this.trackedProgressions.Count; ++index)
          this.trackedProgressions[index].Dispose();

        // Help the GC a bit by untangling the references :)
        this.trackedProgressions.Clear();
        this.trackedProgressions = null;

      } // lock
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

      // Add the new progression into the tracking list. This has to be done
      // inside a lock to prevent issues with the progressUpdate callback, which could
      // access the totalWeight field before it has been updated to reflect the
      // new progression added to the collection.
      lock(this.trackedProgressions) {

        bool wasEmpty = (this.trackedProgressions.Count == 0);

        // This can be done after we registered the wrapper to our delegates because
        // any incoming progress updates will be stopped from the danger of a
        // division-by-zero from the potentially still zeroed totalWeight by the lock.
        this.totalWeight += weight;

        if(progression.Ended) {

          // If the ended progression would become the only progression in the list,
          // there's no sense in doing anything at all because it would have to be
          // thrown right out again. Only add the progression when there are other
          // running progressions to properly sum total progress for consistency.
          if(!wasEmpty) {

            // Construct a new observation wrapper. This is done inside the lock
            // because as soon as we are subscribed to the events, we can potentially
            // receive them. The lock eliminates the risk of processing a progress update
            // before the progression has been added to the tracked progressions list.
            this.trackedProgressions.Add(
              new ObservedWeightedProgression<Progression>(
                new WeightedProgression<Progression>(progression, weight),
                this.asyncProgressUpdatedDelegate,
                this.asyncEndedDelegate
              )
            );

            // All done, the total progress is different now, so force a recalculation and
            // send out the AsyncProgressUpdated event.
            recalculateProgress();

          }

        } else { // Not ended -- Progression is still running

          // Construct a new progression observer and add the progression to our
          // list of tracked progressions.
          ObservedWeightedProgression<Progression> observedProgression =
            new ObservedWeightedProgression<Progression>(
              new WeightedProgression<Progression>(progression, weight),
              this.asyncProgressUpdatedDelegate,
              this.asyncEndedDelegate
            );

          this.trackedProgressions.Add(observedProgression);

          // If this is the first progression to be added to the list, tell our
          // owner that we're idle no longer!
          if(wasEmpty)
            setIdle(false);

          // All done, the total progress is different now, so force a recalculation and
          // send out the AsyncProgressUpdated event.
          recalculateProgress();

          // The progression might have ended before we had registered to its AsyncEnded
          // event, so we have to do this to be on the safe side. This might cause
          // asyncEnded() to be called twice, but that's not a problem in this
          // implementation and improves performance and simplicity for the normal path.
          if(progression.Ended) {
            asyncEnded();
            observedProgression.Dispose();
          }

        } // if progression ended

      } // lock

    }

    /// <summary>Stops tracking the specified progression</summary>
    /// <param name="progression">Progression to stop tracking of</param>
    public void Untrack(Progression progression) {
      lock(this.trackedProgressions) {

        // Locate the object to be untracked in our collection
        int removeIndex = this.trackedProgressions.FindIndex(
          new Predicate<ObservedWeightedProgression<Progression>>(
            new ProgressionMatcher(progression).Matches
          )
        );
        if(removeIndex == -1)
          throw new InvalidOperationException("Item is not being tracked");

        // Remove and dispose the progression the user wants to untrack
        {
          ObservedWeightedProgression<Progression> wrappedProgression =
            this.trackedProgressions[removeIndex];

          this.trackedProgressions.RemoveAt(removeIndex);
          wrappedProgression.Dispose();
        }

        // If the list is empty, then we're back in the idle state
        if(this.trackedProgressions.Count == 0) {

          this.totalWeight = 0.0f;

          // If we entered the idle state with this call, report the state change!
          setIdle(true);

        } else {

          // Rebuild the total weight from scratch. Subtracting the removed progression's
          // weight would work, too, but we might accumulate rounding errors making the sum
          // drift slowly away from the actual value.
          this.totalWeight = 0.0f;
          for(int index = 0; index < this.trackedProgressions.Count; ++index)
            this.totalWeight += this.trackedProgressions[index].WeightedProgression.Weight;

        }

      } // lock
    }

    /// <summary>Whether the tracker is currently idle</summary>
    public bool Idle {
      get { return this.idle; }
    }

    /// <summary>Current summed progress of the tracked progressions</summary>
    public float Progress {
      get { return this.progress; }
    }

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
      float totalProgress = 0.0f;

      // Lock the collection to avoid trouble when someone tries to remove one
      // of our tracked progressions while we're just doing a progress update
      lock(this.trackedProgressions) {

        // This is a safety measure. In theory, even after all progressions have
        // ended and the collection of tracked progressions is cleared, a waiting
        // thread might deliver another progress update causing this method to
        // be entered. In this case, the right thing is to do nothing at all.
        if(this.totalWeight == 0.0f)
          return;

        // Sum up the total progress
        for(int index = 0; index < this.trackedProgressions.Count; ++index) {
          float weight = this.trackedProgressions[index].WeightedProgression.Weight;
          totalProgress += this.trackedProgressions[index].Progress * weight;
        }

        // This also needs to be in the lock to guarantee that the totalWeight is
        // the one for the number of progressions we just summed -- by design,
        // the total weight always has to be updated at the same time as the collection.
        totalProgress /= this.totalWeight;

        // Finally, trigger the event
        this.progress = totalProgress;
        OnAsyncProgressUpdated(totalProgress);

      } // lock
    }

    /// <summary>Called when one of the tracked progressions has ended</summary>
    private void asyncEnded() {
      lock(this.trackedProgressions) {

        // If any progressions in the list are still going, keep the entire list.
        // This behavior is intentional in order to prevent the tracker's progress from
        // jumping back repeatedly when multiple tracked progressions come to an end.
        for(int index = 0; index < this.trackedProgressions.Count; ++index)
          if(!this.trackedProgressions[index].WeightedProgression.Progression.Ended)
            return;

        // All progressions have finished, get rid of the wrappers and make a
        // fresh start for future progressions to be tracked. No need to call
        // Dispose() since, as a matter of fact, when the progression
        this.trackedProgressions.Clear();
        this.totalWeight = 0.0f;

        // Notify our owner that we're idle now. This line is only reached when all
        // progressions were finished, so it's safe to trigger this here.
        setIdle(true);

      } // lock
    }

    /// <summary>Called when one of the tracked progression has achieved progress</summary>
    private void asyncProgressUpdated() {
      recalculateProgress();
    }

    /// <summary>Changes the idle state</summary>
    /// <param name="idle">Whether or not the tracker is currently idle</param>
    /// <remarks>
    ///   This method expects to be called during a lock() on trackedProgressions!
    /// </remarks>
    private void setIdle(bool idle) {
      this.idle = idle;

      OnAsyncIdleStateChanged(idle);
    }

    /// <summary>Whether the tracker is currently idle</summary>
    private volatile bool idle;
    /// <summary>Current summed progress of the tracked progressions</summary>
    private volatile float progress;
    /// <summary>Total weight of all progressions being tracked</summary>
    private volatile float totalWeight;
    /// <summary>Progressions being tracked by this tracker</summary>
    private List<ObservedWeightedProgression<Progression>> trackedProgressions;
    /// <summary>Delegate for the asyncEnded() method</summary>
    private ObservedWeightedProgression<Progression>.ReportDelegate asyncEndedDelegate;
    /// <summary>Delegate for the asyncProgressUpdated() method</summary>
    private ObservedWeightedProgression<Progression>.ReportDelegate asyncProgressUpdatedDelegate;

  }

} // namespace Nuclex.Support.Tracking
