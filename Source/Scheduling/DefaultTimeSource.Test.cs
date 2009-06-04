#region CPL License
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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the default scheduler time source</summary>
  [TestFixture]
  public class DefaultTimeSourceTest {

    /// <summary>
    ///   Verifies that the time source's default constructor is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      DefaultTimeSource timeSource = new DefaultTimeSource();
    }

    /// <summary>
    ///   Verifies that the time source can provide the current UTC time
    /// </summary>
    [Test]
    public void TestCurrentUtcTime() {
      DefaultTimeSource timeSource = new DefaultTimeSource();

      Assert.That(
        timeSource.CurrentUtcTime, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds
      );
    }

    /// <summary>
    ///   Verifies that the default time source's tick property is working if
    ///   the Stopwatch class is used to measure time
    /// </summary>
    [Test]
    public void TestTicksWithStopwatch() {
      DefaultTimeSource timeSource = new DefaultTimeSource(true);
      long ticks1 = timeSource.Ticks;
      long ticks2 = timeSource.Ticks;

      Assert.That(ticks2, Is.GreaterThanOrEqualTo(ticks1));
    }

    /// <summary>
    ///   Verifies that the default time source's tick property is working if
    ///   Environment.TickCount is used to measure time
    /// </summary>
    [Test]
    public void TestTicksWithTickCount() {
      DefaultTimeSource timeSource = new DefaultTimeSource(false);
      long ticks1 = timeSource.Ticks;
      long ticks2 = timeSource.Ticks;

      Assert.That(ticks2, Is.GreaterThanOrEqualTo(ticks1));
    }

    /// <summary>
    ///   Verifies that the default time source's WaitOne() method works correctly
    /// </summary>
    [Test]
    public void TestWaitOne() {
      DefaultTimeSource timeSource = new DefaultTimeSource();
      AutoResetEvent waitEvent = new AutoResetEvent(true);

      Assert.IsTrue(timeSource.WaitOne(waitEvent, TimeSpan.FromMilliseconds(1).Ticks));
      Assert.IsFalse(timeSource.WaitOne(waitEvent, TimeSpan.FromMilliseconds(1).Ticks));
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // UNITTEST
