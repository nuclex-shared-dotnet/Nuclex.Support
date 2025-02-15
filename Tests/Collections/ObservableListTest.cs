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

#if !NO_NMOCK

using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;
using NMock;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable list class</summary>
  [TestFixture]
  internal class ObservableListTest {

    #region interface IObservableCollectionSubscriber

    /// <summary>Interface used to test the observable collection</summary>
    public interface IObservableCollectionSubscriber {

      /// <summary>Called when the collection is about to clear its contents</summary>
      /// <param name="sender">Collection that is clearing its contents</param>
      /// <param name="arguments">Not used</param>
      void Clearing(object sender, EventArgs arguments);

      /// <summary>Called when the collection has been cleared of its contents</summary>
      /// <param name="sender">Collection that was cleared of its contents</param>
      /// <param name="arguments">Not used</param>
      void Cleared(object sender, EventArgs arguments);

      /// <summary>Called when an item is added to the collection</summary>
      /// <param name="sender">Collection to which an item is being added</param>
      /// <param name="arguments">Contains the item that is being added</param>
      void ItemAdded(object sender, ItemEventArgs<int> arguments);

      /// <summary>Called when an item is removed from the collection</summary>
      /// <param name="sender">Collection from which an item is being removed</param>
      /// <param name="arguments">Contains the item that is being removed</param>
      void ItemRemoved(object sender, ItemEventArgs<int> arguments);

      /// <summary>Called when an item is replaced in the dictionary</summary>
      /// <param name="sender">Dictionary in which an item is being replaced</param>
      /// <param name="arguments">Contains the replaced item and its replacement</param>
      void ItemReplaced(object sender, ItemReplaceEventArgs<int> arguments);

    }

    #endregion // interface IObservableCollectionSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new MockFactory();

      this.mockedSubscriber = this.mockery.CreateMock<IObservableCollectionSubscriber>();

      this.observedList = new ObservableList<int>();
      this.observedList.Clearing += new EventHandler(
        this.mockedSubscriber.MockObject.Clearing
      );
      this.observedList.Cleared += new EventHandler(
        this.mockedSubscriber.MockObject.Cleared
      );
      this.observedList.ItemAdded += new EventHandler<ItemEventArgs<int>>(
        this.mockedSubscriber.MockObject.ItemAdded
      );
      this.observedList.ItemRemoved += new EventHandler<ItemEventArgs<int>>(
        this.mockedSubscriber.MockObject.ItemRemoved
      );
      this.observedList.ItemReplaced += new EventHandler<ItemReplaceEventArgs<int>>(
        this.mockedSubscriber.MockObject.ItemReplaced
      );
    }

    /// <summary>Tests whether the Clearing event is fired</summary>
    [Test]
    public void TestClearingEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.Clearing(null, null)).WithAnyArguments();
      this.mockedSubscriber.Expects.One.Method(m => m.Cleared(null, null)).WithAnyArguments();
      this.observedList.Clear();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemAdded event is fired</summary>
    [Test]
    public void TestItemAddedEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.ItemAdded(null, null)).WithAnyArguments();

      this.observedList.Add(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemRemoved event is fired</summary>
    [Test]
    public void TestItemRemovedEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.ItemAdded(null, null)).WithAnyArguments();

      this.observedList.Add(123);

      this.mockedSubscriber.Expects.One.Method(m => m.ItemRemoved(null, null)).WithAnyArguments();

      this.observedList.Remove(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether items in the collection can be replaced</summary>
    [Test]
    public void TestItemReplacement() {
      this.mockedSubscriber.Expects.Exactly(3).Method(
        m => m.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observedList.Add(1);
      this.observedList.Add(2);
      this.observedList.Add(3);

      this.mockedSubscriber.Expects.One.Method(m => m.ItemReplaced(null, null)).WithAnyArguments();

      // Replace the middle item with something else
      this.observedList[1] = 4;

      Assert.AreEqual(
        1, this.observedList.IndexOf(4)
      );

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether a the list constructor is working</summary>
    [Test]
    public void TestListConstructor() {
      int[] integers = new int[] { 12, 34, 56, 78 };

      var testList = new ObservableList<int>(integers);

      CollectionAssert.AreEqual(integers, testList);
    }

    /// <summary>Mock object factory</summary>
    private MockFactory mockery;
    /// <summary>The mocked observable collection subscriber</summary>
    private Mock<IObservableCollectionSubscriber> mockedSubscriber;
    /// <summary>An observable collection to which a mock will be subscribed</summary>
    private ObservableList<int> observedList;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST

#endif // !NO_NMOCK
