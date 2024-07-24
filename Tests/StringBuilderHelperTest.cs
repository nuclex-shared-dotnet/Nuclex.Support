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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>
  ///   Unit test for the helper class to .NET's string builder
  /// </summary>
  [TestFixture]
  internal class StringBuilderHelperTest {

    /// <summary>
    ///   Verifies that bytes are correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendByte() {
      StringBuilder builder = new StringBuilder();

      builder.Append((byte)255, GarbagePolicy.Avoid);
      Assert.AreEqual(((byte)255).ToString(), builder.ToString());

      builder.Clear();

      builder.Append((byte)255, GarbagePolicy.Accept);
      Assert.AreEqual(((byte)255).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a byte with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullByte() {
      StringBuilder builder = new StringBuilder();

      builder.Append((byte)0, GarbagePolicy.Avoid);
      Assert.AreEqual(((byte)0).ToString(), builder.ToString());

      builder.Clear();

      builder.Append((byte)0, GarbagePolicy.Accept);
      Assert.AreEqual(((byte)0).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a positive integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendPositiveInteger() {
      StringBuilder builder = new StringBuilder();

      builder.Append(12345, GarbagePolicy.Avoid);
      Assert.AreEqual((12345).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(12345, GarbagePolicy.Accept);
      Assert.AreEqual((12345).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that an integer with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullInteger() {
      StringBuilder builder = new StringBuilder();

      builder.Append(0, GarbagePolicy.Avoid);
      Assert.AreEqual((0).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(0, GarbagePolicy.Accept);
      Assert.AreEqual((0).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a negative integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNegativeInteger() {
      StringBuilder builder = new StringBuilder();

      builder.Append(-12345, GarbagePolicy.Avoid);
      Assert.AreEqual((-12345).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(-12345, GarbagePolicy.Accept);
      Assert.AreEqual((-12345).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a positive long integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendPositiveLong() {
      StringBuilder builder = new StringBuilder();

      builder.Append(12345L, GarbagePolicy.Avoid);
      Assert.AreEqual((12345L).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(12345L, GarbagePolicy.Accept);
      Assert.AreEqual((12345L).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a long integer with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullLong() {
      StringBuilder builder = new StringBuilder();

      builder.Append(0L, GarbagePolicy.Avoid);
      Assert.AreEqual((0L).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(0L, GarbagePolicy.Accept);
      Assert.AreEqual((0L).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a negative long integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNegativeLong() {
      StringBuilder builder = new StringBuilder();

      builder.Append(-12345L, GarbagePolicy.Avoid);
      Assert.AreEqual((-12345L).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(-12345L, GarbagePolicy.Accept);
      Assert.AreEqual((-12345L).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that negative floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendNegativeFloat() {
      StringBuilder builder = new StringBuilder();

      builder.Append(-0.125f, GarbagePolicy.Avoid);
      Assert.AreEqual((-0.125f).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(-0.125f, GarbagePolicy.Accept);
      Assert.AreEqual((-0.125f).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that positive floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendPositiveFloat() {
      StringBuilder builder = new StringBuilder();

      builder.Append(10.0625f, GarbagePolicy.Avoid);
      Assert.AreEqual((10.0625f).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(10.0625f, GarbagePolicy.Accept);
      Assert.AreEqual((10.0625f).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that very small floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendSmallFloat() {
      StringBuilder builder = new StringBuilder();

      builder.Append(0.00390625f, GarbagePolicy.Avoid);
      Assert.AreEqual((0.00390625f).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(0.00390625f, GarbagePolicy.Accept);
      Assert.AreEqual((0.00390625f).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that very large floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendHugeFloat() {
      StringBuilder builder = new StringBuilder();

      builder.Append(1000000000.0f, GarbagePolicy.Avoid);
      Assert.AreEqual((1000000000.0f).ToString("F1"), builder.ToString());

      builder.Clear();

      builder.Append(1000000000.0f, GarbagePolicy.Accept);
      Assert.AreEqual((1000000000.0f).ToString(), builder.ToString());
    }

    /// <summary>Tests whether the number of decimal places can be restricted</summary>
    [Test]
    public void TestAppendFloatLimitDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      builder.Append(0.00390625f, 3);
      Assert.AreEqual((0.003f).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that a float with no decimal places is correctly appended
    /// </summary>
    [Test]
    public void TestAppendFloatWithoutDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      builder.Append(0.00390625f, 0);
      Assert.AreEqual((0.0f).ToString(), builder.ToString()); // Note: no rounding!
    }

    /// <summary>
    ///   Verifies the behavior of the helper with unsupported floating point values
    /// </summary>
    [Test]
    public void TestAppendOutOfRangeFloat() {
      StringBuilder builder = new StringBuilder();
      Assert.IsFalse(builder.Append(float.PositiveInfinity, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(float.NegativeInfinity, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(float.NaN, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(0.000000059604644775390625f, GarbagePolicy.Avoid));
    }

    /// <summary>
    ///   Verifies that negative double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendNegativeDouble() {
      StringBuilder builder = new StringBuilder();

      builder.Append(-32.015625, GarbagePolicy.Avoid);
      Assert.AreEqual((-32.015625).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(-32.015625, GarbagePolicy.Accept);
      Assert.AreEqual((-32.015625).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that positive double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendPositiveDouble() {
      StringBuilder builder = new StringBuilder();

      builder.Append(10.0625, GarbagePolicy.Avoid);
      Assert.AreEqual((10.0625).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(10.0625, GarbagePolicy.Accept);
      Assert.AreEqual((10.0625).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that very small double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendSmallDouble() {
      StringBuilder builder = new StringBuilder();

      builder.Append(0.00390625, GarbagePolicy.Avoid);
      Assert.AreEqual((0.00390625).ToString(), builder.ToString());

      builder.Clear();

      builder.Append(0.00390625, GarbagePolicy.Accept);
      Assert.AreEqual((0.00390625).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies that very large double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendHugeDouble() {
      StringBuilder builder = new StringBuilder();

      builder.Append(1000000000000000000.0, GarbagePolicy.Avoid);
      Assert.AreEqual((1000000000000000000.0).ToString("F1"), builder.ToString());

      builder.Clear();

      builder.Append(1000000000000000000.0, GarbagePolicy.Accept);
      Assert.AreEqual((1000000000000000000.0).ToString(), builder.ToString());
    }

    /// <summary>Tests whether the number of decimal places can be restricted</summary>
    [Test]
    public void TestAppendDoubleLimitDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625, 3);

      Assert.AreEqual((0.003).ToString(), builder.ToString()); // Note: no rounding!
    }

    /// <summary>
    ///   Verifies that a double with no decimal places is correctly appended
    /// </summary>
    [Test]
    public void TestAppendDoubleWithoutDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625, 0);

      Assert.AreEqual((0.0).ToString(), builder.ToString());
    }

    /// <summary>
    ///   Verifies the behavior of the helper with unsupported double precision
    ///   floating point values
    /// </summary>
    [Test]
    public void TestAppendOutOfRangeDouble() {
      StringBuilder builder = new StringBuilder();
      Assert.IsFalse(builder.Append(double.PositiveInfinity, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(double.NegativeInfinity, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(double.NaN, GarbagePolicy.Avoid));
      Assert.IsFalse(builder.Append(1.1102230246251565404236316680908e-16, GarbagePolicy.Avoid));
    }

    /// <summary>
    ///   Verifies that the contents of a string builder can be cleared
    /// </summary>
    [Test]
    public void TestClear() {
      StringBuilder builder = new StringBuilder("Hello World");
      StringBuilderHelper.Clear(builder);

      Assert.AreEqual(string.Empty, builder.ToString());
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
