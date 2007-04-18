using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for observable operations running in the background</summary>
  public abstract class Operation : Progression {

    /// <summary>Executes the operation</summary>
    public abstract void Execute();

  }

} // namespace Nuclex.Support.Tracking
