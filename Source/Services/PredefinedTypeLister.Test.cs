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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

namespace Nuclex.Support.Services {

  /// <summary>Unit Test for the predefined type lister</summary>
  [TestFixture]
  public class PredefinedTypeListerTest {

    /// <summary>
    ///   Verifies that the type lister correctly takes over a list of types
    ///   supplied manually to the constructor
    /// </summary>
    [Test]
    public void TestPredefinedTypesFromParams() {
      ITypeLister testLister = new PredefinedTypeLister(
        typeof(PredefinedTypeListerTest), typeof(TestAttribute)
      );

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(PredefinedTypeListerTest)).And.Member(typeof(TestAttribute))
      );
    }

    /// <summary>
    ///   Verifies that the type lister correctly takes over a list of types
    ///   supplied as an enumerable list to the constructor
    /// </summary>
    [Test]
    public void TestPredefinedTypesFromEnumerable() {
      IEnumerable<Type> types = typeof(PredefinedTypeListerTest).Assembly.GetTypes();
      ITypeLister testLister = new PredefinedTypeLister(types);

      Assert.That(
        testLister.GetTypes(), Has.Member(typeof(PredefinedTypeListerTest))
      );
    }

    /// <summary>
    ///   Verifies that types can be removed from the type lister
    /// </summary>
    [Test]
    public void TestRemoveTypesFromLister() {
      PredefinedTypeLister testLister = new PredefinedTypeLister(
        typeof(PredefinedTypeListerTest).Assembly.GetTypes()
      );

      Assert.That(
        testLister.GetTypes(), Has.Member(typeof(PredefinedTypeListerTest))
      );
      testLister.Types.Remove(typeof(PredefinedTypeListerTest));
      Assert.That(
        testLister.GetTypes(), Has.No.Member(typeof(PredefinedTypeListerTest))
      );
    }

    /// <summary>
    ///   Verifies that types can be added to the type lister
    /// </summary>
    [Test]
    public void TestAddTypesToLister() {
      PredefinedTypeLister testLister = new PredefinedTypeLister();

      Assert.That(
        testLister.GetTypes(), Has.No.Member(typeof(TestAttribute))
      );
      testLister.Types.Add(typeof(TestAttribute));
      Assert.That(
        testLister.GetTypes(), Has.Member(typeof(TestAttribute))
      );
    }

  }

} // namespace Nuclex.Support.Services

#endif // UNITTEST
