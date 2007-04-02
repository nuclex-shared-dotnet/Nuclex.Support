using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  /// <summary>Queue that dequeues items in order of their priority</summary>
  public class PriorityQueue<ItemType> : ICollection, IEnumerable<ItemType> {

    #region class Enumerator

    /// <summary>Enumerates all items contained in a priority queue</summary>
    private class Enumerator : IEnumerator<ItemType> {

      /// <summary>Initializes a new priority queue enumerator</summary>
      /// <param name="priorityQueue">Priority queue to be enumerated</param>
      public Enumerator(PriorityQueue<ItemType> priorityQueue) {
        this.priorityQueue = priorityQueue;
        Reset();
      }

      /// <summary>Resets the enumerator to its initial state</summary>
      public void Reset() {
        index = -1;
        version = priorityQueue.version;
      }

      /// <summary>The current item being enumerated</summary>
      ItemType IEnumerator<ItemType>.Current {
        get {
          checkVersion();
          return priorityQueue.heap[index];
        }
      }

      /// <summary>Moves to the next item in the priority queue</summary>
      /// <returns>True if a next item was found, false if the end has been reached</returns>
      public bool MoveNext() {
        checkVersion();

        if(index + 1 == priorityQueue.count)
          return false;

        ++index;

        return true;
      }

      /// <summary>Releases all resources used by the enumerator</summary>
      public void Dispose() { }

      /// <summary>Ensures that the priority queue has not changed</summary>
      private void checkVersion() {
        if(version != priorityQueue.version)
          throw new InvalidOperationException("Priority queue has been modified");
      }

      /// <summary>The current item being enumerated</summary>
      object IEnumerator.Current {
        get {
          checkVersion();
          return priorityQueue.heap[index];
        }
      }

      /// <summary>Index of the current item in the priority queue</summary>
      private int index;
      /// <summary>The priority queue whose items this instance enumerates</summary>
      private PriorityQueue<ItemType> priorityQueue;
      /// <summary>Expected version of the priority queue</summary>
      private int version;

    }

    #endregion // class Enumerator

    /// <summary>Initializes a new priority queue</summary>
    /// <param name="comparer">Comparer to use for ordering the items</param>
    public PriorityQueue(IComparer<ItemType> comparer) {
      this.comparer = comparer;
      this.capacity = 15; // 15 is equal to 4 complete levels
      this.heap = new ItemType[this.capacity];
    }

    /// <summary>Takes the item with the highest priority off from the queue</summary>
    /// <returns>The item with the highest priority in the list</returns>
    /// <exception cref="InvalidOperationException">When the queue is empty</exception>
    public ItemType Dequeue() {
      if(count == 0)
        throw new InvalidOperationException("No items available to dequeue");

      ItemType result = this.heap[0];
      --this.count;
      trickleDown(0, heap[this.count]);

      ++this.version;

      return result;
    }

    /// <summary>Puts an item into the priority queue</summary>
    /// <param name="item">Item to be queued</param>
    public void Enqueue(ItemType item) {
      if(this.count == capacity)
        growHeap();

      ++this.count;
      bubbleUp(this.count - 1, item);
      ++this.version;
    }

    /// <summary>Removes all items from the priority queue</summary>
    public void Clear() {
      this.count = 0;
      ++this.version;
    }


    /// <summary>Total number of items in the priority queue</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Copies the contents of the priority queue into an array</summary>
    /// <param name="array">Array to copy the priority queue into</param>
    /// <param name="index">Starting index for the destination array</param>
    public void CopyTo(Array array, int index) {
      Array.Copy(heap, 0, array, index, this.count);
    }

    /// <summary>
    ///   Obtains an object that can be used to synchronize accesses to the priority queue
    ///   from different threads
    /// </summary>
    public object SyncRoot {
      get { return this; }
    }

    /// <summary>Whether operations performed on this priority queue are thread safe</summary>
    public bool IsSynchronized {
      get { return false; }
    }

    /// <summary>Returns a typesafe enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    public IEnumerator<ItemType> GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Moves an item upwards in the heap tree</summary>
    /// <param name="index">Index of the item to be moved</param>
    /// <param name="item">Item to be moved</param>
    private void bubbleUp(int index, ItemType item) {
      int parent = getParent(index);

      // Note: (index > 0) means there is a parent
      while((index > 0) && (this.comparer.Compare(this.heap[parent], item) < 0)) {
        this.heap[index] = this.heap[parent];
        index = parent;
        parent = getParent(index);
      }

      this.heap[index] = item;
    }

    /// <summary>Move the item downwards in the heap tree</summary>
    /// <param name="index">Index of the item to be moved</param>
    /// <param name="item">Item to be moved</param>
    private void trickleDown(int index, ItemType item) {
      int child = getLeftChild(index);

      while(child < this.count) {

        if(
          ((child + 1) < this.count) &&
          (this.comparer.Compare(heap[child], this.heap[child + 1]) < 0)
        )
          ++child;

        this.heap[index] = this.heap[child];
        index = child;
        child = getLeftChild(index);
      }

      bubbleUp(index, item);
    }

    /// <summary>Obtains the left child item in the heap tree</summary>
    /// <param name="index">Index of the item whose left child to return</param>
    /// <returns>The left child item of the provided parent item</returns>
    private int getLeftChild(int index) {
      return (index * 2) + 1;
    }

    /// <summary>Calculates the parent entry of the item on the heap</summary>
    /// <param name="index">Index of the item whose parent to calculate</param>
    /// <returns>The index of the parent to the specified item</returns>
    private int getParent(int index) {
      return (index - 1) / 2;
    }

    /// <summary>Increases the size of the priority collection's heap</summary>
    private void growHeap() {
      this.capacity = (capacity * 2) + 1;

      ItemType[] newHeap = new ItemType[this.capacity];
      Array.Copy(this.heap, 0, newHeap, 0, this.count);
      this.heap = newHeap;
    }

    /// <summary>Returns an enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Comparer used to order the items in the priority queue</summary>
    private IComparer<ItemType> comparer;
    /// <summary>Total number of items in the priority queue</summary>
    private int count;
    /// <summary>Available space in the priority queue</summary>
    private int capacity;
    /// <summary>Incremented whenever the priority queue is modified</summary>
    private int version;
    /// <summary>Tree containing the items in the priority queue</summary>
    private ItemType[] heap;

  }

} // namespace Nuclex.Support.Collections
