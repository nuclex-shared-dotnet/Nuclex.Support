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
using System.Threading;

namespace Nuclex.Support.Tracking {

  /// <summary>
  ///   Helps tracking the progress of one or more waitable background processes
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is useful if you want to display a progress bar for multiple
  ///     Waitables but can not guarantee that no additional Waitables
  ///     will appear inmidst of execution.
  ///   </para>
  ///   <para>
  ///     This class does not implement the <see cref="Waitable" /> interface itself
  ///     in order to not violate the design principles of Waitables which
  ///     guarantee that a <see cref="Waitable" /> will only finish once (whereas the
  ///     progress tracker might 'finish' any number of times).
  ///   </para>
  /// </remarks>
  public class ProgressTracker : IDisposable, IProgressReporter {

    #region class WaitableMatcher

    /// <summary>Matches a direct Waitable to a fully wrapped one</summary>
    private class WaitableMatcher {

      /// <summary>
      ///   Initializes a new Waitable matcher that matches against
      ///   the specified Waitable
      /// </summary>
      /// <param name="toMatch">Waitable to match against</param>
      public WaitableMatcher(Waitable toMatch) {
        this.toMatch = toMatch;
      }

      /// <summary>
      ///   Checks whether the provided Waitable matches the comparison
      ///   Waitable of the instance
      /// </summary>
      /// <param name="other">Waitable to match to the comparison Waitable</param>
      public bool Matches(ObservedWeightedWaitable<Waitable> other) {
        return ReferenceEquals(other.WeightedWaitable.Waitable, this.toMatch);
      }

      /// <summary>Waitable this instance compares against</summary>
      private Waitable toMatch;

    }

    #endregion // class WaitableMatcher

    /// <summary>Triggered when the idle state of the tracker changes</summary>
    /// <remarks>
    ///   The tracker is idle when no Waitables are being tracked in it. If you're
    ///   using this class to feed a progress bar, this would be the event to use for
    ///   showing or hiding the progress bar. The tracker starts off as idle because,
    ///   upon construction, its list of Waitables will be empty.
    /// </remarks>
    public event EventHandler<IdleStateEventArgs> AsyncIdleStateChanged;

    /// <summary>Triggered when the total progress has changed</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new Waitable tracker</summary>
    public ProgressTracker() {

      this.trackedWaitables = new List<ObservedWeightedWaitable<Waitable>>();
      this.idle = true;

      this.asyncEndedDelegate =
        new ObservedWeightedWaitable<Waitable>.ReportDelegate(asyncEnded);
      this.asyncProgressUpdatedDelegate =
        new ObservedWeightedWaitable<Waitable>.ReportDelegate(asyncProgressChanged);

    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      lock(this.trackedWaitables) {

        // Get rid of all Waitables we're tracking. This unsubscribes the
        // observers from the events of the Waitables and stops us from
        // being kept alive and receiving any further events if some of the
        // tracked Waitables are still executing.
        for(int index = 0; index < this.trackedWaitables.Count; ++index)
          this.trackedWaitables[index].Dispose();

        // Help the GC a bit by untangling the references :)
        this.trackedWaitables.Clear();
        this.trackedWaitables = null;

      } // lock
    }

    /// <summary>Begins tracking the specified waitable background process</summary>
    /// <param name="waitable">Waitable background process to be tracked</param>
    public void Track(Waitable waitable) {
      Track(waitable, 1.0f);
    }

    /// <summary>Begins tracking the specified waitable background process</summary>
    /// <param name="waitable">Waitable background process to be tracked</param>
    /// <param name="weight">Weight to assign to this waitable background process</param>
    public void Track(Waitable waitable, float weight) {

      // Add the new Waitable into the tracking list. This has to be done
      // inside a lock to prevent issues with the progressUpdate callback, which could
      // access the totalWeight field before it has been updated to reflect the
      // new Waitable added to the collection.
      lock(this.trackedWaitables) {

        bool wasEmpty = (this.trackedWaitables.Count == 0);

        // This can be done after we registered the wrapper to our delegates because
        // any incoming progress updates will be stopped from the danger of a
        // division-by-zero from the potentially still zeroed totalWeight by the lock.
        this.totalWeight += weight;

        if(waitable.Ended) {

          // If the ended Waitable would become the only Waitable in the list,
          // there's no sense in doing anything at all because it would have to be
          // thrown right out again. Only add the Waitable when there are other
          // running Waitables to properly sum total progress for consistency.
          if(!wasEmpty) {

            // Construct a new observation wrapper. This is done inside the lock
            // because as soon as we are subscribed to the events, we can potentially
            // receive them. The lock eliminates the risk of processing a progress update
            // before the Waitable has been added to the tracked Waitables list.
            this.trackedWaitables.Add(
              new ObservedWeightedWaitable<Waitable>(
                new WeightedWaitable<Waitable>(waitable, weight),
                this.asyncProgressUpdatedDelegate,
                this.asyncEndedDelegate
              )
            );

            // All done, the total progress is different now, so force a recalculation and
            // send out the AsyncProgressUpdated event.
            recalculateProgress();

          }

        } else { // Not ended -- Waitable is still running

          // Construct a new Waitable observer and add the Waitable to our
          // list of tracked Waitables.
          ObservedWeightedWaitable<Waitable> observedWaitable =
            new ObservedWeightedWaitable<Waitable>(
              new WeightedWaitable<Waitable>(waitable, weight),
              this.asyncProgressUpdatedDelegate,
              this.asyncEndedDelegate
            );

          this.trackedWaitables.Add(observedWaitable);

          // If this is the first Waitable to be added to the list, tell our
          // owner that we're idle no longer!
          if(wasEmpty)
            setIdle(false);

          // All done, the total progress is different now, so force a recalculation and
          // send out the AsyncProgressUpdated event.
          recalculateProgress();

          // The Waitable might have ended before we had registered to its AsyncEnded
          // event, so we have to do this to be on the safe side. This might cause
          // asyncEnded() to be called twice, but that's not a problem in this
          // implementation and improves performance and simplicity for the normal path.
          if(waitable.Ended) {
            asyncEnded();
            observedWaitable.Dispose();
          }

        } // if Waitable ended

      } // lock

    }

    /// <summary>Stops tracking the specified waitable background process</summary>
    /// <param name="waitable">Waitable background process to stop tracking of</param>
    public void Untrack(Waitable waitable) {
      lock(this.trackedWaitables) {

        // Locate the object to be untracked in our collection
        int removeIndex = this.trackedWaitables.FindIndex(
          new Predicate<ObservedWeightedWaitable<Waitable>>(
            new WaitableMatcher(waitable).Matches
          )
        );
        if(removeIndex == -1)
          throw new InvalidOperationException("Item is not being tracked");

        // Remove and dispose the Waitable the user wants to untrack
        {
          ObservedWeightedWaitable<Waitable> wrappedWaitable =
            this.trackedWaitables[removeIndex];

          this.trackedWaitables.RemoveAt(removeIndex);
          wrappedWaitable.Dispose();
        }

        // If the list is empty, then we're back in the idle state
        if(this.trackedWaitables.Count == 0) {

          this.totalWeight = 0.0f;

          // If we entered the idle state with this call, report the state change!
          setIdle(true);

        } else {

          // Rebuild the total weight from scratch. Subtracting the removed Waitable's
          // weight would work, too, but we might accumulate rounding errors making the sum
          // drift slowly away from the actual value.
          this.totalWeight = 0.0f;
          for(int index = 0; index < this.trackedWaitables.Count; ++index)
            this.totalWeight += this.trackedWaitables[index].WeightedWaitable.Weight;

        }

      } // lock
    }

    /// <summary>Whether the tracker is currently idle</summary>
    public bool Idle {
      get { return this.idle; }
    }

    /// <summary>Current summed progress of the tracked Waitables</summary>
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
      EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
      if(copy != null)
        copy(this, new ProgressReportEventArgs(progress));
    }

    /// <summary>Recalculates the total progress of the tracker</summary>
    private void recalculateProgress() {
      float totalProgress = 0.0f;

      // Lock the collection to avoid trouble when someone tries to remove one
      // of our tracked Waitables while we're just doing a progress update
      lock(this.trackedWaitables) {

        // This is a safety measure. In theory, even after all Waitables have
        // ended and the collection of tracked Waitables is cleared, a waiting
        // thread might deliver another progress update causing this method to
        // be entered. In this case, the right thing is to do nothing at all.
        if(this.totalWeight == 0.0f)
          return;

        // Sum up the total progress
        for(int index = 0; index < this.trackedWaitables.Count; ++index) {
          float weight = this.trackedWaitables[index].WeightedWaitable.Weight;
          totalProgress += this.trackedWaitables[index].Progress * weight;
        }

        // This also needs to be in the lock to guarantee that the totalWeight is
        // the one for the number of Waitables we just summed -- by design,
        // the total weight always has to be updated at the same time as the collection.
        totalProgress /= this.totalWeight;

        // Finally, trigger the event
        this.progress = totalProgress;
        OnAsyncProgressUpdated(totalProgress);

      } // lock
    }

    /// <summary>Called when one of the tracked Waitables has ended</summary>
    private void asyncEnded() {
      lock(this.trackedWaitables) {

        // If any Waitables in the list are still going, keep the entire list.
        // This behavior is intentional in order to prevent the tracker's progress from
        // jumping back repeatedly when multiple tracked Waitables come to an end.
        for(int index = 0; index < this.trackedWaitables.Count; ++index)
          if(!this.trackedWaitables[index].WeightedWaitable.Waitable.Ended)
            return;

        // All Waitables have finished, get rid of the wrappers and make a
        // fresh start for future Waitables to be tracked. No need to call
        // Dispose() since, as a matter of fact, when the Waitable
        this.trackedWaitables.Clear();
        this.totalWeight = 0.0f;

        // Notify our owner that we're idle now. This line is only reached when all
        // Waitables were finished, so it's safe to trigger this here.
        setIdle(true);

      } // lock
    }

    /// <summary>Called when one of the tracked Waitables has achieved progress</summary>
    private void asyncProgressChanged() {
      recalculateProgress();
    }

    /// <summary>Changes the idle state</summary>
    /// <param name="idle">Whether or not the tracker is currently idle</param>
    /// <remarks>
    ///   This method expects to be called during a lock() on trackedWaitables!
    /// </remarks>
    private void setIdle(bool idle) {
      this.idle = idle;

      OnAsyncIdleStateChanged(idle);
    }

    /// <summary>Whether the tracker is currently idle</summary>
    private volatile bool idle;
    /// <summary>Current summed progress of the tracked Waitables</summary>
    private volatile float progress;
    /// <summary>Total weight of all Waitables being tracked</summary>
    private volatile float totalWeight;
    /// <summary>Waitables being tracked by this tracker</summary>
    private List<ObservedWeightedWaitable<Waitable>> trackedWaitables;
    /// <summary>Delegate for the asyncEnded() method</summary>
    private ObservedWeightedWaitable<Waitable>.ReportDelegate asyncEndedDelegate;
    /// <summary>Delegate for the asyncProgressUpdated() method</summary>
    private ObservedWeightedWaitable<Waitable>.ReportDelegate asyncProgressUpdatedDelegate;

  }

} // namespace Nuclex.Support.Tracking
