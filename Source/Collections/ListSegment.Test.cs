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

  /// <summary>Unit Test for the list segment class</summary>
  [TestFixture]
  internal class ListSegmentTest {

    /// <summary>
    ///   Tests whether the default constructor of the ListSegment class throws the
    ///   right exception when being passed 'null' instead of a list
    /// </summary>
    [Test]
    public void SimpleConstructorThrowsWhenListIsNull() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new ListSegment<int>(null); }
      );
    }

    /// <summary>
    ///   Tests whether the simple constructor of the ListSegment class accepts
    ///   an empty list
    /// </summary>
    [Test]
    public void SimpleConstructorAcceptsEmptyList() {
      new ListSegment<int>(new List<int>());
    }

    /// <summary>
    ///   Tests whether the full constructor of the ListSegment class throws the
    ///   right exception when being passed 'null' instead of a string
    /// </summary>
    [Test]
    public void ConstructorThrowsWhenListIsNull() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new ListSegment<int>(null, 0, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the ListSegment class accepts
    ///   an empty string
    /// </summary>
    [Test]
    public void ConstructorAcceptsEmptyList() {
      new ListSegment<int>(new List<int>(), 0, 0);
    }

    /// <summary>
    ///   Tests whether the full constructor of the ListSegment class throws the
    ///   right exception when being passed an invalid start offset
    /// </summary>
    [Test]
    public void ConstructorThrowsOnInvalidOffset() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new ListSegment<int>(new List<int>(), -1, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the ListSegment class throws the
    ///   right exception when being passed an invalid element count
    /// </summary>
    [Test]
    public void ConstructorThrowsOnInvalidCount() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new ListSegment<int>(new List<int>(), 0, -1); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the ListSegment class throws the
    ///   right exception when being passed a string length that's too large
    /// </summary>
    [Test]
    public void ConstructorThrowsOnListOverrun() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      Assert.Throws<ArgumentException>(
        delegate() { new ListSegment<int>(testList, 3, 3); }
      );
    }

    /// <summary>Tests whether the 'Text' property works as expected</summary>
    [Test]
    public void ListPropertyStoresOriginalList() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 1, 3);
      Assert.AreSame(testList, testSegment.List);
    }

    /// <summary>Tests whether the 'Offset' property works as expected</summary>
    [Test]
    public void OffsetPropertyIsStored() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 1, 3);
      Assert.AreEqual(1, testSegment.Offset);
    }

    /// <summary>Tests whether the 'Count' property works as expected</summary>
    [Test]
    public void CountPropertyIsStored() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 1, 3);
      Assert.AreEqual(3, testSegment.Count);
    }

    /// <summary>
    ///   Tests whether two differing instances produce different hash codes
    /// </summary>
    [Test]
    public void DifferentInstancesHaveDifferentHashCodes_Usually() {
      var forwardCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var reverseCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1, 8
      );

      Assert.AreNotEqual(
        forwardCountSegment.GetHashCode(), reverseCountSegment.GetHashCode()
      );
    }

    /// <summary>
    ///   Tests whether two equivalent instances produce an identical hash code
    /// </summary>
    [Test]
    public void EquivalentInstancesHaveSameHashcode() {
      var testSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var identicalSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );

      Assert.AreEqual(
        testSegment.GetHashCode(), identicalSegment.GetHashCode()
      );
    }

    /// <summary>Tests the equals method performing a comparison against null</summary>
    [Test]
    public void EqualsAgainstNullIsAlwaysFalse() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 1, 3);

      Assert.IsFalse(
        testSegment.Equals(null)
      );
    }

    /// <summary>Tests the equality operator with differing instances</summary>
    [Test]
    public void DifferingInstancesAreNotEqual() {
      var forwardCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var reverseCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1, 8
      );

      Assert.IsFalse(forwardCountSegment == reverseCountSegment);
    }

    /// <summary>Tests the equality operator with equivalent instances</summary>
    [Test]
    public void EquivalentInstancesAreEqual() {
      var testSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var identicalSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );

      Assert.IsTrue(testSegment == identicalSegment);
    }

    /// <summary>Tests the inequality operator with differing instances</summary>
    [Test]
    public void DifferingInstancesAreUnequal() {
      var forwardCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var reverseCountSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 1, 8
      );

      Assert.IsTrue(forwardCountSegment != reverseCountSegment);
    }

    /// <summary>Tests the inequality operator with equivalent instances</summary>
    [Test]
    public void EquivalentInstancesAreNotUnequal() {
      var testSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );
      var identicalSegment = new ListSegment<int>(
        new List<int>(capacity: 9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2, 7
      );

      Assert.IsFalse(testSegment != identicalSegment);
    }

    /// <summary>Tests the ToString() method of the string segment</summary>
    [Test]
    public void TestToString() {
      var testList = new List<int>(capacity: 6) { 1, 2, 3, 4, 5, 6 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 2, 2);

      string stringRepresentation = testSegment.ToString();
      StringAssert.Contains("3, 4", stringRepresentation);
      StringAssert.DoesNotContain("2", stringRepresentation);
      StringAssert.DoesNotContain("5", stringRepresentation);
    }

    /// <summary>Tests whether the 'Text' property works as expected</summary>
    [Test]
    public void ToListReturnsSubset() {
      var testList = new List<int>(capacity: 5) { 1, 2, 3, 4, 5 };
      ListSegment<int> testSegment = new ListSegment<int>(testList, 1, 3);
      CollectionAssert.AreEqual(
        new List<int>(capacity: 3) { 2, 3, 4 },
        testSegment.ToList()
      );
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
