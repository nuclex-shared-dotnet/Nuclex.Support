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
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Contains unit tests for the integer helper class</summary>
  [TestFixture]
  internal class IntegerHelperTest {

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for long integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2ULong() {
      Assert.AreEqual(1UL, IntegerHelper.NextPowerOf2(0UL));
      Assert.AreEqual(1UL, IntegerHelper.NextPowerOf2(1UL));
      Assert.AreEqual(2UL, IntegerHelper.NextPowerOf2(2UL));
      Assert.AreEqual(4UL, IntegerHelper.NextPowerOf2(3UL));
      Assert.AreEqual(4UL, IntegerHelper.NextPowerOf2(4UL));
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(4611686018427387905UL)
      );
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(9223372036854775807UL)
      );
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(9223372036854775808UL)
      );
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for long integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2Long() {
      Assert.AreEqual(1L, IntegerHelper.NextPowerOf2(0L));
      Assert.AreEqual(1L, IntegerHelper.NextPowerOf2(1L));
      Assert.AreEqual(2L, IntegerHelper.NextPowerOf2(2L));
      Assert.AreEqual(4L, IntegerHelper.NextPowerOf2(3L));
      Assert.AreEqual(4L, IntegerHelper.NextPowerOf2(4L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(2305843009213693953L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(4611686018427387903L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(4611686018427387904L));
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2UInt() {
      Assert.AreEqual(1U, IntegerHelper.NextPowerOf2(0U));
      Assert.AreEqual(1U, IntegerHelper.NextPowerOf2(1U));
      Assert.AreEqual(2U, IntegerHelper.NextPowerOf2(2U));
      Assert.AreEqual(4U, IntegerHelper.NextPowerOf2(3U));
      Assert.AreEqual(4U, IntegerHelper.NextPowerOf2(4U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(1073741825U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(2147483647U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(2147483648U));
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2Int() {
      Assert.AreEqual(1, IntegerHelper.NextPowerOf2(0));
      Assert.AreEqual(1, IntegerHelper.NextPowerOf2(1));
      Assert.AreEqual(2, IntegerHelper.NextPowerOf2(2));
      Assert.AreEqual(4, IntegerHelper.NextPowerOf2(3));
      Assert.AreEqual(4, IntegerHelper.NextPowerOf2(4));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(536870913));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(1073741823));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(1073741824));
    }

    /// <summary>Verifies that the bit counting method for integers works</summary>
    [Test]
    public void TestCountBitsInInteger() {
      Assert.AreEqual(0, IntegerHelper.CountBits(0));
      Assert.AreEqual(32, IntegerHelper.CountBits(-1));
      Assert.AreEqual(16, IntegerHelper.CountBits(0x55555555));
      Assert.AreEqual(16, IntegerHelper.CountBits(0xAAAAAAAA));

      for (int bitIndex = 0; bitIndex < 32; ++bitIndex) {
        Assert.AreEqual(1, IntegerHelper.CountBits(1 << bitIndex));
      }
    }

    /// <summary>Verifies that the bit counting method for long integers works</summary>
    [Test]
    public void TestCountBitsInLongInteger() {
      Assert.AreEqual(0, IntegerHelper.CountBits(0L));
      Assert.AreEqual(64, IntegerHelper.CountBits(-1L));
      Assert.AreEqual(32, IntegerHelper.CountBits(0x5555555555555555));
      Assert.AreEqual(32, IntegerHelper.CountBits(0xAAAAAAAAAAAAAAAA));

      for (int bitIndex = 0; bitIndex < 64; ++bitIndex) {
        Assert.AreEqual(1, IntegerHelper.CountBits(1 << bitIndex));
      }
    }

  }

} // namespace Nuclex.Support
