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

  partial class MultiDictionary<TKey, TValue> {

    /// <summary>
    ///   Provides access to the values stored in a multi dictionary as a collection
    /// </summary>
    private class ValueCollection : ICollection<TValue>, ICollection {

      #region class Enumerator

      /// <summary>Enumerates the values stored in a multi dictionary</summary>
      private class Enumerator : IEnumerator<TValue> {

        /// <summary>Initializes a new enumerator</summary>
        /// <param name="valueCollections">Value collections being enumerated</param>
        public Enumerator(ICollection<ICollection<TValue>> valueCollections) {
          this.valueCollections = valueCollections;

          Reset();
        }

        /// <summary>Immediately releases all resources owned by the instance</summary>
        public void Dispose() {
          if(this.currentValue != null) {
            this.currentValue.Dispose();
            this.currentValue = null;
          }
          if(this.currentCollection != null) {
            this.currentCollection.Dispose();
            this.currentCollection = null;
          }
        }

        /// <summary>The current value the enumerator is pointing at</summary>
        public TValue Current {
          get {
            if(this.currentValue == null) {
              throw new InvalidOperationException("Enumerator is not on a valid position");
            }

            return this.currentValue.Current;
          }
        }

        /// <summary>Advances the enumerator to the next item</summary>
        /// <returns>
        ///   True if there was a next item, false if the enumerator reached the end
        /// </returns>
        public bool MoveNext() {
          if(this.currentCollection == null) {
            return false;
          }

          for(; ; ) {

            // Try to move the enumerator in the current key's list to the next item
            if(this.currentValue != null) {
              if(this.currentValue.MoveNext()) {
                return true; // We found the next item
              } else {
                this.currentValue.Dispose();
              }
            }

            // Enumerator for the current key's list reached the end, go to the next key
            if(this.currentCollection.MoveNext()) {
              this.currentValue = this.currentCollection.Current.GetEnumerator();
            } else {
              this.currentValue = null; // Guaranteed to be disposed already
              this.currentCollection.Dispose();
              this.currentCollection = null;
              return false;
            }

          }
        }

        /// <summary>Resets the enumerator to its initial position</summary>
        public void Reset() {
          if(this.currentValue != null) {
            this.currentValue.Dispose();
            this.currentValue = null;
          }
          if(this.currentCollection != null) {
            this.currentCollection.Dispose();
          }
          this.currentCollection = valueCollections.GetEnumerator();
        }

        #region IEnumerator implementation

        /// <summary>The current entry the enumerator is pointing at</summary>
        object IEnumerator.Current {
          get { return Current; }
        }

        #endregion // IEnumerator implementation

        /// <summary>Value collections being enumerated</summary>
        private ICollection<ICollection<TValue>> valueCollections;
        /// <summary>The current value collection the enumerator is in</summary>
        private IEnumerator<ICollection<TValue>> currentCollection;
        /// <summary>Current value in the collection the enumerator is in</summary>
        private IEnumerator<TValue> currentValue;

      }

      #endregion // class Enumerator

      /// <summary>Initializes a new multi dictionary value collection</summary>
      /// <param name="dictionary">Dictionary whose values the collection represents</param>
      public ValueCollection(MultiDictionary<TKey, TValue> dictionary) {
        this.dictionary = dictionary;
        this.dictionaryAsICollection = (ICollection)dictionary;
      }

      /// <summary>Determines whether the collection contains a specific value</summary>
      /// <param name="item">Value for which the collection will be checked</param>
      /// <returns>True if the collection contains the specified value</returns>
      public bool Contains(TValue item) {
        foreach(ICollection<TValue> values in this.dictionary.Values) {
          if(values.Contains(item)) {
            return true;
          }
        }

        return false;
      }

      /// <summary>Copies the contents of the collection into an array</summary>
      /// <param name="array">Array the collection contents will be copied into</param>
      /// <param name="arrayIndex">
      ///   Starting index in the array where writing will begin
      /// </param>
      public void CopyTo(TValue[] array, int arrayIndex) {
        foreach(ICollection<TValue> values in this.dictionary.Values) {
          foreach(TValue value in values) {
            array[arrayIndex] = value;
            ++arrayIndex;
          }
        }
      }

      /// <summary>The number of values in the collection</summary>
      public int Count {
        get { return this.dictionary.count; }
      }

      /// <summary>Always true since the value collection is read-only</summary>
      public bool IsReadOnly {
        get { return true; }
      }

      /// <summary>Returns a new enumerator for the value collection</summary>
      /// <returns>A new enumerator for the value collection</returns>
      public IEnumerator<TValue> GetEnumerator() {
        return new Enumerator(this.dictionary.typedDictionary.Values);
      }

      #region IEnumerator implementation

      /// <summary>Returns a non-typesafe enumerator for the collection</summary>
      /// <returns>The non-typesafe collection enumerator</returns>
      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      #endregion // IEnumerator implementation

      #region ICollection<> implementation

      /// <summary>Throws a NotSupportedException</summary>
      /// <param name="item">Not used</param>
      void ICollection<TValue>.Add(TValue item) {
        throw new NotSupportedException(
          "Items cannot be added to a dictionary through its values collection"
        );
      }

      /// <summary>Throws a NotSupportedException</summary>
      void ICollection<TValue>.Clear() {
        throw new NotSupportedException(
          "The values collection of a dictionary cannot be cleared"
        );
      }

      /// <summary>Throws a NotSupportedException</summary>
      /// <param name="item">Not used</param>
      /// <returns>Nothing, since the method always throws an exception</returns>
      bool ICollection<TValue>.Contains(TValue item) {
        throw new NotImplementedException();
      }

      /// <summary>Not supported</summary>
      /// <param name="item">Item that will not be removed</param>
      /// <returns>Nothing because the method throws an exception</returns>
      bool ICollection<TValue>.Remove(TValue item) {
        throw new NotSupportedException(
          "Items cannot be removed from a dictionary through its values collection"
        );
      }

      #endregion ICollection<> implementation

      #region ICollection implementation

      /// <summary>Copies the contents of the collection into an array</summary>
      /// <param name="array">Array the collection's contents are copied into</param>
      /// <param name="arrayIndex">
      ///   Starting index in the array where writing will begin
      /// </param>
      void ICollection.CopyTo(Array array, int arrayIndex) {
        foreach(ICollection<TValue> values in this.dictionary.Values) {
          foreach(TValue value in values) {
            array.SetValue(value, arrayIndex);
            ++arrayIndex;
          }
        }
      }

      /// <summary>Whether the dictionary is thread-safe</summary>
      bool ICollection.IsSynchronized {
        get { return this.dictionaryAsICollection.IsSynchronized; }
      }

      /// <summary>
      ///   The synchronization root used by the dictionary for thread synchronization
      /// </summary>
      object ICollection.SyncRoot {
        get { return this.dictionaryAsICollection.SyncRoot; }
      }

      #endregion // ICollection implementation

      /// <summary>Dictionary whose values the collection represents</summary>
      private MultiDictionary<TKey, TValue> dictionary;
      /// <summary>The dictionary under its ICollection interface</summary>
      private ICollection dictionaryAsICollection;

    }

  }

} // namespace Nuclex.Support.Collections
