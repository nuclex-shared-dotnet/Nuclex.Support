using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Event arguments for a progress update notification</summary>
  public class ProgressUpdateEventArgs : EventArgs {

    /// <summary>Initializes the progress update informations</summary>
    /// <param name="progress">Achieved progress ranging from 0.0 to 1.0</param>
    public ProgressUpdateEventArgs(float progress) {
      this.progress = progress;
    }

    /// <summary>Currently achieved progress</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Achieved progress</summary>
    private float progress;

  }

} // namespace Nuclex.Support.Tracking
