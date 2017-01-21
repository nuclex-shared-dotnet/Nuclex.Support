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
