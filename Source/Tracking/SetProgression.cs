using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Forms a single progression from a set of progressions</summary>
  /// <typeparam name="ProgressionType">Type of progressions to manage as a set</typeparam>
  public class SetProgression<ProgressionType> : Progression
    where ProgressionType : Progression {

    /// <summary>Initializes a new set progression</summary>
    /// <param name="progressions">Progressions to track with this set</param>
    /// <remarks>
    ///   Uses a default weighting factor of 1.0 for all progressions.
    /// </remarks>
    public SetProgression(IEnumerable<ProgressionType> progressions) {
    }

    /// <summary>Initializes a new set progression</summary>
    /// <param name="progressions">Progressions to track with this set</param>
    public SetProgression(IEnumerable<WeightedProgression<ProgressionType>> progressions) {
    }

    /// <summary>Childs contained in the progression set</summary>
    public ReadOnlyCollection<WeightedProgression<ProgressionType>> Childs {
      get { return null; }
    }
/*    
    /// <summary>Progressions being managed in the set</summary>
    private ReadOnlyCollection<ProgressionType> progressions;
    /// <summary>whether the progress needs to be recalculated</summary>
    private volatile bool needProgressRecalculation;
    /// <summary>Total progress achieved by the progressions in this collection</summary>
    private volatile float totalProgress;
*/
  }

} // namespace Nuclex.Support.Tracking
