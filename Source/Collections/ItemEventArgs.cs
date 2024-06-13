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
  ///   Argument container used by collections to notify about changed items
  /// </summary>
  public class ItemEventArgs<TItem> : EventArgs {

    /// <summary>Initializes a new event arguments supplier</summary>
    /// <param name="item">Item to be supplied to the event handler</param>
    public ItemEventArgs(TItem item) {
      this.item = item;
    }

    /// <summary>Obtains the collection item the event arguments are carrying</summary>
    public TItem Item {
      get { return this.item; }
    }

    /// <summary>Item to be passed to the event handler</summary>
    private TItem item;

  }

} // namespace Nuclex.Support.Collections
