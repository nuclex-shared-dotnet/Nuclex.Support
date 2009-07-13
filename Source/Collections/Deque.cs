using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

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
  public partial class Deque<ItemType> /*: IList<ItemType>, IList*/ {

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
            this.blocks[1], item, 0, this.blockSize
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

} // namespace Nuclex.Support.Collections
