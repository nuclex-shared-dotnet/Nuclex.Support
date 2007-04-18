using System;
using System.Collections.Generic;
using System.Text;

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
