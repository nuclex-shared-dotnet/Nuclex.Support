using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Interface for abortable processes</summary>
  public interface IAbortable {

    /// <summary>Aborts the running process. Can be called from any thread.</summary>
    void AsyncAbort();

  }

} // namespace Nuclex.Support.Tracking
