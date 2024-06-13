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
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  /// <summary>
  ///   Compares two values in reverse or reverses the output of another comparer
  /// </summary>
  /// <typeparam name="TCompared">Type of values to be compared</typeparam>
  public class ReverseComparer<TCompared> : IComparer<TCompared> {

    /// <summary>Initializes a new reverse comparer</summary>
    public ReverseComparer() : this(Comparer<TCompared>.Default) { }

    /// <summary>
    ///   Initializes the comparer to provide the inverse results of another comparer
    /// </summary>
    /// <param name="comparerToReverse">Comparer whose results will be inversed</param>
    public ReverseComparer(IComparer<TCompared> comparerToReverse) {
      this.comparer = comparerToReverse;
    }

    /// <summary>Compares the left value to the right value</summary>
    /// <param name="left">Value on the left side</param>
    /// <param name="right">Value on the right side</param>
    /// <returns>The relationship of the two values</returns>
    public int Compare(TCompared left, TCompared right) {
      return this.comparer.Compare(right, left); // intentionally reversed 
    }

    /// <summary>The default comparer from the .NET framework</summary>
    private IComparer<TCompared> comparer;

  }

} // namespace Nuclex.Support.Collections
