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
using System.Collections;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  /// <summary>Collection of weakly referenced objects</summary>
  /// <remarks>
  ///   This collection tries to expose the interface of a normal collection, but stores
  ///   objects as weak references. When an object is accessed, it can return null.
  ///   when the collection detects that one of its items was garbage collected, it
  ///   will silently remove that item.
  /// </remarks>
  public partial class WeakCollection<TItem> : IList<TItem>, IList
    where TItem : class {

    #region class UnpackingEnumerator

    /// <summary>
    ///   An enumerator that unpacks the items returned by an enumerator of the
    ///   weak reference collection into the actual item type on-the-fly.
    /// </summary>
    private class UnpackingEnumerator : IEnumerator<TItem> {

      /// <summary>Initializes a new unpacking enumerator</summary>
      /// <param name="containedTypeEnumerator">
      ///   Enumerator of the weak reference collection
      /// </param>
      public UnpackingEnumerator(
        IEnumerator<WeakReference<TItem>> containedTypeEnumerator
      ) {
        this.containedTypeEnumerator = containedTypeEnumerator;
      }

      /// <summary>Immediately releases all resources used by the instance</summary>
      public void Dispose() {
        this.containedTypeEnumerator.Dispose();
      }

      /// <summary>
      ///   The element in the collection at the current position of the enumerator.
      /// </summary>
      public TItem Current {
        get { return this.containedTypeEnumerator.Current.Target; }
      }

      /// <summary>Gets the current element in the collection.</summary>
      /// <returns>The current element in the collection.</returns>
      /// <exception cref="System.InvalidOperationException">
      ///   The enumerator is positioned before the first element of the collection
      ///   or after the last element.
      /// </exception>
      public bool MoveNext() {
        return this.containedTypeEnumerator.MoveNext();
      }

      /// <summary>
      ///   Sets the enumerator to its initial position, which is before the first element
      ///   in the collection.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      ///   The collection was modified after the enumerator was created.
      /// </exception>
      public void Reset() {
        this.containedTypeEnumerator.Reset();
      }

      /// <summary>The current element in the collection.</summary>
      /// <exception cref="System.InvalidOperationException">
      ///   The enumerator is positioned before the first element of the collection
      ///   or after the last element.
      /// </exception>
      object IEnumerator.Current {
        get { return Current; }
      }

      /// <summary>An enumerator from the wrapped collection</summary>
      private IEnumerator<WeakReference<TItem>> containedTypeEnumerator;

    }

    #endregion // class UnpackingEnumerator

    /// <summary>Initializes a new weak reference collection</summary>
    /// <param name="items">
    ///   Internal list of weak references that are unpacking when accessed through
    ///   the WeakCollection's interface.
    /// </param>
    public WeakCollection(IList<WeakReference<TItem>> items) :
      this(items, EqualityComparer<TItem>.Default) { }

    /// <summary>Initializes a new weak reference collection</summary>
    /// <param name="items">
    ///   Internal list of weak references that are unpacking when accessed through
    ///   the WeakCollection's interface.
    /// </param>
    /// <param name="comparer">
    ///   Comparer used to identify and compare items to each other
    /// </param>
    public WeakCollection(
      IList<WeakReference<TItem>> items, IEqualityComparer<TItem> comparer
    ) {
      this.items = items;
      this.comparer = comparer;
    }

    /// <summary>
    ///   Determines whether an element is in the WeakCollection
    /// </summary>
    /// <param name="item">
    ///   The object to locate in the WeakCollection. The value can be null.
    /// </param>
    /// <returns>
    ///   True if value is found in the WeakCollection; otherwise, false.
    /// </returns>
    /// <remarks>
    ///   The default implementation of this method is very unoptimized and will
    ///   enumerate all the items in the collection, transforming one after another
    ///   to check whether the transformed item matches the item the user was
    ///   looking for. It is recommended to provide a custom implementation of
    ///   this method, if possible.
    /// </remarks>
    public virtual bool Contains(TItem item) {
      return (IndexOf(item) != -1);
    }

    /// <summary>
    ///   Copies the entire WeakCollection to a compatible one-dimensional
    ///   System.Array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional System.Array that is the destination of the elements copied
    ///   from the WeakCollection. The System.Array must have zero-based indexing.
    /// </param>
    /// <param name="index">
    ///   The zero-based index in array at which copying begins.
    /// </param>
    /// <exception cref="System.ArgumentException">
    ///   Index is equal to or greater than the length of array or the number of elements
    ///   in the source WeakCollection is greater than the available space from index to
    ///   the end of the destination array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///   Array is null.
    /// </exception>
    public void CopyTo(TItem[] array, int index) {
      if(this.items.Count > (array.Length - index)) {
        throw new ArgumentException(
          "Array too small to fit the collection items starting at the specified index"
        );
      }

      for(int itemIndex = 0; itemIndex < this.items.Count; ++itemIndex) {
        array[itemIndex + index] = this.items[itemIndex].Target;
      }
    }

    /// <summary>Removes all items from the WeakCollection</summary>
    public void Clear() {
      this.items.Clear();
    }

    /// <summary>
    ///   Returns an enumerator that iterates through the WeakCollection.
    /// </summary>
    /// <returns>An enumerator or the WeakCollection.</returns>
    public IEnumerator<TItem> GetEnumerator() {
      return new UnpackingEnumerator(this.items.GetEnumerator());
    }

    /// <summary>
    ///   Searches for the specified object and returns the zero-based index of the
    ///   first occurrence within the entire WeakCollection.
    /// </summary>
    /// <param name="item">
    ///   The object to locate in the WeakCollection. The value can
    ///   be null for reference types.
    /// </param>
    /// <returns>
    ///   The zero-based index of the first occurrence of item within the entire
    ///   WeakCollection, if found; otherwise, -1.
    /// </returns>
    /// <remarks>
    ///   The default implementation of this method is very unoptimized and will
    ///   enumerate all the items in the collection, transforming one after another
    ///   to check whether the transformed item matches the item the user was
    ///   looking for. It is recommended to provide a custom implementation of
    ///   this method, if possible.
    /// </remarks>
    public int IndexOf(TItem item) {
      for(int index = 0; index < this.items.Count; ++index) {
        TItem itemAtIndex = this.items[index].Target;
        if((itemAtIndex == null) || (item == null)) {
          if(ReferenceEquals(item, itemAtIndex)) {
            return index;
          }
        } else {
          if(this.comparer.Equals(itemAtIndex, item)) {
            return index;
          }
        }
      }

      return -1;
    }

    /// <summary>
    ///   The number of elements contained in the WeakCollection instance
    /// </summary>
    public int Count {
      get { return this.items.Count; }
    }

    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///    Index is less than zero or index is equal to or greater than
    ///    WeakCollection.Count.
    /// </exception>
    public TItem this[int index] {
      get { return this.items[index].Target; }
      set { this.items[index] = new WeakReference<TItem>(value); }
    }

    /// <summary>
    ///   Removes the first occurrence of a specific object from the WeakCollection.
    /// </summary>
    /// <param name="item">The object to remove from the WeakCollection</param>
    /// <returns>
    ///   True if item was successfully removed from the WeakCollection; otherwise, false.
    /// </returns>
    public bool Remove(TItem item) {
      for(int index = 0; index < this.items.Count; ++index) {
        TItem itemAtIndex = this.items[index].Target;
        if((itemAtIndex == null) || (item == null)) {
          if(ReferenceEquals(item, itemAtIndex)) {
            this.items.RemoveAt(index);
            return true;
          }
        } else {
          if(this.comparer.Equals(item, itemAtIndex)) {
            this.items.RemoveAt(index);
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>Adds an item to the WeakCollection.</summary>
    /// <param name="item">The object to add to the WeakCollection</param>
    public void Add(TItem item) {
      this.items.Add(new WeakReference<TItem>(item));
    }

    /// <summary>Inserts an item to the WeakCollection at the specified index.</summary>
    /// <param name="index">
    ///   The zero-based index at which item should be inserted.
    /// </param>
    /// <param name="item">The object to insert into the WeakCollection</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   index is not a valid index in the WeakCollection.
    /// </exception>
    public void Insert(int index, TItem item) {
      this.items.Insert(index, new WeakReference<TItem>(item));
    }

    /// <summary>
    ///   Removes the WeakCollection item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the WeakCollection.
    /// </exception>
    public void RemoveAt(int index) {
      this.items.RemoveAt(index);
    }

    /// <summary>Whether the List is write-protected</summary>
    public bool IsReadOnly {
      get { return this.items.IsReadOnly; }
    }

    /// <summary>
    ///   Removes the items that have been garbage collected from the collection
    /// </summary>
    public void RemoveDeadItems() {
      int newCount = 0;

      // Eliminate all items that have been garbage collected by shifting
      for(int index = 0; index < this.items.Count; ++index) {
        if(this.items[index].IsAlive) {
          this.items[newCount] = this.items[index];
          ++newCount;
        }
      }

      // If any garbage collected items were found, resize the collection so
      // the space that became empty in the previous shifting process will be freed
      while(this.items.Count > newCount) {
        this.items.RemoveAt(this.items.Count - 1);
      }
    }

    /// <summary>Weak references to the items contained in the collection</summary>
    private IList<WeakReference<TItem>> items;
    /// <summary>Used to identify and compare items in the collection</summary>
    private IEqualityComparer<TItem> comparer;
    /// <summary>Synchronization root for threaded accesses to this collection</summary>
    private object syncRoot;

  }

} // namespace Nuclex.Support.Collections
