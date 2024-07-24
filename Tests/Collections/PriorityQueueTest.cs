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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the priority queue class</summary>
  [TestFixture]
  internal class PriorityQueueTest {

    #region class FloatComparer

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

    #endregion // class FloatComparer

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

#if DEBUG
    /// <summary>
    ///   Tests whether the priority queue's enumerators are invalidated when the queue's
    ///   contents are modified
    /// </summary>
    [Test]
    public void TestEnumeratorInvalidationOnModify() {
      PriorityQueue<int> testQueue = new PriorityQueue<int>();
      IEnumerator<int> testQueueEnumerator = testQueue.GetEnumerator();

      testQueue.Enqueue(123);

      Assert.Throws<InvalidOperationException>(
        delegate() { testQueueEnumerator.MoveNext(); }
      );
    }
#endif

    /// <summary>
    ///   Verifies that an exception is thrown when Peek() is called on an empty queue
    /// </summary>
    [Test]
    public void TestPeekEmptyQueue() {
      PriorityQueue<int> testQueue = new PriorityQueue<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { testQueue.Peek(); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown when Dequeue() is called on an empty queue
    /// </summary>
    [Test]
    public void TestDequeueEmptyQueue() {
      PriorityQueue<int> testQueue = new PriorityQueue<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { testQueue.Dequeue(); }
      );
    }

    /// <summary>
    ///   Verifies that the priority queue can handle large amounts of data
    /// </summary>
    [Test]
    public void TestLargeQueue() {
      PriorityQueue<int> testQueue = new PriorityQueue<int>();
      List<int> testList = new List<int>();

      for(int index = 0; index < 1000; ++index) {
        testQueue.Enqueue(index * 2);
        testList.Add(index * 2);
      }

      CollectionAssert.AreEquivalent(testList, testQueue);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
