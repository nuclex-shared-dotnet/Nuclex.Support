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

  /// <summary>Task that runs in the background or externally</summary>
  public interface IAsyncTask {

    /// <summary>Triggered when the process starts running</summary>
    event EventHandler Started;

    /// <summary>Triggered when the action finishes for any reason</summary>
    event EventHandler Finished;

    /// <summary>Whether the action is currently running</summary>
    bool IsRunning { get;  }

  }

} // namespace Nuclex.Support.Async