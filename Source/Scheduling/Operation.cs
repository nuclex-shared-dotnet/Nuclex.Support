using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for observable operations running in the background</summary>
  public abstract class Operation : Progression {

    /// <summary>Executes the operation</summary>
    public abstract void Start();

    /// <summary>
    ///   Executes the operation synchronously, blocking the calling thread
    /// </summary>
    public virtual void Execute() {
      Start();
      WaitHandle.WaitOne();
    }

  }

} // namespace Nuclex.Support.Tracking
