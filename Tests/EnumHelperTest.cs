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

using System;
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the enumeration helper class</summary>
  [TestFixture]
  internal class EnumHelperTest {

    #region enum TestEnumeration

    /// <summary>An enumeration used for unit testing</summary>
    internal enum TestEnumeration {
      /// <summary>First arbitrary enumeration value</summary>
      One = -2,
      /// <summary>Third arbitrary enumeration value</summary>
      Three = 33,
      /// <summary>Second arbitrary enumeration value</summary>
      Two = 23
    }

    #endregion // enum TestEnumeration

    #region enum EmptyEnumeration

    internal enum EmptyEnumeration { }

    #endregion // enum EmptyEnumeration

    /// <summary>
    ///   Verifies that the enum helper can list the members of an enumeration
    /// </summary>
    [Test]
    public void TestGetValues() {
      CollectionAssert.AreEquivalent(
        new TestEnumeration[] {
          TestEnumeration.One, TestEnumeration.Two, TestEnumeration.Three
        },
        EnumHelper.GetValues<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the enum helper can locate the highest value in an enumeration
    /// </summary>
    [Test]
    public void TestGetHighestValue() {
      Assert.AreEqual(
        TestEnumeration.Three, EnumHelper.GetHighestValue<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the enum helper can locate the lowest value in an enumeration
    /// </summary>
    [Test]
    public void TestGetLowestValue() {
      Assert.AreEqual(
        TestEnumeration.One, EnumHelper.GetLowestValue<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the GetValues() method is used on
    ///   a non-enumeration type
    /// </summary>
    [Test]
    public void TestThrowOnNonEnumType() {
      Assert.Throws<ArgumentException>(
        delegate() { EnumHelper.GetValues<int>(); }
      );
    }

    /// <summary>
    ///   Verifies that the default value for an enumeration is returned if
    ///   the GetLowestValue() method is used on an empty enumeration
    /// </summary>
    [Test]
    public void TestLowestValueInEmptyEnumeration() {
      Assert.AreEqual(
        default(EmptyEnumeration), EnumHelper.GetLowestValue<EmptyEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the default value for an enumeration is returned if
    ///   the GetHighestValue() method is used on an empty enumeration
    /// </summary>
    [Test]
    public void TestHighestValueInEmptyEnumeration() {
      Assert.AreEqual(
        default(EmptyEnumeration), EnumHelper.GetHighestValue<EmptyEnumeration>()
      );
    }

  }

} // namespace Nuclex.Support
