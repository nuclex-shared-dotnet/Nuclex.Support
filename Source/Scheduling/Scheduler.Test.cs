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
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32;

using NUnit.Framework;

#if false

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the scheduler</summary>
  [TestFixture]
  public class SchedulerTest {

    /// <summary>
    ///   Test whether the Scheduler can explicitely create a windows time source
    /// </summary>
    [Test]
    public void TestCreateWindowsTimeSource() {
      ITimeSource timeSource = Scheduler.CreateTimeSource(true);
      try {
        Assert.That(timeSource is WindowsTimeSource);
      }
      finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Test whether the Scheduler can explicitely create a generic time source
    /// </summary>
    [Test]
    public void TestCreateGenericTimeSource() {
      ITimeSource timeSource = Scheduler.CreateTimeSource(false);
      try {
        Assert.That(timeSource is GenericTimeSource);
      }
      finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Test whether the Scheduler can automatically choose the right time source
    /// </summary>
    [Test]
    public void TestCreateDefaultTimeSource() {
      ITimeSource timeSource = Scheduler.CreateDefaultTimeSource();
      try {
        Assert.IsNotNull(timeSource);
      }
      finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Verifies that the default constructor of the scheduler is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      using(Scheduler scheduler = new Scheduler()) { }
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif

#endif // UNITTEST
