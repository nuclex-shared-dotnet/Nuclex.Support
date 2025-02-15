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
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  partial class Deque<TItem> {

    #region IEnumerable members

    /// <summary>Obtains a new enumerator for the contents of the deque</summary>
    /// <returns>The new enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }

    #endregion

    #region IList Members

    /// <summary>Adds an item to the deque</summary>
    /// <param name="value">Item that will be added to the deque</param>
    /// <returns>The index at which the new item was added</returns>
    int IList.Add(object value) {
      verifyCompatibleObject(value);

      AddLast((TItem)value);
      return this.count - 1;
    }

    /// <summary>Checks whether the deque contains the specified item</summary>
    /// <param name="value">Item the deque will be scanned for</param>
    /// <returns>True if the deque contained the specified item</returns>
    bool IList.Contains(object value) {
      return isCompatibleObject(value) && Contains((TItem)value);
    }

    /// <summary>Determines the index of the item in the deque</summary>
    /// <param name="value">Item whose index will be determined</param>
    /// <returns>The index of the specified item in the deque</returns>
    int IList.IndexOf(object value) {
      if(isCompatibleObject(value)) {
        return IndexOf((TItem)value);
      } else {
        return -1;
      }
    }

    /// <summary>Inserts an item into the deque at the specified location</summary>
    /// <param name="index">Index at which the item will be inserted</param>
    /// <param name="value">Item that will be inserted</param>
    void IList.Insert(int index, object value) {
      verifyCompatibleObject(value);
      Insert(index, (TItem)value);
    }

    /// <summary>Whether the deque has a fixed size</summary>
    bool IList.IsFixedSize {
      get { return false; }
    }

    /// <summary>Whether the deque is read-only</summary>
    bool IList.IsReadOnly {
      get { return false; }
    }

    /// <summary>Removes the specified item from the deque</summary>
    /// <param name="value">Item that will be removed from the deque</param>
    void IList.Remove(object value) {
      if(isCompatibleObject(value)) {
        Remove((TItem)value);
      }
    }

    /// <summary>Accesses an item in the deque by its index</summary>
    /// <param name="index">Index of the item that will be accessed</param>
    /// <returns>The item at the specified index</returns>
    object IList.this[int index] {
      get { return this[index]; }
      set {
        verifyCompatibleObject(value);
        this[index] = (TItem)value;
      }
    }

    #endregion

    #region ICollection<ItemType> Members

    /// <summary>Adds an item into the deque</summary>
    /// <param name="item">Item that will be added to the deque</param>
    void ICollection<TItem>.Add(TItem item) {
      AddLast(item);
    }

    /// <summary>Whether the collection is read-only</summary>
    bool ICollection<TItem>.IsReadOnly {
      get { return false; }
    }

    #endregion

    #region ICollection Members

    /// <summary>Copies the contents of the deque into an array</summary>
    /// <param name="array">Array the contents of the deque will be copied into</param>
    /// <param name="index">Index at which writing into the array will begin</param>
    void ICollection.CopyTo(Array array, int index) {
      if(!(array is TItem[])) {
        throw new ArgumentException("Incompatible array type", "array");
      }

      CopyTo((TItem[])array, index);
    }

    /// <summary>Whether the deque is thread-synchronized</summary>
    bool ICollection.IsSynchronized {
      get { return false; }
    }

    /// <summary>Synchronization root of the instance</summary>
    object ICollection.SyncRoot {
      get { return this; }
    }

    #endregion

  }

} // namespace Nuclex.Support.Collections
