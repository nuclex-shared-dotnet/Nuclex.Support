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

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Operation that sequentially executes a series of operations</summary>
  /// <typeparam name="OperationType">
  ///   Type of the child operations the QueueOperation will contain
  /// </typeparam>
  public class QueueOperation<OperationType> : Operation
    where OperationType : Operation {

    /// <summary>Initializes a new queue operation</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    /// <remarks>
    ///   All child operations will have a default weight of 1.0
    /// </remarks>
    public QueueOperation(IEnumerable<OperationType> childs) {
      this.setProgression = new SetProgression<OperationType>(childs);
    }

    /// <summary>Initializes a new queue operation with custom weights</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    public QueueOperation(IEnumerable<WeightedProgression<OperationType>> childs) {
      this.setProgression = new SetProgression<OperationType>(childs);
    }
    
    /// <summary>Launches the background operation</summary>
    public override void Begin() {
      this.setProgression.AsyncProgressUpdated +=
        new EventHandler<ProgressUpdateEventArgs>(setProgressionProgressUpdated);

      

      //this.setProgression.Childs[0].Progression.Begin();
    }

    private void setProgressionProgressUpdated(object sender, ProgressUpdateEventArgs e) {
      throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>SetProgression used internally to handle progress reports</summary>
    private volatile SetProgression<OperationType> setProgression;

  }

} // namespace Nuclex.Support.Scheduling
