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
using System.Collections;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the read only collection wrapper</summary>
  [TestFixture]
  internal class ReadOnlyCollectionTest {

    /// <summary>
    ///   Verifies that the copy constructor of the read only collection works
    /// </summary>
    [Test]
    public void TestCopyConstructor() {
      int[] integers = new int[] { 12, 34, 56, 78 };
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(integers);

      CollectionAssert.AreEqual(integers, testCollection);
    }

    /// <summary>Verifies that the IsReadOnly property returns true</summary>
    [Test]
    public void TestIsReadOnly() {
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(new int[0]);

      Assert.IsTrue(testCollection.IsReadOnly);
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only collection works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(inputIntegers);

      int[] outputIntegers = new int[testCollection.Count];
      testCollection.CopyTo(outputIntegers, 0);

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only collection is able to
    ///   determine if the collection contains an item
    /// </summary>
    [Test]
    public void TestContains() {
      int[] integers = new int[] { 1234, 6789 };
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(integers);

      Assert.IsTrue(testCollection.Contains(1234));
      Assert.IsFalse(testCollection.Contains(4321));
    }

    /// <summary>
    ///   Ensures that the Add() method of the read only collection throws an exception
    /// </summary>
    [Test]
    public void TestThrowOnAdd() {
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<int>).Add(123); }
      );
    }

    /// <summary>
    ///   Ensures that the Remove() method of the read only collection throws an exception
    /// </summary>
    [Test]
    public void TestThrowOnRemove() {
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<int>).Remove(123); }
      );
    }

    /// <summary>
    ///   Ensures that the Clear() method of the read only collection throws an exception
    /// </summary>
    [Test]
    public void TestThrowOnClear() {
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<int>).Clear(); }
      );
    }

    /// <summary>
    ///   Tests whether the typesafe enumerator of the read only collection is working
    /// </summary>
    [Test]
    public void TestTypesafeEnumerator() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(inputIntegers);

      List<int> outputIntegers = new List<int>();
      foreach(int value in testCollection) {
        outputIntegers.Add(value);
      }

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only collection works if invoked via
    ///   the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(inputIntegers);

      int[] outputIntegers = new int[testCollection.Count];
      (testCollection as ICollection).CopyTo(outputIntegers, 0);

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      ReadOnlyCollection<int> testCollection = new ReadOnlyCollection<int>(new int[0]);

      if(!(testCollection as ICollection).IsSynchronized) {
        lock((testCollection as ICollection).SyncRoot) {
          Assert.AreEqual(0, testCollection.Count);
        }
      }
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
