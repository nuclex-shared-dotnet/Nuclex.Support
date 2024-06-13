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

  /// <summary>Wraps a Collection and prevents users from modifying it</summary>
  /// <typeparam name="TItem">Type of items to manage in the Collection</typeparam>
  public class ReadOnlyCollection<TItem> :
    ICollection<TItem>,
    ICollection {

    /// <summary>Initializes a new read-only Collection wrapper</summary>
    /// <param name="collection">Collection that will be wrapped</param>
    public ReadOnlyCollection(ICollection<TItem> collection) {
      this.typedCollection = collection;
      this.objectCollection = (collection as ICollection);
    }

    /// <summary>Determines whether the List contains the specified item</summary>
    /// <param name="item">Item that will be checked for</param>
    /// <returns>True if the specified item is contained in the List</returns>
    public bool Contains(TItem item) {
      return this.typedCollection.Contains(item);
    }

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    public void CopyTo(TItem[] array, int arrayIndex) {
      this.typedCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>The number of items current contained in the List</summary>
    public int Count {
      get { return this.typedCollection.Count; }
    }

    /// <summary>Whether the List is write-protected</summary>
    public bool IsReadOnly {
      get { return true; }
    }

    /// <summary>Returns a new enumerator over the contents of the List</summary>
    /// <returns>The new List contents enumerator</returns>
    public IEnumerator<TItem> GetEnumerator() {
      return this.typedCollection.GetEnumerator();
    }

    #region ICollection<> implementation

    /// <summary>Adds an item to the end of the List</summary>
    /// <param name="item">Item that will be added to the List</param>
    void ICollection<TItem>.Add(TItem item) {
      throw new NotSupportedException(
        "Adding items is not supported by the read-only List"
      );
    }

    /// <summary>Removes all items from the List</summary>
    void ICollection<TItem>.Clear() {
      throw new NotSupportedException(
        "Clearing is not supported by the read-only List"
      );
    }

    /// <summary>Removes the specified item from the List</summary>
    /// <param name="item">Item that will be removed from the List</param>
    /// <returns>True of the specified item was found in the List and removed</returns>
    bool ICollection<TItem>.Remove(TItem item) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only List"
      );
    }

    #endregion

    #region IEnumerable implementation

    /// <summary>Returns a new enumerator over the contents of the List</summary>
    /// <returns>The new List contents enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return this.objectCollection.GetEnumerator();
    }

    #endregion

    #region ICollection implementation

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="index">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    void ICollection.CopyTo(Array array, int index) {
      this.objectCollection.CopyTo(array, index);
    }

    /// <summary>Whether the List is synchronized for multi-threaded usage</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectCollection.IsSynchronized; }
    }

    /// <summary>Synchronization root on which the List locks</summary>
    object ICollection.SyncRoot {
      get { return this.objectCollection.SyncRoot; }
    }

    #endregion

    /// <summary>The wrapped Collection under its type-safe interface</summary>
    private ICollection<TItem> typedCollection;
    /// <summary>The wrapped Collection under its object interface</summary>
    private ICollection objectCollection;

  }

} // namespace Nuclex.Support.Collections
