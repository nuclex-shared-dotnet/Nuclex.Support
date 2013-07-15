#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2013 Nuclex Development Labs

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

using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the Variegator multi dictionary</summary>
  [TestFixture]
  internal class VariegatorTest {

    /// <summary>
    ///   Tests whether the default constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void InstancesCanBeCreated() {
      new Variegator<int, string>();
    }

    /// <summary>
    ///   Verifies that querying for a missing value leads to an exception being thrown
    /// </summary>
    [Test]
    public void QueryingMissingValueThrowsException() {
      var variegator = new Variegator<int, string>();
      Assert.Throws<KeyNotFoundException>(
        () => {
          variegator.Get(123);
        }
      );
    }

    /// <summary>
    ///   Verifies that the variegator resolves ambiguous matches according to its design
    /// </summary>
    [Test]
    public void AmbiguityResolvesToLeastRecentValue() {
      var variegator = new Variegator<int, string>();
      variegator.Add(1, "one");
      variegator.Add(1, "eins");

      string first = variegator.Get(1);
      string second = variegator.Get(1);

      // The variegator should have selected the first value by random and then
      // returned the other value on the second query
      Assert.AreNotEqual(first, second);

      // Now the variegator should return the first value again because it is
      // the least recently used value
      Assert.AreEqual(first, variegator.Get(1));

      // Repeating the query, the second should be returned again because now
      // it has become the least recently used value
      Assert.AreEqual(second, variegator.Get(1));
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
