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
  public abstract class Request : Waitable {

    #region class EndedDummyRequest

    /// <summary>Dummy request that is always in the ended state</summary>
    private class EndedDummyRequest : Request {
      /// <summary>Creates a new successfully completed dummy request</summary>
      public EndedDummyRequest() : this(null) { }
      /// <summary>Creates a new failed dummy request</summary>
      /// <param name="exception">Exception that caused the dummy to fail</param>
      public EndedDummyRequest(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }
      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override void ReraiseExceptions() {
        if(this.exception != null)
          throw this.exception;
      }
      /// <summary>Exception that supposedly caused the request to fail</summary>
      private Exception exception;
    }

    #endregion // EndedDummyRequest

    /// <summary>Creates a new failed dummy request</summary>
    /// <param name="exception">Exception that supposedly caused the request to fail</param>
    /// <returns>
    ///   A failed request that reports the provided exception as cause for its failure
    /// </returns>
    public static Request CreateFailedDummyRequest(Exception exception) {
      return new EndedDummyRequest(exception);
    }

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call End() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public virtual void Join() {

      // If the progression itself hasn't ended yet, block the caller until it has.
      if(!Ended)
        WaitHandle.WaitOne();

      // Allow the implementor to throw an exception in case an error has occured
      ReraiseExceptions();

    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected virtual void ReraiseExceptions() { }

  }

  /// <summary>Request providing a result that can be passed to the caller</summary>
  /// <typeparam name="ResultType">
  ///   Type of the result being provided by the request
  /// </typeparam>
  public abstract class Request<ResultType> : Request {

    #region class EndedDummyRequest

    /// <summary>Dummy request that is always in the ended state</summary>
    private class EndedDummyRequest : Request<ResultType> {
      /// <summary>Creates a new failed dummy request</summary>
      /// <param name="exception">Exception that caused the dummy to fail</param>
      public EndedDummyRequest(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }
      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override ResultType GatherResults() {
        throw this.exception;
      }
      /// <summary>Exception that supposedly caused the request to fail</summary>
      private Exception exception;
    }

    #endregion // EndedDummyRequest

    /// <summary>Creates a new failed dummy request</summary>
    /// <param name="exception">Exception that supposedly caused the request to fail</param>
    /// <returns>
    ///   A failed request that reports the provided exception as cause for its failure
    /// </returns>
    public static new Request CreateFailedDummyRequest(Exception exception) {
      return new EndedDummyRequest(exception);
    }

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call End() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public new ResultType Join() {
      base.Join();

      // Return the results of the request
      return GatherResults();
    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected abstract ResultType GatherResults();

  }

} // namespace Nuclex.Support.Tracking
