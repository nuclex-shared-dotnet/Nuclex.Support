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
using System.Diagnostics;

namespace Nuclex.Support {

  /// <summary>Manages a globally shared instance of the given Type</summary>
  /// <typeparam name="TShared">
  ///   Type of which a globally shared instance will be provided
  /// </typeparam>
  public static class Shared<TShared> where TShared : new() {

    /// <summary>Returns the global instance of the class</summary>
    [Obsolete("Avoid singletons at all costs. Consider a dependency injector instead.")]
    public static TShared Instance {
      [DebuggerStepThrough]
      get { return instance; }
    }

    /// <summary>Stored the globally shared instance</summary>
    private static readonly TShared instance = new TShared();

  }

} // namespace Nuclex.Support
