using System;
using System.Collections.Generic;

namespace Nuclex.Support.Tracking {

  /// <summary>Extended type of progression that is able to fail</summary>
  /// <remarks>
  ///   <para>
  ///     If the background process fails, the exception that caused it to fail is
  ///     communicated to all parties waiting on the progression through the
  ///     Exception property. Implementers should place their code in try..catch
  ///     blocks and call SetException() to temporarily store the exception for
  ///     retrieval by the caller(s).
  ///   </para>
  ///   <para>
  ///     As with all progressions, the interface contract still requires you to call
  ///     OnAsyncEnded(), no matter what the outcome of your background operation is.
  ///   </para>
  /// </remarks>
  public class FailableProgression : Progression {

    #region class EndedDummyProgression

    /// <summary>Dummy progression that is always in the ended state</summary>
    private class EndedDummyProgression : FailableProgression {
      /// <summary>Creates a new successfully completed dummy progression</summary>
      public EndedDummyProgression() : this(null) { }
      /// <summary>Creates a new failed dummy progression</summary>
      /// <param name="exception">Exception that caused the dummy to fail</param>
      public EndedDummyProgression(Exception exception) {
        OnAsyncEnded();

        // Only call SetException() if an actual exception was provided. Who knows what
        // evil code might be inside SetException() after all ;)
        if(exception != null)
          SetException(exception);
      }
    }

    #endregion // EndedDummyProgression

    /// <summary>Creates a new failed dummy progression</summary>
    /// <param name="error">
    ///   Exception that supposedly caused the progression to fail
    /// </param>
    /// <returns>
    ///   A failed progression that reports the provided exception as cause for its failure
    /// </returns>
    public static FailableProgression CreateFailedDummyProgression(Exception exception) {
      return new EndedDummyProgression(exception);
    }

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call End() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public virtual void Join() {

      // By design, end can only be called once!
      lock(this) {
        if(this.endCalled)
          throw new InvalidOperationException("End() has already been called");

        this.endCalled = true;
      }

      // If the progression itself hasn't ended yet, block the caller until it has.
      if(!Ended)
        WaitHandle.WaitOne();

      // If an exception occured during the background execution
      if(this.occuredException != null)
        throw this.occuredException;

    }

    /// <summary>Exception that occured while the operation was executing</summary>
    /// <remarks>
    ///   If this field is null, it is assumed that no exception has occured
    ///   in the background process. If it is set, however, the End() method will
    ///   re-raise the exception to the calling thread when it is called.
    /// </remarks>
    public Exception OccuredException {
      get { return this.occuredException; }
    }

    /// <summary>Sets the exception to raise to the caller of the End() method</summary>
    /// <param name="exception">Exception to raise to the caller of the End() method</param>
    protected void SetException(Exception exception) {

      // We allow the caller to set the exception multiple times. While I certainly
      // can't think of a scenario where this would happen, throwing an exception
      // in that case seems worse. The caller might just be executing an exception
      // handling block and locking + throwing here could cause all kinds of problems.
      this.occuredException = exception;

    }

    /// <summary>Exception that occured while the operation was executing</summary>
    private volatile Exception occuredException;
    /// <summary>Whether the End() method has been called already</summary>
    private volatile bool endCalled;

  }

} // namespace Nuclex.Support.Tracking
