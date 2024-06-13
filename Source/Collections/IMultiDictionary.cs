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

  /// <summary>
  ///   Associative collection that can store several values under one key and vice versa
  /// </summary>
  /// <typeparam name="TKey">Type of keys used within the dictionary</typeparam>
  /// <typeparam name="TValue">Type of values stored in the dictionary</typeparam>
  public interface IMultiDictionary<TKey, TValue> :
    IDictionary<TKey, ICollection<TValue>>,
    IDictionary,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable {

    /// <summary>Adds a value into the dictionary under the provided key</summary>
    /// <param name="key">Key the value will be stored under</param>
    /// <param name="value">Value that will be stored under the specified key</param>
    void Add(TKey key, TValue value);

    /// <summary>Determines the number of values stored under the specified key</summary>
    /// <param name="key">Key whose values will be counted</param>
    /// <returns>The number of values stored under the specified key</returns>
    int CountValues(TKey key);

    /// <summary>
    ///   Removes the item with the specified key and value from the dictionary
    /// </summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <param name="value">Value of the item that will be removed</param>
    /// <returns>
    ///   True if the specified item was contained in the dictionary and was removed
    /// </returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    bool Remove(TKey key, TValue value);

    /// <summary>Removes all items with the specified key from the dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <returns>The number of items that have been removed from the dictionary</returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    int RemoveKey(TKey key);

  }

} // namespace Nuclex.Support.Collections
