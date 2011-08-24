#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
#if !NO_SPECIALIZED_COLLECTIONS
using System.Collections.Specialized;
#endif

namespace Nuclex.Support.Collections {

  /// <summary>Collection which fires events when items are added or removed</summary>
  /// <typeparam name="TItem">Type of items the collection manages</typeparam>
  public class ObservableCollection<TItem> :
    Collection<TItem>,
#if !NO_SPECIALIZED_COLLECTIONS
    INotifyCollectionChanged,
#endif
    IObservableCollection<TItem> {

    /// <summary>Raised when an item has been added to the collection</summary>
    public event EventHandler<ItemEventArgs<TItem>> ItemAdded;
    /// <summary>Raised when an item is removed from the collection</summary>
    public event EventHandler<ItemEventArgs<TItem>> ItemRemoved;
    /// <summary>Raised when the collection is about to be cleared</summary>
    /// <remarks>
    ///   This could be covered by calling ItemRemoved for each item currently
    ///   contained in the collection, but it is often simpler and more efficient
    ///   to process the clearing of the entire collection as a special operation.
    /// </remarks>
    public event EventHandler Clearing;
    /// <summary>Raised when the collection has been cleared</summary>
    public event EventHandler Cleared;

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Called when the collection has changed</summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
#endif

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class that is empty.
    /// </summary>
    public ObservableCollection() : base() { }

    /// <summary>
    ///   Initializes a new instance of the ObservableCollection class as a wrapper
    ///   for the specified list.
    /// </summary>
    /// <param name="list">The list that is wrapped by the new collection.</param>
    /// <exception cref="System.ArgumentNullException">List is null</exception>
    public ObservableCollection(IList<TItem> list) : base(list) { }

    /// <summary>Removes all elements from the Collection</summary>
    protected override void ClearItems() {
      OnClearing();
      base.ClearItems();
      OnCleared();
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Reset, default(TItem), -1);
#endif
    }

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fires the CollectionChanged event</summary>
    /// <param name="action">Type of change that has occured</param>
    /// <param name="item">The item that has been added, removed or replaced</param>
    /// <param name="index">Index of the changed item</param>
    protected virtual void OnCollectionChanged(
      NotifyCollectionChangedAction action, TItem item, int index
    ) {
      if(CollectionChanged != null) {
        CollectionChanged(
          this, new NotifyCollectionChangedEventArgs(action, item, index)
         );
      }
    }
#endif

    /// <summary>
    ///   Inserts an element into the ObservableCollection at the specified index
    /// </summary>
    /// <param name="index">
    ///   The object to insert. The value can be null for reference types.
    /// </param>
    /// <param name="item">The zero-based index at which item should be inserted</param>
    protected override void InsertItem(int index, TItem item) {
      base.InsertItem(index, item);
      OnAdded(item);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
#endif
    }

    /// <summary>
    ///   Removes the element at the specified index of the ObservableCollection
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove</param>
    protected override void RemoveItem(int index) {
      TItem item = base[index];
      base.RemoveItem(index);
      OnRemoved(item);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
#endif
    }

    /// <summary>Replaces the element at the specified index</summary>
    /// <param name="index">
    ///   The new value for the element at the specified index. The value can be null
    ///   for reference types
    /// </param>
    /// <param name="item">The zero-based index of the element to replace</param>
    protected override void SetItem(int index, TItem item) {
      TItem oldItem = base[index];
      base.SetItem(index, item);
      OnRemoved(oldItem);
      OnAdded(item);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Replace, item, index);
#endif
    }

    /// <summary>Fires the 'ItemAdded' event</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(TItem item) {
      if(ItemAdded != null)
        ItemAdded(this, new ItemEventArgs<TItem>(item));
    }

    /// <summary>Fires the 'ItemRemoved' event</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(TItem item) {
      if(ItemRemoved != null)
        ItemRemoved(this, new ItemEventArgs<TItem>(item));
    }

    /// <summary>Fires the 'Clearing' event</summary>
    protected virtual void OnClearing() {
      if(Clearing != null)
        Clearing(this, EventArgs.Empty);
    }

    /// <summary>Fires the 'Cleared' event</summary>
    protected virtual void OnCleared() {
      if(Cleared != null)
        Cleared(this, EventArgs.Empty);
    }

  }

} // namespace Nuclex.Support.Collections
