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
  public class ParentingCollection<ParentType, ItemType> : Collection<ItemType>, IDisposable
    where ItemType : Parentable<ParentType>
    where ParentType : class {

    /// <summary>Called when the object is garbage-collected</summary>
    ~ParentingCollection() {
      Dispose(false); // called from GC
    }

    /// <summary>Reparents all elements in the collection</summary>
    /// <param name="parent">New parent to take ownership of the items</param>
    protected void Reparent(ParentType parent) {
      this.parent = parent;

      for(int index = 0; index < Count; ++index)
        base[index].SetParent(parent);
    }

    /// <summary>Called when the asset needs to release its resources</summary>
    /// <param name="calledByUser">
    ///   Whether the mehod has been called from user code. If this argument
    ///   is false, the object is being disposed by the garbage collector and
    ///   it mustn't access other objects (including the attempt to Dispose() them)
    ///   as these might have already been destroyed by the GC.
    /// </param>
    protected virtual void Dispose(bool calledByUser) {

      // Only destroy the other resources when we're not being called from
      // the garbage collector, otherwise we'd risk accessing objects that
      // have already been disposed
      if(calledByUser) {

        // Have the items do their cleanup work
        Reparent(null);

        // Dispose of all the items in the collection
        foreach(ItemType item in this) {
          IDisposable disposable = item as IDisposable;
          if(disposable != null)
            disposable.Dispose();
        }

        // Remove all items from the collection
        base.ClearItems();

      }

    }

    /// <summary>Clears all elements from the collection</summary>
    protected override void ClearItems() {
      for(int index = 0; index < Count; ++index)
        base[index].SetParent(null);

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
      base[index].SetParent(null);
      base.RemoveItem(index);
    }

    /// <summary>Takes over a new element that is directly assigned</summary>
    /// <param name="index">Index of the element that was assigned</param>
    /// <param name="item">New item</param>
    protected override void SetItem(int index, ItemType item) {
      base.SetItem(index, item);
      item.SetParent(this.parent);
    }

    /// <summary>Release all resources owned by the instance explicitely</summary>
    void IDisposable.Dispose() {
      Dispose(true); // Called by user
    }

    /// <summary>Parent this collection currently belongs to</summary>
    private ParentType parent;

  }

} // namespace Nuclex.Support.Collections
