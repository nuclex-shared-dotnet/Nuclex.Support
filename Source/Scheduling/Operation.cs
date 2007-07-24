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

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Base class for observable operations running in the background</summary>
  public abstract class Operation : Progression {

    /// <summary>Launches the background operation</summary>
    public abstract void Begin();

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call End() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public virtual void End() {

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

} // namespace Nuclex.Support.Scheduling
