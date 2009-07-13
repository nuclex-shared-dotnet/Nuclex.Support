#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

#if UNITTEST

using System;
using System.Collections.Generic;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the double ended queue</summary>
  [TestFixture]
  public class DequeTest {

    /// <summary>Verifies that the AddLast() method of the deque is working</summary>
    [Test]
    public void TestAddLast() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(item, intDeque[item]);
      }
    }

    /// <summary>Verifies that the AddFirst() method of the deque is working</summary>
    [Test]
    public void TestAddFirst() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddFirst(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(47 - item, intDeque[item]);
      }
    }

    /// <summary>
    ///   Verifies that the RemoveFirst() method of the deque is working
    /// </summary>
    [Test]
    public void TestRemoveFirst() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(item, intDeque.First);
        Assert.AreEqual(48 - item, intDeque.Count);
        intDeque.RemoveFirst();
      }
    }

    /// <summary>
    ///   Verifies that the RemoveLast() method of the deque is working
    /// </summary>
    [Test]
    public void TestRemoveLast() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(47 - item, intDeque.Last);
        Assert.AreEqual(48 - item, intDeque.Count);
        intDeque.RemoveLast();
      }
    }


    /// <summary>Verifies that the Insert() method works in all cases</summary>
    /// <remarks>
    ///   We have several different cases here that will be tested. The deque can
    ///   shift items to the left or right (depending on which end is closer to
    ///   the insertion point) and the insertion point may fall in an only partially
    ///   occupied block, requiring elaborate index calculations
    /// </remarks>
    [Test]
    public void TestInsert() {
      for(int testedIndex = 0; testedIndex <= 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 0; item < 96; ++item) {
          intDeque.AddLast(item);
        }

        intDeque.Insert(testedIndex, 12345);

        Assert.AreEqual(97, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        Assert.AreEqual(12345, intDeque[testedIndex]);
        for(int index = testedIndex + 1; index < 97; ++index) {
          Assert.AreEqual(index - 1, intDeque[index]);
        }
      }
    }

    /// <summary>Verifies that the Insert() method works in all cases</summary>
    [Test]
    public void TestInsertNonNormalized() {
      for(int testedIndex = 0; testedIndex <= 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 4; item < 96; ++item) {
          intDeque.AddLast(item);
        }
        intDeque.AddFirst(3);
        intDeque.AddFirst(2);
        intDeque.AddFirst(1);
        intDeque.AddFirst(0);

        intDeque.Insert(testedIndex, 12345);

        Assert.AreEqual(97, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        Assert.AreEqual(12345, intDeque[testedIndex]);
        for(int index = testedIndex + 1; index < 97; ++index) {
          Assert.AreEqual(index - 1, intDeque[index]);
        }
      }
    }

    /// <summary>Verifies the the RemoveAt() method works in all cases</summary>
    [Test]
    public void TestRemoveAt() {
      for(int testedIndex = 0; testedIndex < 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 0; item < 96; ++item) {
          intDeque.AddLast(item);
        }

        intDeque.RemoveAt(testedIndex);

        Assert.AreEqual(95, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        for(int index = testedIndex; index < 95; ++index) {
          Assert.AreEqual(index + 1, intDeque[index]);
        }
      }
    }

    /// <summary>Verifies the the RemoveAt() method works in all cases</summary>
    [Test]
    public void TestRemoveAtNonNormalized() {
      for(int testedIndex = 0; testedIndex < 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 4; item < 96; ++item) {
          intDeque.AddLast(item);
        }
        intDeque.AddFirst(3);
        intDeque.AddFirst(2);
        intDeque.AddFirst(1);
        intDeque.AddFirst(0);

        intDeque.RemoveAt(testedIndex);

        Assert.AreEqual(95, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        for(int index = testedIndex; index < 95; ++index) {
          Assert.AreEqual(index + 1, intDeque[index]);
        }
      }
    }

    /// <summary>
    ///   Validates that an exception is thrown if the 'First' property is accessed
    ///   in an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnAccessFirstInEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { Console.WriteLine(intDeque.First); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the 'Last' property is accessed
    ///   in an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnAccessLastInEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { Console.WriteLine(intDeque.Last); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the first item is attempted to be 
    ///   removed from an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnRemoveFirstFromEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { intDeque.RemoveFirst(); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the last item is attempted to be 
    ///   removed from an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnRemoveLastFromEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { intDeque.RemoveLast(); }
      );
    }

    /// <summary>
    ///   Verifies that items can be assigned by their index
    /// </summary>
    [Test]
    public void TestIndexAssignment() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 32; ++item) {
        intDeque.AddLast(item);
      }
      intDeque[16] = 12345;
      intDeque[17] = 54321;

      for(int index = 0; index < 16; ++index) {
        intDeque.RemoveFirst();
      }

      Assert.AreEqual(12345, intDeque.First);
      intDeque.RemoveFirst();
      Assert.AreEqual(54321, intDeque.First);
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an invalid index is accessed
    /// </summary>
    [Test]
    public void TestThrowOnInvalidIndex() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 32; ++item) {
        intDeque.AddLast(item);
      }

      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { Console.WriteLine(intDeque[32]); }
      );
    }

    /// <summary>Tests the IndexOf() method</summary>
    [Test, TestCase(0), TestCase(16), TestCase(32), TestCase(48)]
    public void TestIndexOf(int count) {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 4; item < count; ++item) {
        intDeque.AddLast(item);
      }
      if(count > 3) { intDeque.AddFirst(3); }
      if(count > 2) { intDeque.AddFirst(2); }
      if(count > 1) { intDeque.AddFirst(1); }
      if(count > 0) { intDeque.AddFirst(0); }

      Assert.AreEqual(count - 1, intDeque.IndexOf(count - 1));
      Assert.AreEqual(-1, intDeque.IndexOf(count));
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
