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
  public class ParentingCollection<ParentType, ItemType> : Collection<ItemType>
    where ItemType : Parentable<ParentType> {

    /// <summary>Reparents all elements in the collection</summary>
    /// <param name="parent">New parent to take ownership of the items</param>
    protected void Reparent(ParentType parent) {
      this.parent = parent;

      for(int index = 0; index < Count; ++index)
        base[index].SetParent(parent);
    }

    /// <summary>Clears all elements from the collection</summary>
    protected override void ClearItems() {
      for(int index = 0; index < Count; ++index)
        base[index].SetParent(default(ParentType));

      base.ClearItems();
    }

    /// <summary>Inserts a new element into the collection</summary>
    /// <param name="index">Index at which to insert the element</param>
    /// <param name="item">Item to be inserted</param>
    protected override void InsertItem(int index, ItemType item) {
      base.InsertItem(index, item);
      item.SetParent(this.parent);
    }

    /// <summary>Removes an element from the collection</summary>
    /// <param name="index">Index of the element to remove</param>
    protected override void RemoveItem(int index) {
      base[index].SetParent(default(ParentType));
      base.RemoveItem(index);
    }

    /// <summary>Takes over a new element that is directly assigned</summary>
    /// <param name="index">Index of the element that was assigned</param>
    /// <param name="item">New item</param>
    protected override void SetItem(int index, ItemType item) {
      base.SetItem(index, item);
      item.SetParent(this.parent);
    }

    /// <summary>Disposes the collection and optionally all items contained therein</summary>
    /// <param name="disposeItems">Whether to try calling Dispose() on all items</param>
    /// <remarks>
    ///   This method is intended to support collections that need to dispose of their
    ///   items. The ParentingCollection will first detach all items from the parent
    ///   object and then call Dispose() on any item that implements IDisposable.
    /// </remarks>
    protected void InternalDispose(bool disposeItems) {

      if(disposeItems) {

        // Have the items do their cleanup work
        //Reparent(default(ParentType));

        // Dispose of all the items in the collection that implement IDisposable
        foreach(ItemType item in this) {
          IDisposable disposable = item as IDisposable;

          // If the item is disposable, we get rid of it
          if(disposable != null)
            disposable.Dispose();
          /*else
            Reparent(default(ParentType));*/
          
        }

      }

      // Remove all items from the collection
      base.ClearItems();

    }

    /// <summary>Parent this collection currently belongs to</summary>
    private ParentType parent;

  }

} // namespace Nuclex.Support.Collections
