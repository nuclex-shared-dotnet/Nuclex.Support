#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion
using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {

  /// <summary>Queue that dequeues items in order of their priority</summary>
  public class UnintrusivePriorityQueue<ItemType, PriorityType>
    : ICollection, IEnumerable<ItemType>
    where PriorityType : IComparable<PriorityType> {

    #region struct Entry

    /// <summary>An entry in the priority queue</summary>
    private struct Entry {

      /// <summary>Initializes a new priority queue entry</summary>
      /// <param name="item">Item to be stored in the entry</param>
      /// <param name="priority">Priority of the item in the entry</param>
      public Entry(ItemType item, PriorityType priority) {
        this.Item = item;
        this.Priority = priority;
      }

      /// <summary>Item contained in this priority queue entry</summary>
      public ItemType Item;
      /// <summary>Priority assigned to this entry</summary>
      public PriorityType Priority;

    }

    #endregion // struct Entry

    #region class EntryComparer

    /// <summary>Compares two priority queue entries based on their priority</summary>
    private class EntryComparer : IComparer<Entry> {

      /// <summary>Compares the left entry to the right entry</summary>
      /// <param name="left">Entry on the left side</param>
      /// <param name="right">Entry on the right side</param>
      /// <returns>The relationship of the two entries</returns>
      public int Compare(Entry left, Entry right) {
        return left.Priority.CompareTo(right.Priority);
      }

      /// <summary>Default instance for this comparer</summary>
      public static readonly EntryComparer Instance = new EntryComparer();

    }

    #endregion // class EntryComparer

    #region class UnwrappingEnumerator

    /// <summary>Enumerates all items contained in a priority queue</summary>
    private class UnwrappingEnumerator : IEnumerator<ItemType> {

      /// <summary>Initializes a new priority queue enumerator</summary>
      /// <param name="entryEnumerator">Enumerator of entries to unwrap</param>
      public UnwrappingEnumerator(IEnumerator<Entry> entryEnumerator) {
        this.entryEnumerator = entryEnumerator;
      }

      /// <summary>Resets the enumerator to its initial state</summary>
      public void Reset() {
        this.entryEnumerator.Reset();
      }

      /// <summary>The current item being enumerated</summary>
      ItemType IEnumerator<ItemType>.Current {
        get { return this.entryEnumerator.Current.Item; }
      }

      /// <summary>Releases all resources used by the enumerator</summary>
      public void Dispose() {
        this.entryEnumerator.Dispose();
      }

      /// <summary>Moves to the next item in the priority queue</summary>
      /// <returns>True if a next item was found, false if the end has been reached</returns>
      public bool MoveNext() {
        return this.entryEnumerator.MoveNext();
      }

      /// <summary>The current item being enumerated</summary>
      object IEnumerator.Current {
        get { return this.entryEnumerator.Current.Item; }
      }

      /// <summary>Enumerator for entries to be unwrapped by this enumerator</summary>
      private IEnumerator<Entry> entryEnumerator;

    }

    #endregion // class Enumerator

    /// <summary>Initializes a new non-intrusive priority queue</summary>
    public UnintrusivePriorityQueue() {
      this.intrusiveQueue = new PriorityQueue<Entry>(EntryComparer.Instance);
    }

    /// <summary>Takes the item with the highest priority off from the queue</summary>
    /// <returns>The item with the highest priority in the list</returns>
    public ItemType Dequeue() {
      return this.intrusiveQueue.Dequeue().Item;
    }

    /// <summary>Puts an item into the priority queue</summary>
    /// <param name="item">Item to be queued</param>
    /// <param name="priority">Priority of the item to be queued</param>
    public void Enqueue(ItemType item, PriorityType priority) {
      this.intrusiveQueue.Enqueue(new Entry(item, priority));
    }

    /// <summary>Removes all items from the priority queue</summary>
    public void Clear() {
      this.intrusiveQueue.Clear();
    }

    /// <summary>Total number of items in the priority queue</summary>
    public int Count {
      get { return this.intrusiveQueue.Count; }
    }

    /// <summary>Copies the contents of the priority queue into an array</summary>
    /// <param name="array">Array to copy the priority queue into</param>
    /// <param name="index">Starting index for the destination array</param>
    public void CopyTo(Array array, int index) {

      Entry[] entries = new Entry[this.intrusiveQueue.Count];

      this.intrusiveQueue.CopyTo(array, 0);

      for(int entryIndex = 0; entryIndex < this.intrusiveQueue.Count; ++entryIndex)
        array.SetValue(entries[entryIndex], entryIndex + index);

    }

    /// <summary>
    ///   Obtains an object that can be used to synchronize accesses to the priority queue
    ///   from different threads
    /// </summary>
    public object SyncRoot {
      get { return this.intrusiveQueue.SyncRoot; }
    }

    /// <summary>Whether operations performed on this priority queue are thread safe</summary>
    public bool IsSynchronized {
      get { return this.intrusiveQueue.IsSynchronized; }
    }

    /// <summary>Returns a typesafe enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    public IEnumerator<ItemType> GetEnumerator() {
      return new UnwrappingEnumerator(this.intrusiveQueue.GetEnumerator());
    }

    /// <summary>Returns an enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return new UnwrappingEnumerator(this.intrusiveQueue.GetEnumerator());
    }

    /// <summary>Intrusive priority queue being wrapped by this class</summary>
    private PriorityQueue<Entry> intrusiveQueue;

  }

} // namespace Nuclex.Support.Collections
