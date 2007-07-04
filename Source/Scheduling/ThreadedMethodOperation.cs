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

namespace Nuclex.Support.Scheduling {

  /// <summary>Operation that executes a method in a background thread</summary>
  public class ThreadedMethodOperation : Operation {

    /// <summary>
    ///   Initializes a new threaded method operation for a parameterless method
    /// </summary>
    /// <param name="method">Method to be invoked in a background thread</param>
    /// <remarks>
    ///   Uses a ThreadPool thread to execute the method in
    /// </remarks>
    public ThreadedMethodOperation(ThreadStart method)
      : this(method, true) { }

    /// <summary>
    ///   Initializes a new threaded method operation for a parameterless method
    /// </summary>
    /// <param name="method">Method to be invoked in a background thread</param>
    /// <param name="useThreadPool">Whether to use a ThreadPool thread</param>
    /// <remarks>
    ///   If useThreadPool is false, a new thread will be created. This guarantees
    ///   that the method will be executed immediately but has an impact on
    ///   performance since the creation of new threads is not a cheap operation.
    /// </remarks>
    public ThreadedMethodOperation(ThreadStart method, bool useThreadPool) {
      if(useThreadPool) {
        ThreadPool.QueueUserWorkItem(callMethod, method);
      } else {
        Thread thread = new Thread(callMethod);
        thread.Name = "Nuclex.Support.Scheduling.ThreadedMethodOperation thread";
        thread.IsBackground = true;
        thread.Start(method);
      }
    }

    /// <summary>Invokes the delegate passed as an argument</summary>
    /// <param name="method">ThreadStart-comaptible Delegate to invoke</param>
    private void callMethod(object method) {
#if PROGRESSION_STARTABLE
      AsyncStarted();
#endif

      try {
        ((ThreadStart)method)();
      }
      catch(Exception exception) {
        this.occuredException = exception;
      }
      finally {
        AsyncEnded();
      }
    }

  }

} // namespace Nuclex.Support.Scheduling
