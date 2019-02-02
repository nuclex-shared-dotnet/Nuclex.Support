#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2019 Nuclex Development Labs

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

namespace Nuclex.Support.Threading {

	/// <summary>Action with no arguments that can be cancelled</summary>
	/// <param name="cancellationToken">
	///   Cancellation token by which the action can be cancelled
	/// </param>
	public delegate void CancellableAction(CancellationToken cancellationToken);

	/// <summary>Action with no arguments that can be cancelled</summary>
	/// <param name="cancellationToken">
	///   Cancellation token by which the action can be cancelled
	/// </param>
	/// <param name="arg1">First argument for the action</param>
	public delegate void CancellableAction<in T1>(T1 arg1, CancellationToken cancellationToken);

	/// <summary>Action with no arguments that can be cancelled</summary>
	/// <param name="cancellationToken">
	///   Cancellation token by which the action can be cancelled
	/// </param>
	/// <param name="arg1">First argument for the action</param>
	/// <param name="arg2">Second argument for the action</param>
	public delegate void CancellableAction<in T1, in T2>(
		T1 arg1, T2 arg2, CancellationToken cancellationToken
	);

} // namespace Nuclex.Support.Threading
