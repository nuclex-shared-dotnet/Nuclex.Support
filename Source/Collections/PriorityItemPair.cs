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
using System.Text;

namespace Nuclex.Support.Collections {

  /// <summary>An pair of a priority and an item</summary>
  public struct PriorityItemPair<TPriority, TItem> {

    /// <summary>Initializes a new priority / item pair</summary>
    /// <param name="priority">Priority of the item in the pair</param>
    /// <param name="item">Item to be stored in the pair</param>
    public PriorityItemPair(TPriority priority, TItem item) {
      this.Priority = priority;
      this.Item = item;
    }

    /// <summary>Priority assigned to this priority / item pair</summary>
    public TPriority Priority;
    /// <summary>Item contained in this priority / item pair</summary>
    public TItem Item;

    /// <summary>Converts the priority / item pair into a string</summary>
    /// <returns>A string describing the priority / item pair</returns>
    public override string ToString() {
      int length = 4;

      // Convert the priority value into a string or use the empty string
      // constant if the ToString() overload returns null
      string priorityString = this.Priority.ToString();
      if(priorityString != null)
        length += priorityString.Length;
      else
        priorityString = string.Empty;

      // Convert the item value into a string or use the empty string
      // constant if the ToString() overload returns null
      string itemString = this.Item.ToString();
      if(itemString != null)
        length += itemString.Length;
      else
        itemString = string.Empty;

      // Concatenate priority and item into a single string
      StringBuilder builder = new StringBuilder(length);
      builder.Append('[');
      builder.Append(priorityString);
      builder.Append(", ");
      builder.Append(itemString);
      builder.Append(']');
      return builder.ToString();
    }

  }

} // namespace Nuclex.Support.Collections
