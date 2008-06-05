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
using System.Threading;

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for background processes the user can wait on</summary>
  /// <remarks>
  ///   <para>
  ///     By encapsulating long-running operations which will ideally be running in
  ///     a background thread in a class that's derived from <see cref="Waitable" />
  ///     you can wait for the completion of the operation and optionally even receive
  ///     feedback on the achieved progress. This is useful for displaying a progress
  ///     bar, loading screen or some other means of entertaining the user while he
  ///     waits for the task to complete.
  ///   </para>
  ///   <para>
  ///     You can register callbacks which will be fired once the <see cref="Waitable" />
  ///     task has completed. This class deliberately does not provide an Execute()
  ///     method or anything similar to clearly seperate the initiation of an operation
  ///     from just monitoring it. By omitting an Execute() method, it also becomes
  ///     possible to construct a Waitable just-in-time when it is explicitely being
  ///     asked for.
  ///   </para>
  /// </remarks>
  public abstract class Waitable {

    #region class EndedDummyWaitable

    /// <summary>Dummy waitable which always is in the 'ended' state</summary>
    private class EndedDummyWaitable : Waitable {

      /// <summary>Initializes a new ended dummy waitable</summary>
      public EndedDummyWaitable() {
        OnAsyncEnded();
      }

    }

    #endregion // class EndedDummyWaitable

    /// <summary>A dummy waitable that's always in the 'ended' state</summary>
    /// <remarks>
    ///   Useful if an operation is already complete when it's being asked for or
    ///   when a progression that's lazily created is accessed after the original
    ///   operation has ended already.
    /// </remarks>
    public static readonly Waitable EndedDummy = new EndedDummyWaitable();

    /// <summary>Will be triggered when the Waitable has ended</summary>
    public event EventHandler AsyncEnded;

    /// <summary>Whether the Waitable has ended already</summary>
    public bool Ended {
      get { return this.ended; }
    }

    /// <summary>WaitHandle that can be used to wait for the Waitable to end</summary>
    public WaitHandle WaitHandle {
      get {

        // The WaitHandle will only be created when someone asks for it!
        // See the Double-Check Locking idiom on why the condition is checked twice
        // (primarily, it avoids an expensive lock when it isn't needed)
        //
        // We can *not* optimize this lock away since we absolutely must not create
        // two doneEvents -- someone might call .WaitOne() on the first one when only
        // the second one is referenced by this.doneEvent and thus gets set in the end.
        if(this.doneEvent == null) {

          lock(this) {

            if(this.doneEvent == null)
              this.doneEvent = new ManualResetEvent(this.ended);

          }

        }

        return this.doneEvent;
      }
    }

    /// <summary>Fires the AsyncEnded event</summary>
    /// <remarks>
    ///   <para>
    ///     This event should be fired by the implementing class when its work is completed.
    ///     It's of no interest to this class whether the outcome of the process was
    ///     successfull or not, the outcome and results of the process taking place both
    ///     need to be communicated seperately.
    ///   </para>
    ///   <para>
    ///     Calling this method is mandatory. Implementers need to take care that
    ///     the OnAsyncEnded() method is called on any instance of Progression that's
    ///     being created. This method also must not be called more than once.
    ///   </para>
    /// </remarks>
    protected virtual void OnAsyncEnded() {

      // Make sure the progression is not ended more than once. By guaranteeing that
      // a progression can only be ended once, we allow users of this class to
      // skip some safeguards against notifications arriving twice.
      lock(this) {

        // No double lock here, this is an exception that indicates an implementation
        // error that will not be triggered under normal circumstances. We don't want
        // to waste any effort optimizing the speed at which an implementation fault
        // will be noticed.
        if(this.ended)
          throw new InvalidOperationException("The progression has already been ended");

        this.ended = true;

      }

      // Doesn't need a lock. If another thread wins the race and creates the event
      // after we just saw it being null, it would be created in an already set
      // state due to the ended flag (see above) being set to true beforehand!
      if(this.doneEvent != null)
        this.doneEvent.Set();

      // Finally, fire the AsyncEnded event
      EventHandler copy = AsyncEnded;
      if(copy != null)
        copy(this, EventArgs.Empty);

    }

    /// <summary>Event that will be set when the progression is completed</summary>
    /// <remarks>
    ///   This event is will only be created when it is specifically asked for using
    ///   the WaitHandle property.
    /// </remarks>
    private volatile ManualResetEvent doneEvent;
    /// <summary>Whether the operation has completed yet</summary>
    private volatile bool ended;

  }

} // namespace Nuclex.Support.Tracking
