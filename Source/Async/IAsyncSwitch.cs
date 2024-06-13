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
using System.Threading;

namespace Nuclex.Support.Async {

  /// <summary>State that can be switched in the background or externally</summary>
  /// <typeparam name="TOptions">Options that can be switched asynchronously</typeparam>
  public interface IAsyncSwitch<TOptions> : IAsyncTask {

    /// <summary>Switches to a different option or configuration</summary>
    /// <param name="option">Option or configuration to switch to</param>
    void Switch(TOptions option);

    /// <summary>Switches to a different option or configuration</summary>
    /// <param name="option">Option or configuration to switch to</param>
    /// <param name="cancellationToken">
    ///   Cancellation token by which the switch can be cancelled
    /// </param>
    void Switch(TOptions option, CancellationToken cancellationToken);

    /// <summary>Current target option or configuration being switched to</summary>
    TOptions Target { get; }

  }

} // namespace Nuclex.Support.Async
