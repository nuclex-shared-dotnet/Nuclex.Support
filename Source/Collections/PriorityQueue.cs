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
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  /// <summary>Queue that dequeues items in order of their priority</summary>
  public class PriorityQueue<TItem> : ICollection, IEnumerable<TItem> {

    #region class Enumerator

    /// <summary>Enumerates all items contained in a priority queue</summary>
    private class Enumerator : IEnumerator<TItem> {

      /// <summary>Initializes a new priority queue enumerator</summary>
      /// <param name="priorityQueue">Priority queue to be enumerated</param>
      public Enumerator(PriorityQueue<TItem> priorityQueue) {
        this.priorityQueue = priorityQueue;
        Reset();
      }

      /// <summary>Resets the enumerator to its initial state</summary>
      public void Reset() {
        this.index = -1;
#if DEBUG
        this.expectedVersion = this.priorityQueue.version;
#endif
      }

      /// <summary>The current item being enumerated</summary>
      TItem IEnumerator<TItem>.Current {
        get {
#if DEBUG
          checkVersion();
#endif
          return this.priorityQueue.heap[index];
        }
      }

      /// <summary>Moves to the next item in the priority queue</summary>
      /// <returns>True if a next item was found, false if the end has been reached</returns>
      public bool MoveNext() {
#if DEBUG
        checkVersion();
#endif
        if(this.index + 1 == this.priorityQueue.count)
          return false;

        ++this.index;

        return true;
      }

      /// <summary>Releases all resources used by the enumerator</summary>
      public void Dispose() { }

#if DEBUG
      /// <summary>Ensures that the priority queue has not changed</summary>
      private void checkVersion() {
        if(this.expectedVersion != this.priorityQueue.version)
          throw new InvalidOperationException("Priority queue has been modified");
      }
#endif

      /// <summary>The current item being enumerated</summary>
      object IEnumerator.Current {
        get {
#if DEBUG
          checkVersion();
#endif
          return this.priorityQueue.heap[index];
        }
      }

      /// <summary>Index of the current item in the priority queue</summary>
      private int index;
      /// <summary>The priority queue whose items this instance enumerates</summary>
      private PriorityQueue<TItem> priorityQueue;
#if DEBUG
      /// <summary>Expected version of the priority queue</summary>
      private int expectedVersion;
#endif

    }

    #endregion // class Enumerator

    /// <summary>
    ///   Initializes a new priority queue using IComparable for comparing items
    /// </summary>
    public PriorityQueue() : this(Comparer<TItem>.Default) { }

    /// <summary>Initializes a new priority queue</summary>
    /// <param name="comparer">Comparer to use for ordering the items</param>
    public PriorityQueue(IComparer<TItem> comparer) {
      this.comparer = comparer;
      this.capacity = 15; // 15 is equal to 4 complete levels
      this.heap = new TItem[this.capacity];
    }

    /// <summary>Returns the topmost item in the queue without dequeueing it</summary>
    /// <returns>The topmost item in the queue</returns>
    public TItem Peek() {
      if(this.count == 0) {
        throw new InvalidOperationException("No items queued");
      }

      return this.heap[0];
    }

    /// <summary>Takes the item with the highest priority off from the queue</summary>
    /// <returns>The item with the highest priority in the list</returns>
    /// <exception cref="InvalidOperationException">When the queue is empty</exception>
    public TItem Dequeue() {
      if(this.count == 0) {
        throw new InvalidOperationException("No items available to dequeue");
      }

      TItem result = this.heap[0];
      --this.count;
      trickleDown(0, this.heap[this.count]);
#if DEBUG
      ++this.version;
#endif
      return result;
    }

    /// <summary>Puts an item into the priority queue</summary>
    /// <param name="item">Item to be queued</param>
    public void Enqueue(TItem item) {
      if(this.count == capacity)
        growHeap();

      ++this.count;
      bubbleUp(this.count - 1, item);
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>Removes all items from the priority queue</summary>
    public void Clear() {
      this.count = 0;
#if DEBUG
      ++this.version;
#endif
    }


    /// <summary>Total number of items in the priority queue</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Copies the contents of the priority queue into an array</summary>
    /// <param name="array">Array to copy the priority queue into</param>
    /// <param name="index">Starting index for the destination array</param>
    public void CopyTo(Array array, int index) {
      Array.Copy(this.heap, 0, array, index, this.count);
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
    public IEnumerator<TItem> GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Moves an item upwards in the heap tree</summary>
    /// <param name="index">Index of the item to be moved</param>
    /// <param name="item">Item to be moved</param>
    private void bubbleUp(int index, TItem item) {
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
    private void trickleDown(int index, TItem item) {
      int child = getLeftChild(index);

      while(child < this.count) {

        bool needsToBeMoved =
          ((child + 1) < this.count) &&
          (this.comparer.Compare(heap[child], this.heap[child + 1]) < 0);

        if(needsToBeMoved)
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

      TItem[] newHeap = new TItem[this.capacity];
      Array.Copy(this.heap, 0, newHeap, 0, this.count);
      this.heap = newHeap;
    }

    /// <summary>Returns an enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Comparer used to order the items in the priority queue</summary>
    private IComparer<TItem> comparer;
    /// <summary>Total number of items in the priority queue</summary>
    private int count;
    /// <summary>Available space in the priority queue</summary>
    private int capacity;
    /// <summary>Tree containing the items in the priority queue</summary>
    private TItem[] heap;
#if DEBUG
    /// <summary>Incremented whenever the priority queue is modified</summary>
    private int version;
#endif

  }

} // namespace Nuclex.Support.Collections
