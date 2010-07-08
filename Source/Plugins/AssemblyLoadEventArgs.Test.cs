﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Nuclex.Support.Plugins {

#if XBOX360

  /// <summary>Unit Test for the assembly load event argument container</summary>
  [TestFixture]
  public class AssemblyLoadEventArgsTest {

    /// <summary>
    ///   Tests whether the argument container correctly stores an assembly reference
    /// </summary>
    [Test]
    public void TestEmployerDefaultConstructorDetection() {
      Assembly assembly = Assembly.GetExecutingAssembly();

      AssemblyLoadEventArgs testArguments = new AssemblyLoadEventArgs(assembly);

      Assert.AreSame(assembly, testArguments.LoadedAssembly);
    }

  }

#endif // XBOX360

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
