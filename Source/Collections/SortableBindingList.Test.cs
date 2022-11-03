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
using System.ComponentModel;
using System.Reflection;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the SortableBindingList class</summary>
  [TestFixture]
  internal class SortableBindingListTest {

    #region class TestRecord

    /// <summary>Dummy record used to test the sortable binding list</summary>
    private class TestRecord {

      /// <summary>A property of type integer</summary>
      public int IntegerValue { get; set; }

      /// <summary>A property of type string</summary>
      public string StringValue { get; set; }

      /// <summary>A property of type float</summary>
      public float FloatValue { get; set; }

    }

    #endregion // class TestRecord

    /// <summary>Verifies that the sortable binding list is default constructible</summary>
    [Test]
    public void HasDefaultConstructor() {
      Assert.DoesNotThrow(
        delegate () { new SortableBindingList<TestRecord>(); }
      );
    }

    /// <summary>
    ///   Tests whether the sortable binding list can copy an existing list
    ///   when being constructed
    /// </summary>
    [Test]
    public void HasEnumerableConstructor() {
      var items = new List<TestRecord>() {
        new TestRecord() { IntegerValue = 123 },
        new TestRecord() { IntegerValue = 456 }
      };

      var testList = new SortableBindingList<TestRecord>(items);

      Assert.AreEqual(2, testList.Count);
      Assert.AreSame(items[0], testList[0]);
      Assert.AreSame(items[1], testList[1]);
    }

    /// <summary>Verifies that the sortable binding list supports sorting</summary>
    [Test]
    public void SupportsSorting() {
      var testList = new SortableBindingList<TestRecord>();
      IBindingList testListAsBindingList = testList;

      Assert.IsTrue(testListAsBindingList.SupportsSorting);
    }

    /// <summary>
    ///   Tests whether the sortable binding list can sort its elements by different properties
    /// </summary>
    [Test]
    public void CanSortItems() {
      var items = new List<TestRecord>() {
        new TestRecord() { IntegerValue = 456 },
        new TestRecord() { IntegerValue = 789 },
        new TestRecord() { IntegerValue = 123 }
      };

      var testList = new SortableBindingList<TestRecord>(items);
      IBindingList testListAsBindingList = testList;

      PropertyDescriptor integerValuePropertyDescriptor = (
        TypeDescriptor.GetProperties(typeof(TestRecord))[nameof(TestRecord.IntegerValue)]
      );
      testListAsBindingList.ApplySort(
        integerValuePropertyDescriptor, ListSortDirection.Ascending
      );

      Assert.AreEqual(3, testList.Count);
      Assert.AreEqual(123, testList[0].IntegerValue);
      Assert.AreEqual(456, testList[1].IntegerValue);
      Assert.AreEqual(789, testList[2].IntegerValue);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
