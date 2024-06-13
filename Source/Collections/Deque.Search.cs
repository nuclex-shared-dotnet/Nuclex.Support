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

  partial class Deque<TItem> {

    /// <summary>
    ///   Determines the index of the first occurence of the specified item in the deque
    /// </summary>
    /// <param name="item">Item that will be located in the deque</param>
    /// <returns>The index of the item or -1 if it wasn't found</returns>
    public int IndexOf(TItem item) {
      if(this.blocks.Count == 1) { // Only one block to scan?
        int length = this.lastBlockEndIndex - this.firstBlockStartIndex;
        int index = Array.IndexOf<TItem>(
          this.blocks[0], item, this.firstBlockStartIndex, length
        );

        // If we found something, we need to adjust its index so the first item in
        // the deque always appears at index 0 to the user
        if(index != -1) {
          return (index - this.firstBlockStartIndex);
        } else {
          return -1;
        }
      } else { // At least two blocks exist

        // Scan the first block for the item and if found, return the index
        int length = this.blockSize - this.firstBlockStartIndex;
        int index = Array.IndexOf<TItem>(
          this.blocks[0], item, this.firstBlockStartIndex, length
        );

        // If we found something, we need to adjust its index
        if(index != -1) {
          return (index - this.firstBlockStartIndex);
        }

        int lastBlock = this.blocks.Count - 1;
        for(int tempIndex = 1; tempIndex < lastBlock; ++tempIndex) {
          index = Array.IndexOf<TItem>(
            this.blocks[tempIndex], item, 0, this.blockSize
          );
          if(index != -1) {
            return (index - this.firstBlockStartIndex + tempIndex * this.blockSize);
          }
        }

        // Nothing found, continue the search in the 
        index = Array.IndexOf<TItem>(
          this.blocks[lastBlock], item, 0, this.lastBlockEndIndex
        );
        if(index == -1) {
          return -1;
        } else {
          return (index - this.firstBlockStartIndex + lastBlock * this.blockSize);
        }

      }
    }

  }

} // namespace Nuclex.Support.Collections
