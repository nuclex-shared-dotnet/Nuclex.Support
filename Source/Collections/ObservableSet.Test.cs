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

      /// <summary>Raised when an item has been added to the collection</summary>
      event EventHandler<ItemEventArgs<TItem>> ItemAdded;
      /// <summary>Raised when an item is removed from the collection</summary>
      event EventHandler<ItemEventArgs<TItem>> ItemRemoved;
      /// <summary>Raised when an item is replaced in the collection</summary>
      event EventHandler<ItemReplaceEventArgs<TItem>> ItemReplaced;
      /// <summary>Raised when the collection is about to be cleared</summary>
      event EventHandler Clearing;
      /// <summary>Raised when the collection has been cleared</summary>
      event EventHandler Cleared;

    }

    #endregion // interface IObservableCollectionSubscriber<TItem>

    

    /// <summary>
    ///   Verifies that the observable set has a default constructor
    /// </summary>
    [Test]
    public void HasDefaultConstructor() {
      Assert.IsNotNull(new ObservableSet<int>());
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST