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

  /// <summary>Extension methods for the IList interface</summary>
  public static class ListExtensions {

    #region struct Partition

    /// <summary>
    ///   Stores the left and right index of a partition for the quicksort algorithm
    /// </summary>
    private struct Partition {

      /// <summary>
      ///   Initializes a new partition using the specified left and right index
      /// </summary>
      /// <param name="leftmostIndex">Index of the leftmost element in the partition</param>
      /// <param name="rightmostIndex">Index of the rightmost element in the partition</param>
      public Partition(int leftmostIndex, int rightmostIndex) {
        this.LeftmostIndex = leftmostIndex;
        this.RightmostIndex = rightmostIndex;
      }

      /// <summary>Index of the leftmost element in the partition</summary>
      public int LeftmostIndex;
      /// <summary>Index of the rightmost element in the partition</summary>
      public int RightmostIndex;

    }

    #endregion // struct Partition

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
      int rightIndex = startIndex;

      int lastIndex = startIndex + count - 1;
      for(int index = startIndex + 1; index <= lastIndex; ++index) {
        TElement temp = list[index];

        while(rightIndex >= startIndex) {
          if(comparer.Compare(list[rightIndex], temp) < 0) {
            break;
          }

          list[rightIndex + 1] = list[rightIndex];
          --rightIndex;
        }

        list[rightIndex + 1] = temp;

        rightIndex = index;
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
      InsertionSort(list, 0, list.Count, Comparer<TElement>.Default);
    }

    /// <summary>
    ///   Sorts all the elements in an IList&lt;T&gt; using the quicksort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    /// <param name="startIndex">Index at which the sorting process will begin</param>
    /// <param name="count">Index one past the last element that will be sorted</param>
    /// <param name="comparer">Comparison function to use for comparing list elements</param>
    public static void QuickSort<TElement>(
      this IList<TElement> list, int startIndex, int count, IComparer<TElement> comparer
    ) {
      var remainingPartitions = new Stack<Partition>();

      int lastIndex = startIndex + count - 1;
      for(; ; ) {
        int pivotIndex = quicksortPartition(list, startIndex, lastIndex, comparer);

        // This block just queues the next partitions left of the pivot point and right
        // of the pivot point (if they contain at least 2 elements). It's fattened up
        // a bit by trying to forego the stack and adjusting the startIndex/lastIndex
        // directly where it's clear the next loop can process these partitions.
        if(pivotIndex - 1 > startIndex) { // Are the elements to sort right of the pivot?
          if(pivotIndex + 1 < lastIndex) {  // Are the elements left of the pivot as well?
            remainingPartitions.Push(new Partition(startIndex, pivotIndex - 1));
            startIndex = pivotIndex + 1;
          } else { // Elements to sort are only right of the pivot
            lastIndex = pivotIndex - 1;
          }
        } else if(pivotIndex + 1 < lastIndex) { // Are elements to sort only left of the pivot?
          startIndex = pivotIndex + 1;
        } else { // Partition was fully sorted

          // Did we process all queued partitions? If so, the list is sorted
          if(remainingPartitions.Count == 0) {
            return;
          }

          // Pull the next partition that needs to be sorted from the stack
          Partition current = remainingPartitions.Pop();
          startIndex = current.LeftmostIndex;
          lastIndex = current.RightmostIndex;

        } // if sortable sub-partitions exist left/right/nowhere
      } // for ever (termination inside loop)
    }

    /// <summary>
    ///   Sorts all the elements in an IList&lt;T&gt; using the insertion sort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    /// <param name="comparer">Comparison function to use for comparing list elements</param>
    public static void QuickSort<TElement>(
      this IList<TElement> list, IComparer<TElement> comparer
    ) {
      QuickSort(list, 0, list.Count, comparer);
    }

    /// <summary>
    ///   Sorts all the elements in an IList&lt;T&gt; using the insertion sort algorithm
    /// </summary>
    /// <typeparam name="TElement">Type of elements the list contains</typeparam>
    /// <param name="list">List in which a subset will be sorted</param>
    public static void QuickSort<TElement>(this IList<TElement> list) {
      QuickSort(list, 0, list.Count, Comparer<TElement>.Default);
    }

    /// <summary>
    ///   Moves an element downward over all elements that precede it in the sort order
    /// </summary>
    /// <typeparam name="TElement">Type of elements stored in the sorted list</typeparam>
    /// <param name="list">List that is being sorted</param>
    /// <param name="firstIndex">Index of the first element in the partition</param>
    /// <param name="lastIndex">Index of hte last element in the partition</param>
    /// <param name="comparer">
    ///   Comparison function that decides the ordering of elements
    /// </param>
    /// <returns>The index of the next pivot element</returns>
    private static int quicksortPartition<TElement>(
      IList<TElement> list, int firstIndex, int lastIndex, IComparer<TElement> comparer
    ) {

      // Step through all elements in the partition and accumulate those that are smaller
      // than the last element on the left (by swapping). At the end 'firstIndex' will be
      // the new pivot point, left of which are all elements smaller than the element at
      // 'lastIndex' and right of it will be all elements which are larger.
      for(int index = firstIndex; index < lastIndex; ++index) {
        if(comparer.Compare(list[index], list[lastIndex]) < 0) {
          TElement temp = list[firstIndex];
          list[firstIndex] = list[index];
          list[index] = temp;

          ++firstIndex;
        }
      }

      // The element at 'lastIndex' as a sort value that's in the middle of the two sides,
      // so we'll have to swap it, too, putting it in the middle and making it the new pivot.
      {
        TElement temp = list[firstIndex];
        list[firstIndex] = list[lastIndex];
        list[lastIndex] = temp;
      }

      // Return the index of the new pivot position
      return firstIndex;

    }

  }

} // namespace Nuclex.Support.Collections
