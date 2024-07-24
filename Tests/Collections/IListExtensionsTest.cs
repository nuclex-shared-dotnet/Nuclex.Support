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
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the IList extension methods</summary>
  [TestFixture]
  internal class IListExtensionsTest {

    /// <summary>Tests whether the insertion sort algorithm sorts a list correctly</summary>
    [Test]
    public void InsertionSortCanSortWholeList() {
      var testList = new List<int>(capacity: 5) { 1, 5, 2, 4, 3 };
      var testListAsIList = (IList<int>)testList;

      testListAsIList.InsertionSort();

      CollectionAssert.AreEqual(
        new List<int>(capacity: 5) { 1, 2, 3, 4, 5 },
        testList
      );
    }

    /// <summary>Tests whether the insertion sort algorithm works on big lists</summary>
    [Test]
    public void InsertionSortCanSortBigList() {
      const int ListSize = 16384;

      var testList = new List<int>(capacity: ListSize);
      {
        var random = new Random();
        for(int index = 0; index < ListSize; ++index) {
          testList.Add(random.Next());
        }
      }

      var testListAsIList = (IList<int>)testList;
      testListAsIList.InsertionSort();

      for(int index = 1; index < ListSize; ++index) {
        Assert.LessOrEqual(testListAsIList[index - 1], testListAsIList[index]);
      }
    }

    /// <summary>Tests whether the insertion sort algorithm respects custom boundaries</summary>
    [Test]
    public void InsertionSortCanSortListSegment() {
      var testList = new List<int>(capacity: 7) { 9, 1, 5, 2, 4, 3, 0 };
      var testListAsIList = (IList<int>)testList;

      testListAsIList.InsertionSort(1, 5, Comparer<int>.Default);

      CollectionAssert.AreEqual(
        new List<int>(capacity: 7) { 9, 1, 2, 3, 4, 5, 0 },
        testList
      );
    }

    /// <summary>Tests whether the quicksort algorithm sorts a list correctly</summary>
    [Test]
    public void QuickSortCanSortWholeList() {
      var testList = new List<int>(capacity: 5) { 1, 5, 2, 4, 3 };
      var testListAsIList = (IList<int>)testList;

      testListAsIList.QuickSort();

      CollectionAssert.AreEqual(
        new List<int>(capacity: 5) { 1, 2, 3, 4, 5 },
        testList
      );
    }

    /// <summary>Tests whether the quicksort algorithm works on big lists</summary>
    [Test]
    public void QuickSortCanSortBigList() {
      const int ListSize = 16384;

      var testList = new List<int>(capacity: ListSize);
      {
        var random = new Random();
        for(int index = 0; index < ListSize; ++index) {
          testList.Add(random.Next());
        }
      }

      var testListAsIList = (IList<int>)testList;
      testListAsIList.QuickSort();

      for(int index = 1; index < ListSize; ++index) {
        Assert.LessOrEqual(testListAsIList[index - 1], testListAsIList[index]);
      }
    }

    /// <summary>Tests whether the quicksort algorithm respects custom boundaries</summary>
    [Test]
    public void QuickSortCanSortListSegment() {
      var testList = new List<int>(capacity: 7) { 9, 1, 5, 2, 4, 3, 0 };
      var testListAsIList = (IList<int>)testList;

      testListAsIList.QuickSort(1, 5, Comparer<int>.Default);

      CollectionAssert.AreEqual(
        new List<int>(capacity: 7) { 9, 1, 2, 3, 4, 5, 0 },
        testList
      );
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
