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

namespace Nuclex.Support.Collections {

  /// <summary>Interface for collections that can be observed</summary>
  /// <typeparam name="TItem">Type of items managed in the collection</typeparam>
  public interface IObservableCollection<TItem> {

    /// <summary>Raised when an item has been added to the collection</summary>
    event EventHandler<ItemEventArgs<TItem>> ItemAdded;

    /// <summary>Raised when an item is removed from the collection</summary>
    event EventHandler<ItemEventArgs<TItem>> ItemRemoved;

    /// <summary>Raised when an item is replaced in the collection</summary>
    event EventHandler<ItemReplaceEventArgs<TItem>> ItemReplaced;

    /// <summary>Raised when the collection is about to be cleared</summary>
    /// <remarks>
    ///   This could be covered by calling ItemRemoved for each item currently
    ///   contained in the collection, but it is often simpler and more efficient
    ///   to process the clearing of the entire collection as a special operation.
    /// </remarks>
    event EventHandler Clearing;

    /// <summary>Raised when the collection has been cleared of its items</summary>
    event EventHandler Cleared;

  }

} // namespace Nuclex.Support.Collections
