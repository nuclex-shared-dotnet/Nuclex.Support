#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclex.Support {

  /// <summary>Alternative Thread pool providing one thread for each core</summary>
  /// <remarks>
  ///   <para>
  ///     Unlike the normal thread pool, the affine thread pool provides only as many
  ///     threads as there are CPU cores available on the current platform. This makes
  ///     it more suitable for tasks you want to spread across all available cpu cores
  ///     explicitely, however, it's not a good match if you just want to run a series
  ///     of tasks asynchronously.
  ///   </para>
  ///   <para>
  ///     Implementation based on original code provided by Stephen Toub
  ///     (stoub at microsoft ignorethis dot com)
  ///   </para>
  /// </remarks>
  public static class AffineThreadPool {

    /// <summary>Number of CPU cores available on the system</summary>
#if XBOX360
    public static readonly int CpuCores = 4;
#else
    public static readonly int CpuCores = Environment.ProcessorCount;
#endif

    /// <summary>Delegate used by the thread pool to handle assertion checks</summary>
    /// <param name="condition">Condition that will be asserted</param>
    /// <param name="message">Message explaining what causes the assertion to fail</param>
    /// <param name="details">
    ///   Detailed description of the exact cause of the assertion failure
    /// </param>
    public delegate void AssertionDelegate(
      bool condition, string message, string details
    );

    #region class Semaphore

    /// <summary>
    ///   Lightweight variant of Dijkstra's PV Semaphore semaphore based on
    ///   the Monitor class.
    /// </summary>
    /// <remarks>
    ///   Based on code by Stephen Toub (stoub at microsoft ignorethis dot com).
    /// </remarks>
    private class Semaphore {

#if false // The Thread Pool doesn't use this.
      /// <summary>Initializes the semaphore as a binary semaphore (mutex)</summary>
      public Semaphore() : this(1) { }
#endif

      /// <summary>Initializes the semaphore as a counting semaphore</summary>
      /// <param name="count">
      ///   Initial number of threads that can take out units from this semaphore
      /// </param>
      /// <exception cref="ArgumentException">
      ///   Thrown if the count argument is less than 1
      /// </exception>
      public Semaphore(int count) {
#if false // The Thread Pool doesn't make this mistake.
        if(count < 0) {
          throw new ArgumentException(
            "Semaphore must have a count of at least 0.", "count"
          );
        }
#endif
        this.count = count;
      }

      /// <summary>V the semaphore (add 1 unit to it).</summary>
      public void Release() { v(); }

      /// <summary>P the semaphore (take out 1 unit from it).</summary>
      public void WaitOne() { p(); }

      /// <summary>P the semaphore (take out 1 unit from it).</summary>
      private void p() {

        // Lock so we can work in peace. This works because lock is actually
        // built around Monitor.
        lock(this) {

          // Wait until a unit becomes available. We need to wait in a loop in case
          // someone else wakes up before us. This could happen if the Monitor.Pulse
          // statements were changed to Monitor.PulseAll statements in order to
          // introduce some randomness into the order in which threads are woken.
          while(this.count <= 0) {
            Monitor.Wait(this, Timeout.Infinite);
          }

          --this.count;

        }

      }

      /// <summary>V the semaphore (add 1 unit to it).</summary>
      private void v() {

        // Lock so we can work in peace. This works because lock is actually
        // built around Monitor.
        lock(this) {

          // Release our hold on the unit of control. Then tell everyone
          // waiting on this object that there is a unit available.
          ++this.count;
          Monitor.Pulse(this);

        }

      }

      /// <summary>
      ///   Resets the semaphore to the specified count. Should be used cautiously.
      /// </summary>
      public void Reset(int count) {
        lock(this) {
          this.count = count;
        }
      }

      /// <summary>The number of units alloted by this semaphore.</summary>
      private int count;

    }

    #endregion // class Semaphore

    #region class WaitingCallback

    /// <summary>Used to hold a callback delegate and the state for that delegate.</summary>
    private struct UserWorkItem {

      /// <summary>Initialize the callback holding object.</summary>
      /// <param name="callback">Callback delegate for the callback.</param>
      /// <param name="state">State with which to call the callback delegate.</param>
      public UserWorkItem(WaitCallback callback, object state) {
        this.Callback = callback;
        this.State = state;
      }

      /// <summary>Callback delegate for the callback.</summary>
      public WaitCallback Callback;
      /// <summary>State with which to call the callback delegate.</summary>
      public object State;

    }

    #endregion // class WaitingCallback

    /// <summary>Initializes the thread pool</summary>
    static AffineThreadPool() {

      // Create our thread stores; we handle synchronization ourself
      // as we may run into situations where multiple operations need to be atomic.
      // We keep track of the threads we've created just for good measure; not actually
      // needed for any core functionality.
      waitingCallbacks = new Queue<UserWorkItem>(CpuCores * 4);
      workerThreads = new List<Thread>(CpuCores);
      inUseThreads = 0;
      
      // We can only use these hardware thread indices on the XBox 360
#if XBOX360
      XboxHardwareThreads = new Queue<int>(new int[] { 5, 4, 3, 1 });
#endif

      // Create our "thread needed" event
      workerThreadNeeded = new Semaphore(0);

      // Create all of the worker threads
      for(int index = 0; index < CpuCores; index++) {

        // Create a new thread and add it to the list of threads.
        Thread newThread = new Thread(new ThreadStart(ProcessQueuedItems));
        workerThreads.Add(newThread);

        // Configure the new thread and start it
        newThread.Name = "Nuclex.Support.AffineThreadPool Thread #" + index.ToString();
        newThread.IsBackground = true;
        newThread.Start();
      }

    }

    /// <summary>Queues a user work item to the thread pool</summary>
    /// <param name="callback">
    ///   A WaitCallback representing the delegate to invoke when a thread in the 
    ///   thread pool picks up the work item
    /// </param>
    public static void QueueUserWorkItem(WaitCallback callback) {

      // Queue the delegate with no state
      QueueUserWorkItem(callback, null);

    }

    /// <summary>Queues a user work item to the thread pool.</summary>
    /// <param name="callback">
    ///   A WaitCallback representing the delegate to invoke when a thread in the 
    ///   thread pool picks up the work item
    /// </param>
    /// <param name="state">
    ///   The object that is passed to the delegate when serviced from the thread pool
    /// </param>
    public static void QueueUserWorkItem(WaitCallback callback, object state) {

      // Create a waiting callback that contains the delegate and its state.
      // Add it to the processing queue, and signal that data is waiting.
      UserWorkItem waiting = new UserWorkItem(callback, state);
      lock(waitingCallbacks) {
        waitingCallbacks.Enqueue(waiting);
      }

      // Decrement the semaphore into the negative range, so the worker threads will
      // be woken up until no more tasks are available.
      workerThreadNeeded.Release();

    }

    /// <summary>Empties the work queue of any queued work items</summary>
    public static void EmptyQueue() {
      lock(waitingCallbacks) {
        try {
          while(waitingCallbacks.Count > 0) {
            UserWorkItem callback = waitingCallbacks.Dequeue();
            IDisposable disposableState = callback.State as IDisposable;
            if(disposableState != null) {
              disposableState.Dispose();
            }
          }
        }
        catch(Exception) { // Make sure an error isn't thrown.
          AssertionHandler(
            false,
            "Unhandled exception disposing the state of a user work item",
            "The AffineThreadPool.EmptyQueue() method tried to dispose of any states" +
            "associated with waiting user work items. One of the states implementing" +
            "IDisposable threw an exception during Dispose()."
          );
        }

        // Clear all waiting items and reset the number of worker threads currently needed
        // to be 0 (there is nothing for threads to do)
        waitingCallbacks.Clear();
        workerThreadNeeded.Reset(0);
      }
    }

    /// <summary>Gets the number of threads at the disposal of the thread pool</summary>
    public static int MaxThreads { get { return CpuCores; } }

    /// <summary>Gets the number of currently active threads in the thread pool</summary>
    public static int ActiveThreads { get { return inUseThreads; } }

    /// <summary>
    ///   Gets the number of callback delegates currently waiting in the thread pool
    /// </summary>
    public static int WaitingCallbacks {
      get {
        lock(waitingCallbacks) {
          return waitingCallbacks.Count;
        }
      }
    }

    /// <summary>A thread worker function that processes items from the work queue</summary>
    private static void ProcessQueuedItems() {
#if XBOX360
      // MSDN states that SetProcessorAffinity() should be called from the thread
      // whose affinity is being changed.
      int hardwareThreadIndex;
      lock(XboxHardwareThreads) {
        hardwareThreadIndex = XboxHardwareThreads.Dequeue();
      }
      Thread.CurrentThread.SetProcessorAffinity(new int[] { hardwareThreadIndex });
#endif

      // Keep processing tasks indefinitely
      for(; ; ) {
        UserWorkItem workItem = getNextWorkItem();

        // Execute the work item we just picked up. Make sure to accurately
        // record how many callbacks are currently executing.
        try {
          Interlocked.Increment(ref inUseThreads);
          workItem.Callback(workItem.State);
        }
        catch(Exception) { // Make sure we don't throw here.
          AssertionHandler(
            false,
            "Unhandled exception in queued user work item",
            "An unhandled exception travelled up to the AffineThreadPool from" +
            "a queued user work item that was being executed"
          );
        }
        finally {
          Interlocked.Decrement(ref inUseThreads);
        }
      }
    }

    /// <summary>Obtains the next work item from the queue</summary>
    /// <returns>The next work item in the queue</returns>
    /// <remarks>
    ///   If the queue is empty, the call will block until an item is added to
    ///   the queue and the calling thread was the one picking it up.
    /// </remarks>
    private static UserWorkItem getNextWorkItem() {

      // Get the next item in the queue. If there is nothing there, go to sleep
      // for a while until we're woken up when a callback is waiting.
      for(; ; ) {

        // Try to get the next callback available.  We need to lock on the 
        // queue in order to make our count check and retrieval atomic.
        lock(waitingCallbacks) {
          if(waitingCallbacks.Count > 0) {
            return waitingCallbacks.Dequeue();
          }
        }

        // If we can't get one, go to sleep.
        workerThreadNeeded.WaitOne();

      }

    }

    /// <summary>Default assertion handler for the affine thread pool</summary>
    /// <param name="condition">Condition which is being asserted</param>
    /// <param name="message">Message explaining what causes the assertion to fail</param>
    /// <param name="details">
    ///   Detailed description of the exact cause of the assertion failure
    /// </param>
    public static void DefaultAssertionHandler(
      bool condition, string message, string details
    ) {
      Trace.Assert(condition, message, details);
    }

    /// <summary>Delegate used to handle assertion checks in the code</summary>
    public static AssertionDelegate AssertionHandler = DefaultAssertionHandler;

#if XBOX360
    /// <summary>XNA games on the XBox 360 can use only 4 of 6 hardware threads</summary>
    private static Queue<int> XboxHardwareThreads;
#endif
    /// <summary>Queue of all the callbacks waiting to be executed.</summary>
    private static Queue<UserWorkItem> waitingCallbacks;
    /// <summary>
    /// Used to signal that a worker thread is needed for processing.  Note that multiple
    /// threads may be needed simultaneously and as such we use a semaphore instead of
    /// an auto reset event.
    /// </summary>
    private static Semaphore workerThreadNeeded;
    /// <summary>List of all worker threads at the disposal of the thread pool.</summary>
    private static List<Thread> workerThreads;
    /// <summary>Number of threads currently active.</summary>
    private static int inUseThreads;

  }

} // namespace Nuclex.Support
