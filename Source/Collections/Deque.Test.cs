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

#if true

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
    ///   Tests whether the Insert() method of the deque can insert an item at
    ///   the end of the deque
    /// </summary>
    [Test]
    public void TestInsertAtEnd() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }
      
      intDeque.Insert(intDeque.Count, 12345);

      Assert.AreEqual(12345, intDeque.Last);
    }

    /// <summary>
    ///   Tests whether the Insert() method of the deque can insert an item into
    ///   the last block of the deque when that last block is already full
    /// </summary>
    [Test]
    public void TestInsertInLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.Insert(45, 12345);

      Assert.AreEqual(49, intDeque.Count);
      for(int index = 0; index < 44; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      Assert.AreEqual(12345, intDeque[45]);
      for(int index = 46; index < 49; ++index) {
        Assert.AreEqual(index - 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the Insert() method of the deque can insert an item into
    ///   the second-to-last block of the deque
    /// </summary>
    [Test]
    public void TestInsertInSecondToLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 40; ++item) {
        intDeque.AddLast(item);
      }
      
      intDeque.Insert(24, 12345);

      Assert.AreEqual(41, intDeque.Count);
      for(int index = 0; index < 24; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      Assert.AreEqual(12345, intDeque[24]);
      for(int index = 25; index < 41; ++index) {
        Assert.AreEqual(index - 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the Insert() method of the deque can insert an item into
    ///   the third-to-last block of the deque
    /// </summary>
    [Test]
    public void TestInsertInThirdToLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 40; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.Insert(8, 12345);

      Assert.AreEqual(41, intDeque.Count);

      for(int index = 0; index < 8; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      Assert.AreEqual(12345, intDeque[8]);
      for(int index = 9; index < 41; ++index) {
        Assert.AreEqual(index - 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method of the deque can remove an item from
    ///   the end of the deque
    /// </summary>
    [Test]
    public void TestRemoveAtEnd() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.RemoveAt(intDeque.Count - 1);

      Assert.AreEqual(46, intDeque.Last);
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method of the deque can remove an item
    ///   from the last block of the deque
    /// </summary>
    [Test]
    public void TestRemoveFromLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.RemoveAt(45);

      Assert.AreEqual(47, intDeque.Count);

      for(int index = 0; index < 45; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      for(int index = 45; index < 47; ++index) {
        Assert.AreEqual(index + 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method of the deque can remove an item from
    ///   the second-to-last block of the deque
    /// </summary>
    [Test]
    public void TestRemoveFromSecondToLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 40; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.RemoveAt(24);

      Assert.AreEqual(39, intDeque.Count);

      for(int index = 0; index < 24; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      for(int index = 24; index < 39; ++index) {
        Assert.AreEqual(index + 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method of the deque can remove an item from
    ///   the third-to-last block of the deque
    /// </summary>
    [Test]
    public void TestRemoveFromThirdToLastBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 33; ++item) {
        intDeque.AddLast(item);
      }

      intDeque.RemoveAt(8);

      Assert.AreEqual(32, intDeque.Count);

      for(int index = 0; index < 8; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      for(int index = 8; index < 32; ++index) {
        Assert.AreEqual(index + 1, intDeque[index]);
      }
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

  }

#endif

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
