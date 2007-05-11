#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the priority queue class</summary>
  [TestFixture]
  public class PriorityQueueTest {

    /// <summary>Comparer for two floating point values</summary>
    private class FloatComparer : IComparer<float> {

      /// <summary>The default instance of this comparer</summary>
      public static readonly FloatComparer Default = new FloatComparer();

      /// <summary>Compares two floating points against each other</summary>
      /// <param name="left">First float to compare</param>
      /// <param name="right">Second float to compare</param>
      /// <returns>The relationship of the two floats to each other</returns>
      public int Compare(float left, float right) {
        return Math.Sign(left - right);
      }

    }

    /// <summary>Tests to ensure the count property is properly updated</summary>
    [Test]
    public void TestCount() {
      PriorityQueue<float> testQueue = new PriorityQueue<float>(FloatComparer.Default);

      Assert.AreEqual(0, testQueue.Count);
      testQueue.Enqueue(12.34f);
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue(56.78f);
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Dequeue();
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue(9.0f);
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Clear();
      Assert.AreEqual(0, testQueue.Count);
    }

    /// <summary>Tests to ensure that the priority collection actually sorts items</summary>
    [Test]
    public void TestOrdering() {
      PriorityQueue<float> testQueue = new PriorityQueue<float>(FloatComparer.Default);

      testQueue.Enqueue(1.0f);
      testQueue.Enqueue(9.0f);
      testQueue.Enqueue(2.0f);
      testQueue.Enqueue(8.0f);
      testQueue.Enqueue(3.0f);
      testQueue.Enqueue(7.0f);
      testQueue.Enqueue(4.0f);
      testQueue.Enqueue(6.0f);
      testQueue.Enqueue(5.0f);

      Assert.AreEqual(9.0f, testQueue.Dequeue());
      Assert.AreEqual(8.0f, testQueue.Dequeue());
      Assert.AreEqual(7.0f, testQueue.Dequeue());
      Assert.AreEqual(6.0f, testQueue.Dequeue());
      Assert.AreEqual(5.0f, testQueue.Dequeue());
      Assert.AreEqual(4.0f, testQueue.Dequeue());
      Assert.AreEqual(3.0f, testQueue.Dequeue());
      Assert.AreEqual(2.0f, testQueue.Dequeue());
      Assert.AreEqual(1.0f, testQueue.Dequeue());
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
