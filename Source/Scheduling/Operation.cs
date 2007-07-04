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

    /// <summary>Executes the operation synchronously</summary>
    public virtual void Execute() {
      Begin();
      End();
    }

    /// <summary>Launches the background operation</summary>
    public abstract void Begin();

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method.
    /// </remarks>
    public virtual void End() {

      // Use some ingenious double-checked locking to set the endCalled flag.
      // Quite a lot of effort for a mere safety feature that prevents the programmer
      // from calling End() twice.
      bool error;
      if(!this.endCalled) {
        lock(this) {
          if(!this.endCalled) {
            this.endCalled = true;
            error = false;
          } else {
            error = true;
          }
        }
      } else {
        error = true;
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
    ///   in the background process. When it is set, the End() method will
    /// </remarks>
    protected Exception occuredException;
    /// <summary>Whether the End() method has been called already</summary>
    private volatile bool endCalled;

  }

} // namespace Nuclex.Support.Scheduling
