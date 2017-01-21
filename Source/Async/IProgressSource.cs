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

  /// <summary>Provides progress reports for an <see cref="IAsyncTask" /></summary>
  public interface IProgressSource {

    /// <summary>Called when the asynchronous action has made progress</summary>
    event EventHandler ProgressUpdated;

    /// <summary>Current progress of the asynchronous action</summary>
    float Progress { get; }

  }

  /// <summary>Provides progress reports with a status message</summary>
  /// <typeparam name="TStatus">Type of status message the progress source provides</typeparam>
  public interface IProgressSource<TStatus> : IProgressSource {

    /// <summary>Current status of the asynchronous action</summary>
    TStatus Status { get; }

  }

} // namespace Nuclex.Support.Async