using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  partial class Deque<ItemType> {

    /// <summary>Removes the first item in the double-ended queue</summary>
    public void RemoveFirst() {
      if(this.count == 0) {
        throw new InvalidOperationException("Cannot remove items from empty deque");
      }

      // TODO: Zero removed array entry if array is kept

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
    }

    /// <summary>Removes the last item in the double-ended queue</summary>
    public void RemoveLast() {
      if(this.count == 0) {
        throw new InvalidOperationException("Cannot remove items from empty deque");
      }

      // TODO: Zero removed array entry if array is kept

      --this.lastBlockEndIndex;
      if(this.lastBlockEndIndex == 0) { // Block became empty
        if(this.count > 1) {
          this.blocks.RemoveAt(this.blocks.Count - 1);
          this.lastBlockEndIndex = this.blockSize;
        } else { // Last block - do not remove
          this.firstBlockStartIndex = 0;
          this.lastBlockEndIndex = 0;
        }
      }
      --this.count;
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
    }

    /// <summary>
    ///   Removes an item from the left side of the queue by shifting all items that
    ///   come before it to the right by one
    /// </summary>
    /// <param name="index">Index of the item that will be removed</param>
    private void removeFromLeft(int index) {
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
          this.blocks[lastBlock][this.lastBlockEndIndex - 1] = default(ItemType);
          --this.lastBlockEndIndex;
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
          this.blocks[lastBlock][this.lastBlockEndIndex - 1] = default(ItemType);
          --this.lastBlockEndIndex;
        }

        --this.count;
      }
    }

  }

} // namespace Nuclex.Support.Collections
