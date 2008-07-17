using System;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  /// <summary>
  ///   Compares two values in reverse or reverses the output of another comparer
  /// </summary>
  /// <typeparam name="ComparedType">Type of values to be compared</typeparam>
  public class ReverseComparer<ComparedType> : IComparer<ComparedType> {

    /// <summary>Initializes a new reverse comparer</summary>
    public ReverseComparer() : this(Comparer<ComparedType>.Default) { }

    /// <summary>
    ///   Initializes the comparer to provide the inverse results of another comparer
    /// </summary>
    /// <param name="comparerToReverse">Comparer whose results will be inversed</param>
    public ReverseComparer(IComparer<ComparedType> comparerToReverse) {
      this.comparer = comparerToReverse;
    }

    /// <summary>Compares the left value to the right value</summary>
    /// <param name="left">Value on the left side</param>
    /// <param name="right">Value on the right side</param>
    /// <returns>The relationship of the two values</returns>
    public int Compare(ComparedType left, ComparedType right) {
      return this.comparer.Compare(right, left); // intentionally reversed 
    }

    /// <summary>The default comparer from the .NET framework</summary>
    private IComparer<ComparedType> comparer;

  }

} // namespace Nuclex.Support.Collections
