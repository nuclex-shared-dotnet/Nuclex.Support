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
      remainingPartitions.Push(new Partition(startIndex, startIndex + count - 1));

      while(remainingPartitions.Count > 0) {
        Partition current = remainingPartitions.Pop();
        int leftEnd = current.LeftmostIndex;
        int rightEnd = current.RightmostIndex;

        int pivotIndex = quicksortPartition(list, leftEnd, rightEnd, comparer);
        if(pivotIndex - 1 > leftEnd) {
          remainingPartitions.Push(new Partition(leftEnd, pivotIndex - 1));
        }
        if(pivotIndex + 1 < rightEnd) {
          remainingPartitions.Push(new Partition(pivotIndex + 1, rightEnd));
        }
      }
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

    private static int quicksortPartition<TElement>(
      IList<TElement> list, int firstIndex, int lastIndex, IComparer<TElement> comparer
    ) {
      TElement pivot = list[lastIndex];
      
      // Set the high index element to its proper sorted position
      int nextIndex = firstIndex;
      for(int index = firstIndex; index < lastIndex; ++index) {
        if(comparer.Compare(list[index], pivot) < 0) {
          TElement temp = list[nextIndex];
          list[nextIndex] = list[index];
          list[index] = temp;

          ++nextIndex;
        }
      }
    
      // Set the high index value to its sorted position
      {
        TElement temp = list[nextIndex];
        list[nextIndex] = list[lastIndex];
        list[lastIndex] = temp;
      }

      // Returns the next sorting  element location
      return nextIndex;

    }

  }

} // namespace Nuclex.Support.Collections
