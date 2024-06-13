#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

#if !NO_SETS

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

#endif // !NO_SETS
