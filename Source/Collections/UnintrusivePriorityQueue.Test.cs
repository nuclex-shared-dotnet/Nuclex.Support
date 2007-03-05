using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the priority queue class</summary>
  [TestFixture]
  public class UnintrusivePriorityQueueTest {

    /// <summary>Tests to ensure the count property is properly updated</summary>
    [Test]
    public void TestCount() {
      UnintrusivePriorityQueue<string, float> testQueue =
        new UnintrusivePriorityQueue<string, float>();

      Assert.AreEqual(0, testQueue.Count);
      testQueue.Enqueue("a", 12.34f);
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue("b", 56.78f);
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Dequeue();
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue("c", 9.0f);
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Clear();
      Assert.AreEqual(0, testQueue.Count);
    }

    /// <summary>Tests to ensure that the priority collection actually sorts items</summary>
    [Test]
    public void TestOrdering() {
      UnintrusivePriorityQueue<string, float> testQueue =
        new UnintrusivePriorityQueue<string, float>();

      testQueue.Enqueue("a", 1.0f);
      testQueue.Enqueue("i", 9.0f);
      testQueue.Enqueue("b", 2.0f);
      testQueue.Enqueue("h", 8.0f);
      testQueue.Enqueue("c", 3.0f);
      testQueue.Enqueue("g", 7.0f);
      testQueue.Enqueue("d", 4.0f);
      testQueue.Enqueue("f", 6.0f);
      testQueue.Enqueue("e", 5.0f);

      Assert.AreEqual("i", testQueue.Dequeue());
      Assert.AreEqual("h", testQueue.Dequeue());
      Assert.AreEqual("g", testQueue.Dequeue());
      Assert.AreEqual("f", testQueue.Dequeue());
      Assert.AreEqual("e", testQueue.Dequeue());
      Assert.AreEqual("d", testQueue.Dequeue());
      Assert.AreEqual("c", testQueue.Dequeue());
      Assert.AreEqual("b", testQueue.Dequeue());
      Assert.AreEqual("a", testQueue.Dequeue());
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
