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

    /// <summary>Removes all items from the deque</summary>
    public void Clear() {
      if(this.blocks.Count > 1) { // Are there multiple blocks?

        // Clear the items in the first block to avoid holding on to references
        // in memory unreachable to the user
        for(int index = this.firstBlockStartIndex; index < this.blockSize; ++index) {
          this.blocks[0][index] = default(TItem);
        }

        // Remove any other blocks
        this.blocks.RemoveRange(1, this.blocks.Count - 1);

      } else { // Nope, only a single block exists

        // Clear the items in the block to release any reference we may be keeping alive
        for(
          int index = this.firstBlockStartIndex; index < this.lastBlockEndIndex; ++index
        ) {
          this.blocks[0][index] = default(TItem);
        }

      }

      // Reset the counters to restart the deque from scratch
      this.firstBlockStartIndex = 0;
      this.lastBlockEndIndex = 0;
      this.count = 0;
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>Removes the specified item from the deque</summary>
    /// <param name="item">Item that will be removed from the deque</param>
    /// <returns>True if the item was found and removed</returns>
    public bool Remove(TItem item) {
      int index = IndexOf(item);
      if(index == -1) {
        return false;
      }

      RemoveAt(index);
#if DEBUG
      ++this.version;
#endif
      return true;
    }

    /// <summary>Removes the first item in the double-ended queue</summary>
    public void RemoveFirst() {
      if(this.count == 0) {
        throw new InvalidOperationException("Cannot remove items from empty deque");
      }

      // This is necessary to make sure the deque doesn't hold dead objects alive
      // in unreachable spaces of its memory.
      this.blocks[0][this.firstBlockStartIndex] = default(TItem);

      // Cut off the item from the first block. If the block became empty and it's
      // not the last remaining block, remove it as well.
      ++this.firstBlockStartIndex;
      if(this.firstBlockStartIndex >= this.blockSize) { // Block became empty
        if(this.count > 1) { // Still more blocks in queue, remove block
          this.blocks.RemoveAt(0);
          this.firstBlockStartIndex = 0;
        } else { // Last block - do not remove
          this.firstBlockStartIndex = 0;
          this.lastBlockEndIndex = 0;
        }
      }
      --this.count;
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>Removes the last item in the double-ended queue</summary>
    public void RemoveLast() {
      if(this.count == 0) {
        throw new InvalidOperationException("Cannot remove items from empty deque");
      }

      // This is necessary to make sure the deque doesn't hold dead objects alive
      // in unreachable spaces of its memory.
      int lastBlock = this.blocks.Count - 1;
      this.blocks[lastBlock][this.lastBlockEndIndex - 1] = default(TItem);

      // Cut off the last item in the last block. If the block became empty and it's
      // not the last remaining block, remove it as well.
      --this.lastBlockEndIndex;
      if(this.lastBlockEndIndex == 0) { // Block became empty
        if(this.count > 1) {
          this.blocks.RemoveAt(lastBlock);
          this.lastBlockEndIndex = this.blockSize;
        } else { // Last block - do not remove
          this.firstBlockStartIndex = 0;
          this.lastBlockEndIndex = 0;
        }
      }
      --this.count;
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>Removes the item at the specified index</summary>
    /// <param name="index">Index of the item that will be removed</param>
    public void RemoveAt(int index) {
      int distanceToRightEnd = this.count - index;
      if(index < distanceToRightEnd) { // Are we closer to the left end?
        removeFromLeft(index);
      } else { // Nope, we're closer to the right end
        removeFromRight(index);
      }
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>
    ///   Removes an item from the left side of the queue by shifting all items that
    ///   come before it to the right by one
    /// </summary>
    /// <param name="index">Index of the item that will be removed</param>
    private void removeFromLeft(int index) {
      if(index == 0) {
        RemoveFirst();
      } else {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        int firstBlock = 0;
        int endIndex;

        if(blockIndex > firstBlock) {
          Array.Copy(
            this.blocks[blockIndex], 0,
            this.blocks[blockIndex], 1,
            subIndex
          );
          this.blocks[blockIndex][0] = this.blocks[blockIndex - 1][this.blockSize - 1];

          for(int tempIndex = blockIndex - 1; tempIndex > firstBlock; --tempIndex) {
            Array.Copy(
              this.blocks[tempIndex], 0,
              this.blocks[tempIndex], 1,
              this.blockSize - 1
            );
            this.blocks[tempIndex][0] = this.blocks[tempIndex - 1][this.blockSize - 1];
          }

          endIndex = this.blockSize - 1;
        } else {
          endIndex = subIndex;
        }

        Array.Copy(
          this.blocks[firstBlock], this.firstBlockStartIndex,
          this.blocks[firstBlock], this.firstBlockStartIndex + 1,
          endIndex - this.firstBlockStartIndex
        );

        if(this.firstBlockStartIndex == this.blockSize - 1) {
          this.blocks.RemoveAt(0);
          this.firstBlockStartIndex = 0;
        } else {
          this.blocks[0][this.firstBlockStartIndex] = default(TItem);
          ++this.firstBlockStartIndex;
        }

        --this.count;
      }
    }

    /// <summary>
    ///   Removes an item from the right side of the queue by shifting all items that
    ///   come after it to the left by one
    /// </summary>
    /// <param name="index">Index of the item that will be removed</param>
    private void removeFromRight(int index) {
      if(index == this.count - 1) {
        RemoveLast();
      } else {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        int lastBlock = this.blocks.Count - 1;
        int startIndex;

        if(blockIndex < lastBlock) {
          Array.Copy(
            this.blocks[blockIndex], subIndex + 1,
            this.blocks[blockIndex], subIndex,
            this.blockSize - subIndex - 1
          );
          this.blocks[blockIndex][this.blockSize - 1] = this.blocks[blockIndex + 1][0];

          for(int tempIndex = blockIndex + 1; tempIndex < lastBlock; ++tempIndex) {
            Array.Copy(
              this.blocks[tempIndex], 1,
              this.blocks[tempIndex], 0,
              this.blockSize - 1
            );
            this.blocks[tempIndex][this.blockSize - 1] = this.blocks[tempIndex + 1][0];
          }

          startIndex = 0;
        } else {
          startIndex = subIndex;
        }

        Array.Copy(
          this.blocks[lastBlock], startIndex + 1,
          this.blocks[lastBlock], startIndex,
          this.lastBlockEndIndex - startIndex - 1
        );

        if(this.lastBlockEndIndex == 1) {
          this.blocks.RemoveAt(lastBlock);
          this.lastBlockEndIndex = this.blockSize;
        } else {
          this.blocks[lastBlock][this.lastBlockEndIndex - 1] = default(TItem);
          --this.lastBlockEndIndex;
        }

        --this.count;
      }
    }

  }

} // namespace Nuclex.Support.Collections
