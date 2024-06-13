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
