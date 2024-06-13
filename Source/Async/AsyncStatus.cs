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