using System;
using System.Collections.Generic;

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Services.ProgressTracking {

  /// <summary>Reports the progress of tracked background processes</summary>
  /// <remarks>
  ///   <para>
  ///     This service is intended for the consumer of progress reports. It will notify
  ///     subscribers when background processes start, when progress is achieved and
  ///     when they finish.
  ///   </para>
  ///   <para>
  ///     Usually, this interface, together with the IProgressTrackingService interface,
  ///     is provided by a single service component that tracks the progress of
  ///     transactions taking place asynchronously and reports it back this way.
  ///   </para>
  /// </remarks>
  interface IProgressPublishingService {

    /// <summary>Fired when the overall progress changes</summary>
    event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>The overall progress of all tracked background processes</summary>
    float TotalProgress { get; }

    /// <summary>Currently active background processes</summary>
    IEnumerable<ITrackedProcess> TrackedProcesses { get; }

  }

} // namespace Nuclex.Support.DependencyInjection.ProgressTracking
