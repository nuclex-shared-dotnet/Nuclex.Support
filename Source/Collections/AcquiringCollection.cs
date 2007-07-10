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

  /// <summary>Generic collection of progressions</summary>
  public class AcquiringCollection<ItemType> : Collection<ItemType> {

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class that is empty.
    /// </summary>
    public AcquiringCollection() : base() { }

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class as a wrapper
    ///   for the specified list.
    /// </summary>
    /// <param name="list">The list that is wrapped by the new collection.</param>
    /// <exception cref="System.ArgumentNullException">
    ///    List is null.
    /// </exception>
    public AcquiringCollection(IList<ItemType> list) : base(list) { }

    /// <summary>Removes all elements from the Collection</summary>
    protected override void ClearItems() {
      OnClearing();

      base.ClearItems();
    }

    /// <summary>
    ///   Inserts an element into the ProgressionCollection at the specified index
    /// </summary>
    /// <param name="index">
    ///   The object to insert. The value can be null for reference types.
    /// </param>
    /// <param name="item">The zero-based index at which item should be inserted</param>
    protected override void InsertItem(int index, ItemType item) {
      base.InsertItem(index, item);

      OnAdded(item);
    }

    /// <summary>
    ///   Removes the element at the specified index of the ProgressionCollection
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove</param>
    protected override void RemoveItem(int index) {
      ItemType item = base[index];

      base.RemoveItem(index);

      OnRemoved(item);
    }

    /// <summary>Replaces the element at the specified index</summary>
    /// <param name="index">
    ///   The new value for the element at the specified index. The value can be null
    ///   for reference types
    /// </param>
    /// <param name="item">The zero-based index of the element to replace</param>
    protected override void SetItem(int index, ItemType item) {
      ItemType oldItem = base[index];

      base.SetItem(index, item);

      OnRemoved(oldItem);
      OnAdded(item);
    }

    /// <summary>Called when an item has been added to the collection</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(ItemType item) { }

    /// <summary>Called when an item has been removed from the collection</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(ItemType item) { }

    /// <summary>Called when the collection is being cleared</summary>
    /// <remarks>
    ///   Instead of called the OnRemoved() method for each item in the collection when
    ///   it is being cleared, this variant only triggers the OnClearing() method
    ///   to allow the implementer some room for optimizations. 
    /// </remarks>
    protected virtual void OnClearing() { }

  }

} // namespace Nuclex.Support.Collections
