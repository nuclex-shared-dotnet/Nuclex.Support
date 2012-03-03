#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2012 Nuclex Development Labs

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
      var set = new ObservableSet<int>();
      set.Add(1);
      set.Add(2);
      set.Add(3);

      Assert.AreEqual(3, set.Count);
      set.ExceptWith(set);
      Assert.AreEqual(0, set.Count);
    }

    /// <summary>
    ///   Verifies that a set can be excepted with a collection
    /// </summary>
    [Test]
    public void SetCanBeExceptedWithCollection() {
      var set = new ObservableSet<int>();
      set.Add(1);
      set.Add(2);

      var collection = new List<int>() { 1 };

      Assert.AreEqual(2, set.Count);
      set.ExceptWith(collection);
      Assert.AreEqual(1, set.Count);
      Assert.IsTrue(set.Contains(2));
    }

    /// <summary>
    ///   Verifies that a set can be intersected with a collection
    /// </summary>
    [Test]
    public void SetCanBeIntersectedWithCollection() {
      var set = new ObservableSet<int>();
      set.Add(1);
      set.Add(2);

      var collection = new List<int>() { 1 };

      Assert.AreEqual(2, set.Count);
      set.IntersectWith(collection);
      Assert.AreEqual(1, set.Count);
      Assert.IsTrue(set.Contains(1));
    }

    /// <summary>
    ///   Verifies that it's possible to determine whether a set is a proper subset
    ///   or superset of another set
    /// </summary>
    [Test]
    public void CanDetermineProperSubsetAndSuperset() {
      var set1 = new ObservableSet<int>() { 1, 2, 3 };
      var set2 = new ObservableSet<int>() { 1, 3 };

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
      var set2 = new ObservableSet<int>() { 1, 2, 3 };

      Assert.IsTrue(set1.IsSupersetOf(set2));
      Assert.IsTrue(set2.IsSubsetOf(set1));

      set2.Add(4);

      Assert.IsFalse(set1.IsSupersetOf(set2));
      Assert.IsFalse(set2.IsSubsetOf(set1));
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