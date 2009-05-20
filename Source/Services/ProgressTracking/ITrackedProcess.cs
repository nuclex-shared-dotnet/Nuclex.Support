using System;
using System.Collections.Generic;

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Services.ProgressTracking {

  /// <summary>Process whose progress is being tracked</summary>
  public interface ITrackedProcess {

    /// <summary>Fired whenever the progress of the process changes</summary>
    event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>Unique identifier of the overall process</summary>
    /// <remarks>
    ///   As a convention, using this object's ToString() method should return
    ///   something usable, either a string that can be displayed in the user
    ///   interface or, depending on your architecture, the object could
    ///   implement certain interfaces that allow a localized version of
    ///   the string to be created.
    /// </remarks>
    object ProcessIdentifier { get; }

    /// <summary>Progress that process has achieved so far</summary>
    float CurrentProgress { get; }

  }

} // namespace Nuclex.Support.DependencyInjection.ProgressTracking
