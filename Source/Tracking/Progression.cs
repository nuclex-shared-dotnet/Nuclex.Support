using System;
using System.Threading;

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for actions that give an indication of their progress</summary>
  /// <remarks>
  ///   <para>
  ///     By encapsulating long-running operations which will ideally be running in
  ///     a background thread in a class that's derived from Progression you can wait
  ///     for the completion of the operation and receive feedback on the achieved
  ///     progress. This is useful for displaying a progress bar, loading screen or
  ///     some other means of entertaining the user while he waits for the operation to
  ///     complete. It is also possible to register callbacks which will be fired once
  ///     the progression has ended.
  ///   </para>
  ///   <para>
  ///     This class deliberately does not provide an Execute() method or anything similar
  ///     to clearly seperate the initiation of an operation from just monitoring it.
  ///     By omitting an Execute() method, it also becomes possible to construct a
  ///     progression just-in-time when it is explicitely asked for.
  ///   </para>
  /// </remarks>
  public abstract class Progression {

    #region class EndedDummyProgression

    /// <summary>Dummy progression which always is in the 'ended' state</summary>
    internal class EndedDummyProgression : Progression {

      /// <summary>Initializes a new ended dummy progression</summary>
      public EndedDummyProgression() {
        OnAsyncEnded();
      }

    }

    #endregion // class EndedDummyProgression

    /// <summary>A dummy progression that's always in the 'ended' state</summary>
    /// <remarks>
    ///   Useful if an operation is already complete when it's being asked for or
    ///   when a progression that's lazily created is accessed after the original
    ///   operation has ended already.
    /// </remarks>
    public static readonly Progression EndedDummy = new EndedDummyProgression();

    /// <summary>will be triggered to report when progress has been achieved</summary>
    public event EventHandler<ProgressUpdateEventArgs> AsyncProgressUpdated;

    /// <summary>Will be triggered when the progression has ended</summary>
    public event EventHandler AsyncEnded;

    /// <summary>Whether the progression has ended already</summary>
    public virtual bool Ended {
      get { return this.ended; }
    }

    /// <summary>WaitHandle that can be used to wait for the progression to end</summary>
    public WaitHandle WaitHandle {
      get {
        
        // The WaitHandle will only be created when someone asks for it!
        // See the Double-Check Locking idiom on why the condition is checked twice
        // (primarily, it avoids an expensive lock when it isn't needed)
        if(this.doneEvent == null) {

          lock(this.syncRoot) {

            if(this.doneEvent == null)
              this.doneEvent = new ManualResetEvent(this.ended);

          }

        }

        return this.doneEvent;
      }
    }

    /// <summary>Fires the progress update event</summary>
    /// <param name="progress">Progress to report (ranging from 0.0 to 1.0)</param>
    /// <remarks>
    ///   Informs the observers of this progression about the achieved progress.
    /// </remarks>
    protected virtual void OnAsyncProgressUpdated(float progress) {
      OnAsyncProgressUpdated(new ProgressUpdateEventArgs(progress));
    }

    /// <summary>Fires the progress update event</summary>
    /// <param name="eventArguments">Progress to report (ranging from 0.0 to 1.0)</param>
    /// <remarks>
    ///   Informs the observers of this progression about the achieved progress.
    ///   Allows for classes derived from the Progression class to easily provide
    ///   a custom event arguments class that has been derived from the
    ///   Progression's ProgressUpdateEventArgs class.
    /// </remarks>
    protected virtual void OnAsyncProgressUpdated(ProgressUpdateEventArgs eventArguments) {
      EventHandler<ProgressUpdateEventArgs> copy = AsyncProgressUpdated;
      if(copy != null)
        copy(this, eventArguments);
    }

    /// <summary>Fires the AsyncEnded event</summary>
    /// <remarks>
    ///   This event should be fired by the implementing class when its work is completed.
    ///   It's of no interest to this class whether the outcome of the process was successfull
    ///   or not, the outcome and results of the process taking place need to be communicated
    ///   seperately.
    /// </remarks>
    protected virtual void OnAsyncEnded() {
      this.ended = true;

      lock(this.syncRoot) {
        if(this.doneEvent != null)
          this.doneEvent.Set();
      }

      EventHandler copy = AsyncEnded;
      if(copy != null)
        copy(this, EventArgs.Empty);
    }

    /// <summary>Used to synchronize multithreaded accesses to this object</summary>
    private object syncRoot = new object();
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
