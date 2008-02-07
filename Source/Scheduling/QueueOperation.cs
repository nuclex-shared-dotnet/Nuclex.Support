#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Operation that sequentially executes a series of operations</summary>
  /// <typeparam name="OperationType">
  ///   Type of the child operations the QueueOperation will contain
  /// </typeparam>
  public class QueueOperation<OperationType> : Operation
    where OperationType : Operation {

    /// <summary>Initializes a new queue operation</summary>
    private QueueOperation() {
      this.asyncOperationEndedDelegate = new EventHandler(asyncOperationEnded);
      this.asyncOperationProgressUpdatedDelegate = new EventHandler<ProgressUpdateEventArgs>(
        asyncOperationProgressUpdated
      );

      this.children = new List<WeightedProgression<OperationType>>();
    }

    /// <summary>Initializes a new queue operation with default weights</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    /// <remarks>
    ///   All child operations will have a default weight of 1.0
    /// </remarks>
    public QueueOperation(IEnumerable<OperationType> childs) : this() {

      // Construct a WeightedProgression with the default weight for each
      // progression and wrap it in an ObservedProgression
      foreach(OperationType operation in childs)
        this.children.Add(new WeightedProgression<OperationType>(operation));

      // Since all progressions have a weight of 1.0, the total weight is
      // equal to the number of progressions in our list
      this.totalWeight = (float)this.children.Count;

    }

    /// <summary>Initializes a new queue operation with custom weights</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    public QueueOperation(IEnumerable<WeightedProgression<OperationType>> childs) : this() {

      // Construct an ObservedProgression around each of the WeightedProgressions
      foreach(WeightedProgression<OperationType> operation in childs) {
        this.children.Add(operation);

        // Sum up the total weight
        this.totalWeight += operation.Weight;
      }

    }

    /// <summary>Provides access to the child operations of this queue</summary>
    public IList<WeightedProgression<OperationType>> Children {
      get { return this.children; }
    }

    /// <summary>Launches the background operation</summary>
    public override void Start() {
      startCurrentOperation();
    }

    /// <summary>Prepares the current operation and calls its Begin() method</summary>
    /// <remarks>
    ///   This subscribes the queue to the events of to the current operation
    ///   and launches the operation by calling its Begin() method.
    /// </remarks>
    private void startCurrentOperation() {
      OperationType operation = this.children[this.currentOperationIndex].Progression;

      operation.AsyncEnded += this.asyncOperationEndedDelegate;
      operation.AsyncProgressUpdated += this.asyncOperationProgressUpdatedDelegate;

      operation.Start();
    }

    /// <summary>Disconnects from the current operation and calls its End() method</summary>
    /// <remarks>
    ///   This unsubscribes the queue from the current operation's events, calls End()
    ///   on the operation and, if the operation didn't have an exception to report,
    ///   counts up the accumulated progress of the queue.
    /// </remarks>
    private void endCurrentOperation() {
      OperationType operation = this.children[this.currentOperationIndex].Progression;

      // Disconnect from the operation's events
      operation.AsyncEnded -= this.asyncOperationEndedDelegate;
      operation.AsyncProgressUpdated -= this.asyncOperationProgressUpdatedDelegate;

      try {
        operation.Join();

        // Add the operations weight to the total amount of completed weight in the queue
        this.completedWeight += this.children[this.currentOperationIndex].Weight;

        // Trigger another progress update
        OnAsyncProgressUpdated(this.completedWeight / this.totalWeight);
      }
      catch(Exception exception) {
        SetException(exception);
      }
    }

    /// <summary>Called when the current executing operation ends</summary>
    /// <param name="sender">Operation that ended</param>
    /// <param name="e">Not used</param>
    private void asyncOperationEnded(object sender, EventArgs e) {

      // Unsubscribe from the current operation's events and update the
      // accumulating progress counter
      endCurrentOperation();

      // Only jump to the next operation if no exception occured
      if(OccuredException == null) {

        ++this.currentOperationIndex;

        // Execute the next operation unless we reached the end of the queue
        if(this.currentOperationIndex < this.children.Count) {
          startCurrentOperation();
          return;
        }

      }

      // Either an exception has occured or we reached the end of the operation
      // queue. In any case, we need to report that the operation is over.
      OnAsyncEnded();

    }

    /// <summary>Called when currently executing operation makes progress</summary>
    /// <param name="sender">Operation that has achieved progress</param>
    /// <param name="e">Not used</param>
    private void asyncOperationProgressUpdated(object sender, ProgressUpdateEventArgs e) {

      // Determine the completed weight of the currently executing operation
      float currentOperationCompletedWeight =
        e.Progress * this.children[this.currentOperationIndex].Weight;

      // Build the total normalized amount of progress for the queue
      float progress =
        (this.completedWeight + currentOperationCompletedWeight) / this.totalWeight;

      // Done, we can send the actual progress to any event subscribers
      OnAsyncProgressUpdated(progress);

    }

    /// <summary>Delegate to the asyncOperationEnded() method</summary>
    private EventHandler asyncOperationEndedDelegate;
    /// <summary>Delegate to the asyncOperationProgressUpdated() method</summary>
    private EventHandler<ProgressUpdateEventArgs> asyncOperationProgressUpdatedDelegate;
    /// <summary>Operations being managed in the queue</summary>
    private List<WeightedProgression<OperationType>> children;
    /// <summary>Summed weight of all operations in the queue</summary>
    private float totalWeight;
    /// <summary>Accumulated weight of the operations already completed</summary>
    private float completedWeight;
    /// <summary>Index of the operation currently executing</summary>
    private int currentOperationIndex;

  }

} // namespace Nuclex.Support.Scheduling
