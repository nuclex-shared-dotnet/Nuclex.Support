using System;
using System.Collections.Generic;

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Services.ProgressTracking {

#if false
  /// <summary>Tracks the progress of running background processes</summary>
  public class ProgressTrackingComponent :
    IProgressTrackingService,
    IProgressPublishingService {

    /// <summary>Fired when the overall progress changes</summary>
    public event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>Initializes a new progress tracking component</summary>
    public ProgressTrackingComponent() {
    }

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    /// <param name="processIdentifier">
    ///   Identifier unique to the tracked background process. Can be null.
    /// </param>
    public void Track(Transaction transaction, object processIdentifier) {
    }

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    public void Track(Transaction transaction) {
    }

    /// <summary>Stops tracking the specified transaction</summary>
    /// <param name="transaction">Transaction that will no longer be tracked</param>
    public void Untrack(Transaction transaction) {
    }

    /// <summary>The overall progress of all tracked background processes</summary>
    public float TotalProgress {
      get { return 0.0f; }
    }

    /// <summary>Currently active background processes</summary>
    public IEnumerable<ITrackedProcess> TrackedProcesses {
      get { return null; }
    }

  }
#endif

} // namespace Nuclex.Support.Services.ProgressTracking
