﻿using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  partial class Deque<ItemType> {

    /// <summary>
    ///   Determines the index of the first occurence of the specified item in the deque
    /// </summary>
    /// <param name="item">Item that will be located in the deque</param>
    /// <returns>The index of the item or -1 if it wasn't found</returns>
    public int IndexOf(ItemType item) {
      if(this.blocks.Count == 1) { // Only one block to scan?
        int length = this.lastBlockEndIndex - this.firstBlockStartIndex;
        int index = Array.IndexOf<ItemType>(
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
        int index = Array.IndexOf<ItemType>(
          this.blocks[0], item, this.firstBlockStartIndex, length
        );

        // If we found something, we need to adjust its index
        if(index != -1) {
          return (index - this.firstBlockStartIndex);
        }

        int lastBlock = this.blocks.Count - 1;
        for(int tempIndex = 1; tempIndex < lastBlock; ++tempIndex) {
          index = Array.IndexOf<ItemType>(
            this.blocks[tempIndex], item, 0, this.blockSize
          );
          if(index != -1) {
            return (index - this.firstBlockStartIndex + tempIndex * this.blockSize);
          }
        }

        // Nothing found, continue the search in the 
        index = Array.IndexOf<ItemType>(
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
