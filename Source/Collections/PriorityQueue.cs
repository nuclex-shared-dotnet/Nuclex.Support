using System;
using System.Collections.Generic;
using System.Collections;

namespace Nuclex.Support.Collections {
/*
  /// <summary>Queue that dequeues items in order of their priority</summary>
  public class PriorityQueue<ItemType> : ICollection, IEnumerable<ItemType> {

    public static PriorityQueue Syncronized(PriorityQueue<ItemType> P) {
      return new PriorityQueue<ItemType>(ArrayList.Synchronized(P.InnerList), P.Comparer, false);
    }
    public static PriorityQueue<ItemType> ReadOnly(PriorityQueue<ItemType> P) {
      return new PriorityQueue(ArrayList.ReadOnly(P.InnerList), P.Comparer, false);
    }

    public PriorityQueue()
      : this(Comparer.Default) { }

    public PriorityQueue(int C)
      : this(Comparer.Default, C) { }

    public PriorityQueue(IComparer c) {
      Comparer = c;
    }

    public PriorityQueue(IComparer c, int Capacity) {
      Comparer = c;
      InnerList.Capacity = Capacity;
    }

    protected PriorityQueue(ArrayList Core, IComparer Comp, bool Copy) {
      if(Copy)
        InnerList = Core.Clone() as ArrayList;
      else
        InnerList = Core;
      Comparer = Comp;
    }

    protected void SwitchElements(int i, int j) {
      object h = InnerList[i];
      InnerList[i] = InnerList[j];
      InnerList[j] = h;
    }

    protected virtual int OnCompare(int i, int j) {
      return Comparer.Compare(InnerList[i], InnerList[j]);
    }

    #region public methods
    /// <summary>
    /// Push an object onto the PQ
    /// </summary>
    /// <param name="O">The new object</param>
    /// <returns>
    ///   The index in the list where the object is _now_. This will change when
    ///   objects are taken from or put onto the PQ.
    /// </returns>
    public int Queue(object O) {
      int p = InnerList.Count, p2;
      InnerList.Add(O); // E[p] = O
      do {
        if(p == 0)
          break;
        p2 = (p - 1) / 2;
        if(OnCompare(p, p2) < 0) {
          SwitchElements(p, p2);
          p = p2;
        } else
          break;
      } while(true);
      return p;
    }

    /// <summary>
    /// Get the smallest object and remove it.
    /// </summary>
    /// <returns>The smallest object</returns>
    public object Dequeue() {
      object result = InnerList[0];
      int p = 0, p1, p2, pn;
      InnerList[0] = InnerList[InnerList.Count - 1];
      InnerList.RemoveAt(InnerList.Count - 1);
      do {
        pn = p;
        p1 = 2 * p + 1;
        p2 = 2 * p + 2;
        if(InnerList.Count > p1 && OnCompare(p, p1) > 0) // links kleiner
          p = p1;
        if(InnerList.Count > p2 && OnCompare(p, p2) > 0) // rechts noch kleiner
          p = p2;

        if(p == pn)
          break;
        SwitchElements(p, pn);
      } while(true);
      return result;
    }

    /// <summary>
    /// Notify the PQ that the object at position i has changed
    /// and the PQ needs to restore order.
    /// Since you dont have access to any indexes (except by using the
    /// explicit IList.this) you should not call this function without knowing exactly
    /// what you do.
    /// </summary>
    /// <param name="i">The index of the changed object.</param>
    public void Update(int i) {
      int p = i, pn;
      int p1, p2;
      do	// aufsteigen
			{
        if(p == 0)
          break;
        p2 = (p - 1) / 2;
        if(OnCompare(p, p2) < 0) {
          SwitchElements(p, p2);
          p = p2;
        } else
          break;
      } while(true);
      if(p < i)
        return;
      do	   // absteigen
			{
        pn = p;
        p1 = 2 * p + 1;
        p2 = 2 * p + 2;
        if(InnerList.Count > p1 && OnCompare(p, p1) > 0) // links kleiner
          p = p1;
        if(InnerList.Count > p2 && OnCompare(p, p2) > 0) // rechts noch kleiner
          p = p2;

        if(p == pn)
          break;
        SwitchElements(p, pn);
      } while(true);
    }

    /// <summary>
    /// Get the smallest object without removing it.
    /// </summary>
    /// <returns>The smallest object</returns>
    public object Peek() {
      if(InnerList.Count > 0)
        return InnerList[0];
      return null;
    }

    public bool Contains(object value) {
      return InnerList.Contains(value);
    }

    public void Clear() {
      InnerList.Clear();
    }

    public int Count {
      get {
        return InnerList.Count;
      }
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return InnerList.GetEnumerator();
    }

    public void CopyTo(Array array, int index) {
      InnerList.CopyTo(array, index);
    }

    public object Clone() {
      return new PriorityQueue(InnerList, Comparer, true);
    }

    public bool IsSynchronized {
      get {
        return InnerList.IsSynchronized;
      }
    }

    public object SyncRoot {
      get {
        return this;
      }
    }
    #endregion
    #region explicit implementation
    bool IList.IsReadOnly {
      get {
        return false;
      }
    }

    object IList.this[int index] {
      get {
        return InnerList[index];
      }
      set {
        InnerList[index] = value;
        Update(index);
      }
    }

    int IList.Add(object o) {
      return Queue(o);
    }

    void IList.RemoveAt(int index) {
      throw new NotSupportedException();
    }

    void IList.Insert(int index, object value) {
      throw new NotSupportedException();
    }

    void IList.Remove(object value) {
      throw new NotSupportedException();
    }

    int IList.IndexOf(object value) {
      throw new NotSupportedException();
    }

    bool IList.IsFixedSize {
      get {
        return false;
      }
    }
    #endregion

    protected ArrayList InnerList = new ArrayList();
    protected IComparer Comparer;
  }
*/
} // namespace Nuclex.Support.Collections
