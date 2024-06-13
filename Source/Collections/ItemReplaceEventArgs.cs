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

  /// <summary>
  ///   Argument container used by collections to notify about replaced items
  /// </summary>
  public class ItemReplaceEventArgs<TItem> : EventArgs {

    /// <summary>Initializes a new event arguments supplier</summary>
    /// <param name="oldItem">Item that has been replaced by another item</param>
    /// <param name="newItem">Replacement item that is now part of the collection</param>
    public ItemReplaceEventArgs(TItem oldItem, TItem newItem) {
      this.oldItem = oldItem;
      this.newItem = newItem;
    }

    /// <summary>Item that has been replaced by another item</summary>
    public TItem OldItem {
      get { return this.oldItem; }
    }

    /// <summary>Replacement item that is now part of the collection</summary>
    public TItem NewItem {
      get { return this.newItem; }
    }

    /// <summary>Item that was removed from the collection</summary>
    private TItem oldItem;
    /// <summary>Item that was added to the collection</summary>
    private TItem newItem;

  }

} // namespace Nuclex.Support.Collections
