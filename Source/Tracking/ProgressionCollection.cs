using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Non-generic version of the progression collection</summary>
  public class ProgressionCollection : ProgressionCollection<Progression> { }

  /// <summary>Generic collection of progressions</summary>
  public class ProgressionCollection<ProgressionType>
    : ObservableCollection<Progression> where ProgressionType : Progression { }

} // namespace Nuclex.Support.Tracking
