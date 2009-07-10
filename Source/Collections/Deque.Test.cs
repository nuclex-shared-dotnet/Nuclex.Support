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

#if false
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

  }
#endif

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
