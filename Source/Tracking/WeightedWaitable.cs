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

namespace Nuclex.Support.Tracking {

  /// <summary>Waitable with an associated weight for the total progress</summary>
  public class WeightedWaitable<WaitableType> where WaitableType : Waitable {

    /// <summary>
    ///   Initializes a new weighted waitable with a default weight of 1.0
    /// </summary>
    /// <param name="waitable">Waitable whose progress to monitor</param>
    public WeightedWaitable(WaitableType waitable) : this(waitable, 1.0f) { }

    /// <summary>Initializes a new weighted waitable</summary>
    /// <param name="waitable">Waitable whose progress to monitor</param>
    /// <param name="weight">Weighting of the waitable's progress</param>
    public WeightedWaitable(WaitableType waitable, float weight) {
      this.waitable = waitable;
      this.weight = weight;
    }

    /// <summary>Waitable being wrapped by this weighted waitable</summary>
    public WaitableType Waitable {
      get { return this.waitable; }
    }

    /// <summary>The contribution of this waitable to the total progress</summary>
    public float Weight {
      get { return this.weight; }
    }

    /// <summary>Waitable whose progress we're tracking</summary>
    private WaitableType waitable;
    /// <summary>Weighting of this waitable in the total progress</summary>
    private float weight;

  }

} // namespace Nuclex.Support.Tracking
