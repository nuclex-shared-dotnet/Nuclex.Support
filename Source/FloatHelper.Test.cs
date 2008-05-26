using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the FloatHelper class</summary>
  [TestFixture]
  public class FloatHelperTest {

    /// <summary>Tests the floating point value comparison helper</summary>
    [Test]
    public void TestFloatComparison() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(0.00000001f, 0.0000000100000008f, 1),
        "Minimal difference between very small floating point numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(0.00000001f, 0.0000000100000017f, 1),
        "Larger difference between very small floating point numbers is not considered equal"
      );

      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(1000000.00f, 1000000.06f, 1),
        "Minimal difference between very large floating point numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(1000000.00f, 1000000.13f, 1),
        "Larger difference between very large floating point numbers is not considered equal"
      );
    }

    /// <summary>Tests the double precision floating point value comparison helper</summary>
    [Test]
    public void TestDoubleComparison() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(0.00000001, 0.000000010000000000000002, 1),
        "Minimal difference between very small double precision floating point " +
        "numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(0.00000001, 0.000000010000000000000004, 1),
        "Larger difference between very small double precision floating point " +
        "numbers is not considered equal"
      );

      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(1000000.00, 1000000.0000000001, 1),
        "Minimal difference between very large double precision floating point " +
        "numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(1000000.00, 1000000.0000000002, 1),
        "Larger difference between very large double precision floating point " +
        "numbers is not considered equal"
      );
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST