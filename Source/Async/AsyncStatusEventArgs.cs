#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2017 Nuclex Development Labs

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

namespace Nuclex.Support.Async {

  /// <summary>Provides the status of an <see cref="IAsyncTask" /></summary>
  public class AsyncStatusEventArgs : EventArgs {

    /// <summary>Reusable container that communicates that a task wasn't run</summary>
    public static readonly AsyncStatusEventArgs NotStarted = new AsyncStatusEventArgs(
      AsyncStatus.NotStarted
    );

    /// <summary>Reusable container that communicates that a task is running</summary>
    public static readonly AsyncStatusEventArgs Running = new AsyncStatusEventArgs(
      AsyncStatus.Running
    );

    /// <summary>Reusable container that communicates success</summary>
    public static readonly AsyncStatusEventArgs Success = new AsyncStatusEventArgs(
      AsyncStatus.Succeeded
    );

    /// <summary>Reusable container that communicates a failure</summary>
    public static readonly AsyncStatusEventArgs Failure = new AsyncStatusEventArgs(
      AsyncStatus.Failed
    );

    /// <summary>Reusable container that communicates a cancellation</summary>
    public static readonly AsyncStatusEventArgs Cancel = new AsyncStatusEventArgs(
      AsyncStatus.Cancelled
    );

    /// <summary>Initializes a new <see cref="IAsyncTask" /> outcome container</summary>
    /// <param name="status">Outcome the container will provide</param>
    public AsyncStatusEventArgs(AsyncStatus status) {
      this.status = status;
    }

    /// <summary>The status of the <see cref="IAsyncTask" /></summary>
    public AsyncStatus Status {
      get { return this.status; }
    }

    /// <summary><see cref="IAsyncTask" /> status stored by the container</summary>
    private AsyncStatus status;

  }

} // namespace Nuclex.Support.Async