#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

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