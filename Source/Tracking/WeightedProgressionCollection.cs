using System;
using System.Collections.Generic;
using System.Text;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  internal class WeightedProgressionCollection<ProgressionType>
    : ObservableCollection<WeightedProgression<ProgressionType>>
    where ProgressionType : Progression { }

} // namespace Nuclex.Support.Tracking
