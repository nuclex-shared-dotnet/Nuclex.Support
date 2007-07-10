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
using System.Collections.ObjectModel;

namespace Nuclex.Support.Collections {

  /// <summary>Collection which fires events when items are added or removed</summary>
  public class ObservableCollection<ItemType> : AcquiringCollection<ItemType> {

    /// <summary>Raised when an item has been added to the collection</summary>
    public event EventHandler<ItemEventArgs<ItemType>> ItemAdded;
    /// <summary>Raised when an item is removed from the collection</summary>
    public event EventHandler<ItemEventArgs<ItemType>> ItemRemoved;
    /// <summary>Raised the collection is about to be cleared</summary>
    public event EventHandler Clearing;

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class that is empty.
    /// </summary>
    public ObservableCollection() : base() { }

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class as a wrapper
    ///   for the specified list.
    /// </summary>
    /// <param name="list">The list that is wrapped by the new collection.</param>
    /// <exception cref="System.ArgumentNullException">List is null.</exception>
    public ObservableCollection(IList<ItemType> list) : base(list) { }

    /// <summary>Fires the 'ItemAdded' event</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected override void OnAdded(ItemType item) {
      if(ItemAdded != null)
        ItemAdded(this, new ItemEventArgs<ItemType>(item));
    }

    /// <summary>Fires the 'ItemRemoved' event</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected override void OnRemoved(ItemType item) {
      if(ItemRemoved != null)
        ItemRemoved(this, new ItemEventArgs<ItemType>(item));
    }

    /// <summary>Fires the 'Clearing' event</summary>
    protected override void OnClearing() {
      if(Clearing != null)
        Clearing(this, EventArgs.Empty);
    }

  }

} // namespace Nuclex.Support.Collections
