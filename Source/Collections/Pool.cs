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

namespace Nuclex.Support.Collections {

  /// <summary>Pool that recycles objects in order to avoid garbage build-up</summary>
  /// <typeparam name="TItem">Type of objects being pooled</typeparam>
  /// <remarks>
  ///   <para>
  ///     Use this class to recycle objects instead of letting them become garbage,
  ///     creating new instances each time. The Pool class is designed to either be
  ///     used on its own or as a building block for a static class that wraps it.
  ///   </para>
  ///   <para>
  ///     Special care has to be taken to revert the entire state of a recycled
  ///     object when it is returned to the pool. For example, events will need to
  ///     have their subscriber lists emptied to avoid sending out events to the
  ///     wrong subscribers and accumulating more and more subscribers each time
  ///     they are reused.
  ///   </para>
  ///   <para>
  ///     To simplify such cleanup, pooled objects can implement the IRecyclable
  ///     interface. When an object is returned to the pool, the pool will
  ///     automatically call its IRecyclable.Recycle() method.
  ///   </para>
  /// </remarks>
  public class Pool<TItem> {

    /// <summary>Default number of recyclable objects the pool will store</summary>
    public const int DefaultPoolSize = 64;

    /// <summary>Initializes a new pool using the default capacity</summary>
    public Pool() : this(DefaultPoolSize, null, null) { }

    /// <summary>Initializes a new pool using the default capacity</summary>
    /// <param name="createNewDelegate">Delegate that will be used to create new items</param>
    public Pool(Func<TItem> createNewDelegate) :
      this(DefaultPoolSize, createNewDelegate, null) { }

    /// <summary>Initializes a new pool using the default capacity</summary>
    /// <param name="createNewDelegate">Delegate that will be used to create new items</param>
    /// <param name="recycleDelegate">Delegate that will be used to recycle items</param>
    public Pool(Func<TItem> createNewDelegate, Action<TItem> recycleDelegate) :
      this(DefaultPoolSize, createNewDelegate, recycleDelegate) { }

    /// <summary>Initializes a new pool using a user-specified capacity</summary>
    /// <param name="capacity">Capacity of the pool</param>
    public Pool(int capacity) :
      this(capacity, null, null) { }

    /// <summary>Initializes a new pool using a user-specified capacity</summary>
    /// <param name="capacity">Capacity of the pool</param>
    /// <param name="createNewDelegate">Delegate that will be used to create new items</param>
    public Pool(int capacity, Func<TItem> createNewDelegate) :
      this(capacity, createNewDelegate, null) { }

    /// <summary>Initializes a new pool using a user-specified capacity</summary>
    /// <param name="capacity">Capacity of the pool</param>
    /// <param name="createNewDelegate">Delegate that will be used to create new items</param>
    /// <param name="recycleDelegate">Delegate that will be used to recycle items</param>
    public Pool(int capacity, Func<TItem> createNewDelegate, Action<TItem> recycleDelegate) {
      Capacity = capacity;

      if(createNewDelegate == null) {
        if(!typeof(TItem).HasDefaultConstructor()) {
          throw new ArgumentException(
            "Type " + typeof(TItem).Name + " has no default constructor and " +
            "requires a custom 'create instance' delegate"
          );
        }
        createNewDelegate = new Func<TItem>(Activator.CreateInstance<TItem>);
      }
      if(recycleDelegate == null) {
        recycleDelegate = new Action<TItem>(callRecycleIfSupported);
      }

      this.createNewDelegate = createNewDelegate;
      this.recycleDelegate = recycleDelegate;
    }

    /// <summary>
    ///   Returns a new or recycled instance of the types managed by the pool
    /// </summary>
    /// <returns>A new or recycled instance</returns>
    public TItem Get() {
      lock(this) {
        if(this.items.Count > 0) {
          return this.items.Dequeue();
        } else {
          return this.createNewDelegate();
        }
      }
    }

    /// <summary>
    ///   Redeems an instance that is no longer used to be recycled by the pool
    /// </summary>
    /// <param name="item">The instance that will be redeemed</param>
    public void Redeem(TItem item) {

      // Call Recycle() when the object is redeemed (instead of when it leaves
      // the pool again) in order to eliminate any references the object may hold
      // to other objects.
      this.recycleDelegate(item);

      lock(this) {
        if(this.items.Count < this.capacity) {
          this.items.Enqueue(item);
        }
      }

    }

    /// <summary>Number of objects the pool can retain</summary>
    /// <remarks>
    ///   Changing this value causes the pool to be emtpied. It is recommended that
    ///   you only read the pool's capacity, never change it.
    /// </remarks>
    public int Capacity {
      get { return this.capacity; }
      set {
        this.capacity = value;
        this.items = new Queue<TItem>(value);
      }
    }

    /// <summary>
    ///   Calls the Recycle() method on an objects if it implements
    ///   the IRecyclable interface
    /// </summary>
    /// <param name="item">
    ///   Object whose Recycle() method will be called if supported by the object
    /// </param>
    private static void callRecycleIfSupported(TItem item) {
      IRecyclable recycleable = item as IRecyclable;
      if(recycleable != null) {
        recycleable.Recycle();
      }
    }

    /// <summary>Objects being retained for recycling</summary>
    private Queue<TItem> items;
    /// <summary>Capacity of the pool</summary>
    /// <remarks>
    ///   Required because the Queue class doesn't allow this value to be retrieved
    /// </remarks>
    private int capacity;
    /// <summary>Delegate used to create new instances of the pool's type</summary>
    private Func<TItem> createNewDelegate;
    /// <summary>Delegate used to recycle instances</summary>
    private Action<TItem> recycleDelegate;

  }

} // namespace Nuclex.Support.Collections
