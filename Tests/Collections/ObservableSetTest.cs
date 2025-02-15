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

#if !NO_SETS

#if UNITTEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;
using NMock;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable set wrapper</summary>
  [TestFixture]
  internal class ObservableSetTest {

    #region interface IObservableCollectionSubscriber<TItem>

    public interface IObservableCollectionSubscriber<TItem> {

      /// <summary>Called when an item has been added to the collection</summary>
      void ItemAdded(object sender, ItemEventArgs<TItem> arguments);
      /// <summary>Called when an item is removed from the collection</summary>
      void ItemRemoved(object sender, ItemEventArgs<TItem> arguments);
      /// <summary>Called when an item is replaced in the collection</summary>
      void ItemReplaced(object sender, ItemReplaceEventArgs<TItem> arguments);
      /// <summary>Called when the collection is about to be cleared</summary>
      void Clearing(object sender, EventArgs arguments);
      /// <summary>Called when the collection has been cleared</summary>
      void Cleared(object sender, EventArgs arguments);

    }

    #endregion // interface IObservableCollectionSubscriber<TItem>

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockFactory = new MockFactory();
      this.observableSet = new ObservableSet<int>();

      this.subscriber = this.mockFactory.CreateMock<IObservableCollectionSubscriber<int>>();
      this.observableSet.ItemAdded += this.subscriber.MockObject.ItemAdded;
      this.observableSet.ItemRemoved += this.subscriber.MockObject.ItemRemoved;
      this.observableSet.ItemReplaced += this.subscriber.MockObject.ItemReplaced;
      this.observableSet.Clearing += this.subscriber.MockObject.Clearing;
      this.observableSet.Cleared += this.subscriber.MockObject.Cleared;
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockFactory != null) {
        this.mockFactory.VerifyAllExpectationsHaveBeenMet();

        this.subscriber = null;
        this.mockFactory.Dispose();
        this.mockFactory = null;
      }
    }

    /// <summary>
    ///   Verifies that the observable set has a default constructor
    /// </summary>
    [Test]
    public void HasDefaultConstructor() {
      Assert.IsNotNull(new ObservableSet<int>());
    }

    /// <summary>
    ///   Verifies that adding items to the set triggers the 'ItemAdded' event
    /// </summary>
    [Test]
    public void AddingItemsTriggersEvent() {
      this.subscriber.Expects.One.Method((s) => s.ItemAdded(null, null)).WithAnyArguments();
      this.observableSet.Add(123);
    }

    /// <summary>
    ///   Verifies that removing items from the set triggers the 'ItemRemoved' event
    /// </summary>
    [Test]
    public void RemovingItemsTriggersEvent() {
      this.subscriber.Expects.One.Method((s) => s.ItemAdded(null, null)).WithAnyArguments();
      this.observableSet.Add(123);

      this.subscriber.Expects.One.Method((s) => s.ItemRemoved(null, null)).WithAnyArguments();
      this.observableSet.Remove(123);
    }

    /// <summary>
    ///   Verifies that adding items to the set triggers the 'ItemAdded' event
    /// </summary>
    [Test]
    public void AddingAlreadyContainedItemDoesNotTriggerEvent() {
      this.subscriber.Expects.One.Method((s) => s.ItemAdded(null, null)).WithAnyArguments();
      this.observableSet.Add(123);

      this.subscriber.Expects.No.Method((s) => s.ItemAdded(null, null)).WithAnyArguments();
      this.observableSet.Add(123);
    }

    /// <summary>
    ///   Verifies that excepting the set with itself empties the set
    /// </summary>
    [Test]
    public void ExceptWithSelfEmptiesSet() {
      this.subscriber.Expects.Exactly(3).Method(
        (s) => s.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observableSet.Add(1);
      this.observableSet.Add(2);
      this.observableSet.Add(3);

      Assert.AreEqual(3, this.observableSet.Count);

      this.subscriber.Expects.One.Method((s) => s.Clearing(null, null)).WithAnyArguments();
      this.subscriber.Expects.One.Method((s) => s.Cleared(null, null)).WithAnyArguments();

      this.observableSet.ExceptWith(this.observableSet);
      Assert.AreEqual(0, this.observableSet.Count);
    }

    /// <summary>
    ///   Verifies that a set can be excepted with a collection
    /// </summary>
    [Test]
    public void SetCanBeExceptedWithCollection() {
      this.subscriber.Expects.Exactly(2).Method(
        (s) => s.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observableSet.Add(1);
      this.observableSet.Add(2);

      var collection = new List<int>() { 1 };

      this.subscriber.Expects.One.Method((s) => s.ItemRemoved(null, null)).WithAnyArguments();
      this.observableSet.ExceptWith(collection);
      Assert.AreEqual(1, this.observableSet.Count);
      Assert.IsTrue(this.observableSet.Contains(2));
    }

    /// <summary>
    ///   Verifies that a set can be intersected with a collection
    /// </summary>
    [Test]
    public void SetCanBeIntersectedWithCollection() {
      this.subscriber.Expects.Exactly(2).Method(
        (s) => s.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observableSet.Add(1);
      this.observableSet.Add(2);

      var collection = new List<int>() { 1 };

      this.subscriber.Expects.One.Method((s) => s.ItemRemoved(null, null)).WithAnyArguments();
      this.observableSet.IntersectWith(collection);
      Assert.AreEqual(1, this.observableSet.Count);
      Assert.IsTrue(this.observableSet.Contains(1));
    }

    /// <summary>
    ///   Verifies that it's possible to determine whether a set is a proper subset
    ///   or superset of another set
    /// </summary>
    [Test]
    public void CanDetermineProperSubsetAndSuperset() {
      var set1 = new ObservableSet<int>() { 1, 2, 3 };
      var set2 = new HashSet<int>() { 1, 3 };

      Assert.IsTrue(set1.IsProperSupersetOf(set2));
      Assert.IsTrue(set2.IsProperSubsetOf(set1));

      set2.Add(2);

      Assert.IsFalse(set1.IsProperSupersetOf(set2));
      Assert.IsFalse(set2.IsProperSubsetOf(set1));
    }

    /// <summary>
    ///   Verifies that it's possible to determine whether a set is a subset
    ///   or a superset of another set
    /// </summary>
    [Test]
    public void CanDetermineSubsetAndSuperset() {
      var set1 = new ObservableSet<int>() { 1, 2, 3 };
      var set2 = new HashSet<int>() { 1, 2, 3 };

      Assert.IsTrue(set1.IsSupersetOf(set2));
      Assert.IsTrue(set2.IsSubsetOf(set1));

      set2.Add(4);

      Assert.IsFalse(set1.IsSupersetOf(set2));
      Assert.IsFalse(set2.IsSubsetOf(set1));
    }

    /// <summary>
    ///   Verifies that a set can determine if another set overlaps with it
    /// </summary>
    [Test]
    public void CanDetermineOverlap() {
      var set1 = new ObservableSet<int>() { 1, 3, 5 };
      var set2 = new HashSet<int>() { 3 };

      Assert.IsTrue(set1.Overlaps(set2));
      Assert.IsTrue(set2.Overlaps(set1));
    }

    /// <summary>
    ///   Verifies that a set can determine if another set contains the same elements
    /// </summary>
    [Test]
    public void CanDetermineSetEquality() {
      var set1 = new ObservableSet<int>() { 1, 3, 5 };
      var set2 = new HashSet<int>() { 3, 1, 5 };

      Assert.IsTrue(set1.SetEquals(set2));
      Assert.IsTrue(set2.SetEquals(set1));

      set1.Add(7);

      Assert.IsFalse(set1.SetEquals(set2));
      Assert.IsFalse(set2.SetEquals(set1));
    }

    /// <summary>
    ///   Verifies that a set can be symmetrically excepted with another set
    /// </summary>
    [Test]
    public void CanBeSymmetricallyExcepted() {
      var set1 = new ObservableSet<int>() { 1, 2, 3 };
      var set2 = new HashSet<int>() { 3, 4, 5 };

      set1.SymmetricExceptWith(set2);

      Assert.AreEqual(4, set1.Count);
    }

    /// <summary>
    ///   Verifies that a union of two sets can be built
    /// </summary>
    [Test]
    public void CanBeUnioned() {
      this.subscriber.Expects.Exactly(3).Method(
        (s) => s.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observableSet.Add(1);
      this.observableSet.Add(2);
      this.observableSet.Add(3);

      var set2 = new ObservableSet<int>() { 3, 4, 5 };

      this.subscriber.Expects.Exactly(2).Method(
        (s) => s.ItemAdded(null, null)
      ).WithAnyArguments();
      this.observableSet.UnionWith(set2);
      Assert.AreEqual(5, this.observableSet.Count);
    }

    /// <summary>Creates mock object for the test</summary>
    private MockFactory mockFactory;
    /// <summary>Observable set being tested</summary>
    private ObservableSet<int> observableSet;
    /// <summary>Subscriber for the observable set's events</summary>
    private Mock<IObservableCollectionSubscriber<int>> subscriber;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST

#endif // !NO_SETS
