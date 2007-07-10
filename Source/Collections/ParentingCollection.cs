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
using System.IO;
using System.Reflection;

namespace Nuclex.Support.Collections {

  /// <summary>Collection that automatically assigns an owner to all its elements</summary>
  /// <remarks>
  ///   This collection automatically assigns a parent object to elements that
  ///   are managed in it. The elements have to derive from the Parentable&lt;&gt;
  ///   base class.
  /// </remarks>
  /// <typeparam name="ParentType">Type of the parent object to assign to items</typeparam>
  /// <typeparam name="ItemType">Type of the items being managed in the collection</typeparam>
  public class ParentingCollection<ParentType, ItemType> : AcquiringCollection<ItemType>
    where ItemType : Parentable<ParentType> {

    /// <summary>Reparents all elements in the collection</summary>
    /// <param name="parent">New parent to take ownership of the items</param>
    protected void Reparent(ParentType parent) {
      this.parent = parent;

      for(int index = 0; index < Count; ++index)
        base[index].SetParent(parent);
    }

    /// <summary>Called when an item has been added to the collection</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(ItemType item) {
      item.SetParent(this.parent);
    }

    /// <summary>Called when an item has been removed from the collection</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(ItemType item) {
      item.SetParent(default(ParentType));
    }

    /// <summary>Disposes the collection and optionally all items contained therein</summary>
    /// <remarks>
    ///   <para>
    ///     This method is intended to support collections that need to dispose of their
    ///     items. The ParentingCollection will first detach all items from the parent
    ///     object and then call Dispose() on any item that implements IDisposable.
    ///   </para>
    ///   <para>
    ///     The items contained in the collection are not disconnected from their parent
    ///     (eg. Reparent()ed to null) because this is out of the scope for the
    ///     ParentingCollection&lt;&gt; class to decide and provokes the potentially
    ///     risky situation that an item, when it is Dispose()d, might try to disconnect
    ///     some events or perform other maintenance work on its parent object that
    ///     would then be no longer available. If you wish to disconnect the items from
    ///     their parent prior to disposing them, add a Reparent(null); call before the
    ///     line with InternalDispose(true); in your custom Dispose() method.
    ///   </para>
    /// </remarks>
    protected void DisposeItems() {

      // Dispose all the items in the collection that implement IDisposable,
      // starting from the last item in the assumption that this is the fastest
      // way to empty a list without causing excessive shiftings in the array.
      for(int index = base.Count - 1; index >= 0; --index) {

        IDisposable disposable = base[index] as IDisposable;

        base.RemoveAt(index);

        // If the item is disposable, destroy it now
        if(disposable != null)
          disposable.Dispose();

      }

    }

    /// <summary>Parent this collection currently belongs to</summary>
    private ParentType parent;

  }

} // namespace Nuclex.Support.Collections
