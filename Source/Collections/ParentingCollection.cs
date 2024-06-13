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

using System;
using System.Collections.ObjectModel;

namespace Nuclex.Support.Collections {

  /// <summary>Collection that automatically assigns an owner to all its elements</summary>
  /// <remarks>
  ///   This collection automatically assigns a parent object to elements that
  ///   are managed in it. The elements have to derive from the Parentable&lt;&gt;
  ///   base class.
  /// </remarks>
  /// <typeparam name="TParent">Type of the parent object to assign to items</typeparam>
  /// <typeparam name="TItem">Type of the items being managed in the collection</typeparam>
  public class ParentingCollection<TParent, TItem> : Collection<TItem>
    where TItem : Parentable<TParent> {

    /// <summary>Reparents all elements in the collection</summary>
    /// <param name="parent">New parent to take ownership of the items</param>
    protected void Reparent(TParent parent) {
      this.parent = parent;

      for(int index = 0; index < Count; ++index)
        base[index].SetParent(parent);
    }

    /// <summary>Clears all elements from the collection</summary>
    protected override void ClearItems() {
      for(int index = 0; index < Count; ++index)
        base[index].SetParent(default(TParent));

      base.ClearItems();
    }

    /// <summary>Inserts a new element into the collection</summary>
    /// <param name="index">Index at which to insert the element</param>
    /// <param name="item">Item to be inserted</param>
    protected override void InsertItem(int index, TItem item) {
      base.InsertItem(index, item);
      item.SetParent(this.parent);
    }

    /// <summary>Removes an element from the collection</summary>
    /// <param name="index">Index of the element to remove</param>
    protected override void RemoveItem(int index) {
      base[index].SetParent(default(TParent));
      base.RemoveItem(index);
    }

    /// <summary>Takes over a new element that is directly assigned</summary>
    /// <param name="index">Index of the element that was assigned</param>
    /// <param name="item">New item</param>
    protected override void SetItem(int index, TItem item) {
      base[index].SetParent(default(TParent));
      base.SetItem(index, item);
      item.SetParent(this.parent);
    }

    /// <summary>Disposes all items contained in the collection</summary>
    /// <remarks>
    ///   <para>
    ///     This method is intended to support collections that need to dispose their
    ///     items. It will unparent all of the collection's items and call Dispose()
    ///     on any item that implements IDisposable.
    ///   </para>
    ///   <para>
    ///     Do not call this method from your destructor as it will access the
    ///     contained items in order to unparent and to Dispose() them, which leads
    ///     to undefined behavior since the object might have already been collected
    ///     by the GC. Call it only if your object is being manually disposed.
    ///   </para>
    /// </remarks>
    protected void DisposeItems() {

      // Dispose all the items in the collection that implement IDisposable,
      // starting from the last item in the assumption that this is the fastest
      // way to empty a list without causing excessive shiftings in the array.
      for(int index = base.Count - 1; index >= 0; --index) {
        IDisposable disposable = base[index] as IDisposable;

        // If the item is disposable, destroy it now
        if(disposable != null) {
          disposable.Dispose();
        }
      }

      base.ClearItems();

    }

    /// <summary>Parent this collection currently belongs to</summary>
    private TParent parent;

  }

} // namespace Nuclex.Support.Collections
