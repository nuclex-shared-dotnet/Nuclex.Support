﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

#if !XBOX360

using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Win32;

namespace Nuclex.Support.Scheduling {

  /// <summary>
  ///   Time source that makes use of additional features only available on Windows
  /// </summary>
  public class WindowsTimeSource : DefaultTimeSource, IDisposable {

    /// <summary>Number of ticks (100 ns intervals) in a millisecond</summary>
    private const long TicksPerMillisecond = 10000;

    /// <summary>Initializes a new Windows time source</summary>
    public WindowsTimeSource() {
      this.onDateTimeAdjustedDelegate = new EventHandler(OnDateTimeAdjusted);
      SystemEvents.TimeChanged += this.onDateTimeAdjustedDelegate;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.onDateTimeAdjustedDelegate != null) {
        SystemEvents.TimeChanged -= this.onDateTimeAdjustedDelegate;
        this.onDateTimeAdjustedDelegate = null;
      }
    }

    /// <summary>Waits for an AutoResetEvent become signalled</summary>
    /// <param name="waitHandle">WaitHandle the method will wait for</param>
    /// <param name="ticks">Number of ticks to wait</param>
    /// <returns>
    ///   True if the WaitHandle was signalled, false if the timeout was reached
    /// </returns>
    public override bool WaitOne(AutoResetEvent waitHandle, long ticks) {
      return waitHandle.WaitOne((int)(ticks / TicksPerMillisecond));
    }

    /// <summary>
    ///   Whether the Windows time source can be used on the current platform
    /// </summary>
    public static bool Available {
      get { return Environment.OSVersion.Platform == PlatformID.Win32NT; }
    }

    /// <summary>Delegate for the timeChanged() callback method</summary>
    private EventHandler onDateTimeAdjustedDelegate;

  }

} // namespace Nuclex.Support.Scheduling

#endif // !XBOX360
