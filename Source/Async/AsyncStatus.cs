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

  /// <summary>Status of an <see cref="IAsyncTask" /></summary>
  public enum AsyncStatus {

    /// <summary>The process has not been started yet</summary>
    NotStarted,

    /// <summary>The process is currently executing</summary>
    Running,

    /// <summary>The process completed successfully</summary>
    Succeeded,

    /// <summary>The process encountered a problem</summary>
    Failed,

    /// <summary>The process was cancelled</summary>
    Cancelled

  }

} // namespace Nuclex.Support.Async