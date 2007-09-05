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

namespace Nuclex.Support.Tracking {

  /// <summary>Progression with an associated weight for the total progress</summary>
  public class WeightedProgression<ProgressionType> where ProgressionType : Progression {

    /// <summary>
    ///   Initializes a new weighted progression with a default weight of 1.0
    /// </summary>
    /// <param name="progression">Progression whose progress to monitor</param>
    public WeightedProgression(ProgressionType progression) : this(progression, 1.0f) { }

    /// <summary>Initializes a new weighted progression</summary>
    /// <param name="progression">Progression whose progress to monitor</param>
    /// <param name="weight">Weighting of the progression's progress</param>
    public WeightedProgression(ProgressionType progression, float weight) {
      this.progression = progression;
      this.weight = weight;       
    }

    /// <summary>Progression being wrapped by this weighted progression</summary>
    public ProgressionType Progression {
      get { return this.progression; }
    }

    /// <summary>The contribution of this progression to the total progress</summary>
    public float Weight {
      get { return this.weight; }
    }

    /// <summary>Progression whose progress we're tracking</summary>
    private ProgressionType progression;
    /// <summary>Weighting of this progression in the total progress</summary>
    private float weight;

  }

} // namespace Nuclex.Support.Tracking
