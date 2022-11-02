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
using System.Collections;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  /// <summary>Extension methods for the IList interface</summary>
  public static class ListExtensions {

    /// <summary>
    ///   Sorts a subset of the elements in an IList&lt;T&gt; using the insertion sort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    /// <param name="startIndex">Index at which the sorting process will begin</param>
    /// <param name="count">Index one past the last element that will be sorted</param>
    /// <param name="comparer">Comparison function to use for comparing list elements</param>
    public static void InsertionSort<TElement>(
      this IList<TElement> list, int startIndex, int count, IComparer<TElement> comparer
    ) {
      int index = startIndex;
      int endIndex = startIndex + count - 1;

      while(index < endIndex) {
        int rightIndex = index;
        
        ++index;
        TElement temp = list[index];

        while(rightIndex >= startIndex) {
          if(comparer.Compare(list[rightIndex], temp) < 0) {
            break;
          }

          list[rightIndex + 1] = list[rightIndex];
          --rightIndex;
        }

        list[rightIndex + 1] = temp;
      }
    }

    /// <summary>
    ///   Sorts all the elements in an IList&lt;T&gt; using the insertion sort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    /// <param name="comparer">Comparison function to use for comparing list elements</param>
    public static void InsertionSort<TElement>(
      this IList<TElement> list, IComparer<TElement> comparer
    ) {
      InsertionSort(list, 0, list.Count, comparer);
    }

    /// <summary>
    ///   Sorts all the elements in an IList&lt;T&gt; using the insertion sort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    public static void InsertionSort<TElement>(this IList<TElement> list) {
      InsertionSort(list, Comparer<TElement>.Default);
    }

  }

} // namespace Nuclex.Support.Collections
