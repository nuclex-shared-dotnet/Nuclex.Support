using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

#if true

  /// <summary>A double-ended queue that allocates memory in blocks</summary>
  /// <typeparam name="ItemType">Type of the items being stored in the queue</typeparam>
  /// <remarks>
  ///   <para>
  ///     The double-ended queue allows items to be appended to either side of the queue
  ///     without a hefty toll on performance. Like its namesake in C++, it is implemented
  ///     using multiple arrays.
  ///   </para>
  ///   <para>
  ///     Therefore, it's not only good at coping with lists that are modified at their
  ///     beginning, but also at handling huge data sets since enlarging the deque doesn't
  ///     require items to be copied around and still can be accessed by index.
  ///   </para>
  /// </remarks>
  public class Deque<ItemType> /*: IList<ItemType>, IList*/ {

    /// <summary>Initializes a new deque</summary>
    public Deque() : this(512) { }

    /// <summary>Initializes a new deque using the specified block size</summary>
    /// <param name="blockSize">Size of the individual memory blocks used</param>
    public Deque(int blockSize) {
      this.blockSize = blockSize;

      this.blocks = new List<ItemType[]>();
      this.blocks.Add(new ItemType[this.blockSize]);
    }

    /// <summary>Number of items contained in the double ended queue</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Accesses an item by its index</summary>
    /// <param name="index">Index of the item that will be accessed</param>
    /// <returns>The item at the specified index</returns>
    public ItemType this[int index] {
      get {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        return this.blocks[blockIndex][subIndex];
      }
      set {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        this.blocks[blockIndex][subIndex] = value;
      }
    }

    /// <summary>The first item in the double-ended queue</summary>
    public ItemType First {
      get {
        if(this.lastBlockEndIndex <= this.firstBlockStartIndex) {
          throw new InvalidOperationException("The deque is empty");
        }
        return this.blocks[0][this.firstBlockStartIndex];
      }
    }

    /// <summary>The last item in the double-ended queue</summary>
    public ItemType Last {
      get {
        if(this.lastBlockEndIndex <= this.firstBlockStartIndex) {
          throw new InvalidOperationException("The deque is empty");
        }
        return this.blocks[this.blocks.Count - 1][this.lastBlockEndIndex - 1];
      }
    }

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

    /// <summary>Inserts an item at the beginning of the double-ended queue</summary>
    /// <param name="item">Item that will be inserted into the queue</param>
    public void AddFirst(ItemType item) {
      if(this.firstBlockStartIndex > 0) {
        --this.firstBlockStartIndex;
      } else { // Need to allocate a new block
        this.blocks.Insert(0, new ItemType[this.blockSize]);
        this.firstBlockStartIndex = this.blockSize - 1;
      }

      this.blocks[0][this.firstBlockStartIndex] = item;
      ++this.count;
    }

    /// <summary>Appends an item to the end of the double-ended queue</summary>
    /// <param name="item">Item that will be appended to the queue</param>
    public void AddLast(ItemType item) {
      if(this.lastBlockEndIndex < this.blockSize) {
        ++this.lastBlockEndIndex;
      } else { // Need to allocate a new block
        this.blocks.Add(new ItemType[this.blockSize]);
        this.lastBlockEndIndex = 1;
      }

      this.blocks[this.blocks.Count - 1][this.lastBlockEndIndex - 1] = item;
      ++this.count;
    }

    /// <summary>Inserts the item at the specified index</summary>
    /// <param name="index">Index the item will be inserted at</param>
    /// <param name="item">Item that will be inserted</param>
    public void Insert(int index, ItemType item) {
      // TODO: Not perfect. Can shift to left or to right in a deque.
      //       Rewrite!
      if(index == this.count) {
        AddLast(item);
      } else {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        int lastBlock = this.blocks.Count - 1;
        int blockLength;

        // If the lastmost block is full, we need to add another block
        if(this.lastBlockEndIndex == this.blockSize) {
          this.blocks.Add(new ItemType[this.blockSize]);
          this.blocks[lastBlock + 1][0] = this.blocks[lastBlock][this.blockSize - 1];
          this.lastBlockEndIndex = 1;

          blockLength = this.blockSize - 1;
        } else {
          blockLength = this.lastBlockEndIndex;
          ++this.lastBlockEndIndex;
        }

        // If the insertion point is not in the lastmost block
        if(blockIndex != lastBlock) {
          Array.Copy(
            this.blocks[lastBlock], 0, this.blocks[lastBlock], 1, blockLength
          );
          this.blocks[lastBlock][0] = this.blocks[lastBlock - 1][this.blockSize - 1];

          // Move all the blocks following the insertion point to the right by one item.
          // If there are no blocks inbetween, this for loop will not run.
          for(int tempIndex = lastBlock - 1; tempIndex > blockIndex; --tempIndex) {
            Array.Copy(
              this.blocks[tempIndex], 0, this.blocks[tempIndex], 1, this.blockSize - 1
            );
            this.blocks[tempIndex][0] = this.blocks[tempIndex - 1][this.blockSize - 1];
          }

          blockLength = this.blockSize - 1;
        }

        // Finally, move the items in the block the insertion takes place in
        Array.Copy(
          this.blocks[blockIndex], subIndex,
          this.blocks[blockIndex], subIndex + 1,
          blockLength - subIndex
        );

        this.blocks[blockIndex][subIndex] = item;
        ++this.count;
      }
    }

    /// <summary>Removes the item at the specified index</summary>
    /// <param name="index">Index of the item that will be removed</param>
    public void RemoveAt(int index) {
      // TODO: Not perfect. Can shift to left or to right in a deque.
      //       Rewrite!
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

    /*
        public int IndexOf(ItemType item) {
          switch(this.blocks.Count) {

            // No block exist, so the item cannot be found
            case 0: {
              return -1;
            }

            // Only one block exists, start index and end index apply to the same block
            case 1: {
              int count = this.lastBlockEndIndex - this.firstBlockStartIndex;
              int index = Array.IndexOf<ItemType>(
                this.blocks[0], item, this.firstBlockStartIndex, count
              );

              // If we found something, we need to adjust its index so the first item in
              // the deque always appears at index 0 to the user
              if(index != -1) {
                return (index - this.firstBlockStartIndex);
              } else {
                return -1;
              }
            }

            // Two blocks exist, start index is in first block, end index in second block
            case 2: {

              // Scan the first block for the item and if found, return the index
              int count = this.blockSize - this.firstBlockStartIndex;
              int index = Array.IndexOf<ItemType>(
                this.blocks[0], item, this.firstBlockStartIndex, this.blockSize
              );

              // If we found something, we need to adjust its index
              if(index != -1) {
                return (index - this.firstBlockStartIndex);
              }

              // Nothing found, continue the search in the 
              index = Array.IndexOf<ItemType>(
                this.blocks[1], item, 0, this.lastBlockEndIndex
              );
              if(index == -1) {
                return -1;
              } else {
                return (index - this.firstBlockStartIndex + this.blockSize);
              }
            }

            default: {
              int count = this.blockSize - this.firstBlockStartIndex;
              int index = Array.IndexOf<ItemType>(
                this.blocks[0], item, this.firstBlockStartIndex, this.blockSize
              );
              return -1;
            }

          }
        }
    */
    /// <summary>Calculates the block index and local sub index of an entry</summary>
    /// <param name="index">Index of the entry that will be located</param>
    /// <param name="blockIndex">Index of the block the entry is contained in</param>
    /// <param name="subIndex">Local sub index of the entry within the block</param>
    private void findIndex(int index, out int blockIndex, out int subIndex) {
      if((index < 0) || (index >= this.count)) {
        throw new ArgumentOutOfRangeException("Index out of range", "index");
      }

      index += this.firstBlockStartIndex;
      blockIndex = Math.DivRem(index, this.blockSize, out subIndex);
    }

    /// <summary>Number if items currently stored in the deque</summary>
    private int count;
    /// <summary>Size of a single deque block</summary>
    private int blockSize;
    /// <summary>Memory blocks being used to store the deque's data</summary>
    private List<ItemType[]> blocks;
    /// <summary>Starting index of data in the first block</summary>
    private int firstBlockStartIndex;
    /// <summary>End index of data in the last block</summary>
    private int lastBlockEndIndex;

  }

#endif

} // namespace Nuclex.Support.Collections
