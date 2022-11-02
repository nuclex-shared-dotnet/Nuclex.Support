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
using System.Collections.Generic;
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the IList extension methods</summary>
  [TestFixture]
  internal class IListExtensionsTest {

    /// <summary>Tests whether the insertion sort algorithm can be applied to 'Text' property works as expected</summary>
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

    /// <summary>Tests whether the 'Text' property works as expected</summary>
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

    /// <summary>Tests whether the insertion sort algorithm can be applied to 'Text' property works as expected</summary>
    [Test]
    public void QuickSortCanSortWholeList() {
      var testList = new List<int>(capacity: 5) { 1, 5, 2, 4, 3 };
      var testListAsIList = (IList<int>)testList;
      
      testListAsIList.QuickSort(Comparer<int>.Default);

      CollectionAssert.AreEqual(
        new List<int>(capacity: 5) { 1, 2, 3, 4, 5 },
        testList
      );
    }


  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
