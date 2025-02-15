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

  /// <summary>Unit Test for the Parenting Collection class</summary>
  [TestFixture]
  internal class ParentingCollectionTest {

    #region class TestParentable

    /// <summary>Parentable object that can be the child of an int</summary>
    private class TestParentable : Parentable<int>, IDisposable {

      /// <summary>Initializes a new instance of the parentable test class</summary>
      public TestParentable() { }

      /// <summary>The parent object that owns this instance</summary>
      public int GetParent() {
        return base.Parent;
      }

      /// <summary>Immediately releases all resources owned by the item</summary>
      public void Dispose() {
        this.disposeCalled = true;
      }

      /// <summary>Whether Dispose() has been called on this item</summary>
      public bool DisposeCalled {
        get { return this.disposeCalled; }
      }

      /// <summary>Whether Dispose() has been called on this item</summary>
      private bool disposeCalled;

    }

    #endregion // class TestParentable

    #region class TestParentingCollection

    /// <summary>Parentable object that can be the child of an int</summary>
    private class TestParentingCollection : ParentingCollection<int, TestParentable> {

      /// <summary>Changes the parent of the collection</summary>
      /// <param name="parent">New parent to assign to the collection</param>
      public void SetParent(int parent) {
        base.Reparent(parent);
      }

      /// <summary>Disposes all items contained in the collection</summary>
      public new void DisposeItems() {
        base.DisposeItems();
      }

    }

    #endregion // class TestParentingCollection

    /// <summary>
    ///   Tests whether the parenting collection propagates its parent to an item that
    ///   is added to the collection after the collection's aprent is already assigned
    /// </summary>
    [Test]
    public void TestPropagatePreassignedParent() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable = new TestParentable();

      testCollection.SetParent(54321);
      testCollection.Add(testParentable);

      Assert.AreEqual(54321, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether the parenting collection propagates a new parent to all items
    ///   contained in it when its parent is changed
    /// </summary>
    [Test]
    public void TestPropagateParentChange() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable = new TestParentable();

      testCollection.Add(testParentable);
      testCollection.SetParent(54321);

      Assert.AreEqual(54321, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether the parenting collection propagates its parent to an item that
    ///   is added to the collection after the collection's aprent is already assigned
    /// </summary>
    [Test]
    public void TestPropagateParentOnReplace() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable1 = new TestParentable();
      TestParentable testParentable2 = new TestParentable();

      testCollection.SetParent(54321);
      testCollection.Add(testParentable1);
      testCollection[0] = testParentable2;

      Assert.AreEqual(0, testParentable1.GetParent());
      Assert.AreEqual(54321, testParentable2.GetParent());
    }

    /// <summary>
    ///   Tests whether the parenting collection unsets the parent when an item is removed
    ///   from the collection
    /// </summary>
    [Test]
    public void TestUnsetParentOnRemoveItem() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable = new TestParentable();

      testCollection.Add(testParentable);
      testCollection.SetParent(54321);

      Assert.AreEqual(54321, testParentable.GetParent());

      testCollection.RemoveAt(0);

      Assert.AreEqual(0, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether the parenting collection unsets the parent when all item are
    ///   removed from the collection by clearing it
    /// </summary>
    [Test]
    public void TestUnsetParentOnClear() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable = new TestParentable();

      testCollection.Add(testParentable);
      testCollection.SetParent(54321);

      Assert.AreEqual(54321, testParentable.GetParent());

      testCollection.Clear();

      Assert.AreEqual(0, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether the parenting collection calls Dispose() on all contained items
    ///   that implement IDisposable when its DisposeItems() method is called
    /// </summary>
    [Test]
    public void TestDisposeItems() {
      TestParentingCollection testCollection = new TestParentingCollection();
      TestParentable testParentable = new TestParentable();

      testCollection.Add(testParentable);

      testCollection.DisposeItems();

      Assert.IsTrue(testParentable.DisposeCalled);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
