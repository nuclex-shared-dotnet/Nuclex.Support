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
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable collection class</summary>
  [TestFixture]
  public class ObservableCollectionTest {

    #region interface IObservableCollectionSubscriber

    /// <summary>Interface used to test the observable collection.</summary>
    public interface IObservableCollectionSubscriber {

      /// <summary>Called when the collection is about to clear its contents</summary>
      /// <param name="sender">Collection that is clearing its contents</param>
      /// <param name="e">Not used</param>
      void Clearing(object sender, EventArgs e);

      /// <summary>Called when an item is added to the collection</summary>
      /// <param name="sender">Collection to which an item is being added</param>
      /// <param name="e">Contains the item that is being added</param>
      void ItemAdded(object sender, ItemEventArgs<int> e);

      /// <summary>Called when an item is removed from the collection</summary>
      /// <param name="sender">Collection from which an item is being removed</param>
      /// <param name="e">Contains the item that is being removed</param>
      void ItemRemoved(object sender, ItemEventArgs<int> e);

    }

    #endregion // interface IObservableCollectionSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();

      this.mockedSubscriber = this.mockery.NewMock<IObservableCollectionSubscriber>();

      this.observedCollection = new ObservableCollection<int>();
      this.observedCollection.Clearing += new EventHandler(this.mockedSubscriber.Clearing);
      this.observedCollection.ItemAdded +=
        new EventHandler<ItemEventArgs<int>>(
          this.mockedSubscriber.ItemAdded
        );
      this.observedCollection.ItemRemoved +=
        new EventHandler<ItemEventArgs<int>>(
          this.mockedSubscriber.ItemRemoved
        );

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the Clearing event is fired</summary>
    [Test]
    public void TestClearingEvent() {
      Expect.Once.On(this.mockedSubscriber).
        Method("Clearing");

      this.observedCollection.Clear();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemAdded event is fired</summary>
    [Test]
    public void TestItemAddedEvent() {
      Expect.Once.On(this.mockedSubscriber).
        Method("ItemAdded").
        WithAnyArguments();

      this.observedCollection.Add(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemRemoved event is fired</summary>
    [Test]
    public void TestItemRemovedEvent() {
      Expect.Once.On(this.mockedSubscriber).
        Method("ItemAdded").
        WithAnyArguments();

      this.observedCollection.Add(123);

      Expect.Once.On(this.mockedSubscriber).
        Method("ItemRemoved").
        WithAnyArguments();

      this.observedCollection.Remove(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;
    /// <summary>The mocked observable collection subscriber</summary>
    private IObservableCollectionSubscriber mockedSubscriber;
    /// <summary>An observable collection to which a mock will be subscribed</summary>
    private ObservableCollection<int> observedCollection;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
