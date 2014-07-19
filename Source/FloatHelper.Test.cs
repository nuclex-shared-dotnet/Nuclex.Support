#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2014 Nuclex Development Labs

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

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the FloatHelper class</summary>
  [TestFixture]
  public class FloatHelperTest {

    /// <summary>Tests the floating point value comparison helper</summary>
    [Test]
    public void UlpDistancesOnFloatsCompareAsEqual() {
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
    public void UlpDistancesOnDoublesCompareAsEqual() {
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

    /// <summary>Tests the integer reinterpretation functions</summary>
    [Test]
    public void IntegersCanBeReinterpretedAsFloats() {
      Assert.AreEqual(
        12345.0f,
        FloatHelper.ReinterpretAsFloat(FloatHelper.ReinterpretAsInt(12345.0f)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>Tests the long reinterpretation functions</summary>
    [Test]
    public void LongsCanBeReinterpretedAsDoubles() {
      Assert.AreEqual(
        12345.67890,
        FloatHelper.ReinterpretAsDouble(FloatHelper.ReinterpretAsLong(12345.67890)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>Tests the floating point reinterpretation functions</summary>
    [Test]
    public void FloatsCanBeReinterpretedAsIntegers() {
      Assert.AreEqual(
        12345,
        FloatHelper.ReinterpretAsInt(FloatHelper.ReinterpretAsFloat(12345)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>
    ///   Verifies that the IsZero() method can distinguish zero from very small values
    /// </summary>
    [Test]
    public void CanDetermineIfFloatIsZero() {
      Assert.IsTrue(FloatHelper.IsZero(FloatHelper.PositiveZeroFloat));
      Assert.IsTrue(FloatHelper.IsZero(FloatHelper.NegativeZeroFloat));
      Assert.IsFalse(FloatHelper.IsZero(1.401298E-45f));
      Assert.IsFalse(FloatHelper.IsZero(-1.401298E-45f));
    }

    /// <summary>
    ///   Verifies that the IsZero() method can distinguish zero from very small values
    /// </summary>
    [Test]
    public void CanDetermineIfDoubleIsZero() {
      Assert.IsTrue(FloatHelper.IsZero(FloatHelper.PositiveZeroDouble));
      Assert.IsTrue(FloatHelper.IsZero(FloatHelper.NegativeZeroDouble));
      Assert.IsFalse(FloatHelper.IsZero(4.94065645841247E-324));
      Assert.IsFalse(FloatHelper.IsZero(-4.94065645841247E-324));
    }

    /// <summary>
    ///   Tests the double prevision floating point reinterpretation functions
    /// </summary>
    [Test]
    public void DoublesCanBeReinterpretedAsLongs() {
      Assert.AreEqual(
        1234567890,
        FloatHelper.ReinterpretAsLong(FloatHelper.ReinterpretAsDouble(1234567890)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>
    ///   Verifies that two denormalized floats can be compared in ulps
    /// </summary>
    [Test]
    public void DenormalizedFloatsCanBeCompared() {
      float zero = 0.0f;
      float zeroPlusOneUlp = FloatHelper.ReinterpretAsFloat(
        FloatHelper.ReinterpretAsInt(zero) + 1
      );
      float zeroMinusOneUlp = -zeroPlusOneUlp;

      // Across zero
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zeroMinusOneUlp, zeroPlusOneUlp, 1));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zeroPlusOneUlp, zeroMinusOneUlp, 2));

      // Against zero
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zero, zeroPlusOneUlp, 0));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zero, zeroPlusOneUlp, 1));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zero, zeroMinusOneUlp, 0));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zero, zeroMinusOneUlp, 1));
    }

    /// <summary>
    ///   Verifies that the negative floating point zero is within one ulp of the positive
    ///   floating point zero and vice versa
    /// </summary>
    [Test]
    public void NegativeZeroFloatEqualsPositiveZero() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(
          FloatHelper.NegativeZeroFloat, FloatHelper.PositiveZeroFloat, 0
        )
      );
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(
          FloatHelper.PositiveZeroFloat, FloatHelper.NegativeZeroFloat, 0
        )
      );
    }

    /// <summary>Verifies that floats can be compared across the zero boundary</summary>
    [Test]
    public void FloatsCanBeComparedAcrossZeroInUlps() {
      float tenUlps = float.Epsilon * 10.0f;

      Assert.IsTrue(FloatHelper.AreAlmostEqual(-tenUlps, tenUlps, 20));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(tenUlps, -tenUlps, 20));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(-tenUlps, tenUlps, 19));

      Assert.IsTrue(FloatHelper.AreAlmostEqual(-tenUlps, 0, 10));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(0, -tenUlps, 10));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(-tenUlps, 0, 9));

      Assert.IsTrue(FloatHelper.AreAlmostEqual(0, tenUlps, 10));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(tenUlps, 0, 10));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(0, tenUlps, 9));
    }

    /// <summary>
    ///   Verifies that two denormalized doubles can be compared in ulps
    /// </summary>
    [Test]
    public void DenormalizedDoublesCanBeCompared() {
      double zero = 0.0;
      double zeroPlusOneUlp = FloatHelper.ReinterpretAsDouble(
        FloatHelper.ReinterpretAsLong(zero) + 1
      );
      double zeroMinusOneUlp = -zeroPlusOneUlp;

      // Across zero
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zeroMinusOneUlp, zeroPlusOneUlp, 1));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zeroPlusOneUlp, zeroMinusOneUlp, 2));

      // Against zero
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zero, zeroPlusOneUlp, 0));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zero, zeroPlusOneUlp, 1));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(zero, zeroMinusOneUlp, 0));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(zero, zeroMinusOneUlp, 1));
    }

    /// <summary>
    ///   Verifies that the negative double precision floating point zero is within one ulp
    ///   of the positive double precision floating point zero and vice versa
    /// </summary>
    [Test]
    public void NegativeZeroDoubleEqualsPositiveZero() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(
          FloatHelper.NegativeZeroDouble, FloatHelper.NegativeZeroDouble, 0
        )
      );
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(
          FloatHelper.NegativeZeroDouble, FloatHelper.NegativeZeroDouble, 0
        )
      );
    }

    /// <summary>Verifies that doubles can be compared across the zero boundary</summary>
    [Test]
    public void DoublesCanBeComparedAcrossZeroInUlps() {
      double tenUlps = double.Epsilon * 10.0;

      Assert.IsTrue(FloatHelper.AreAlmostEqual(-tenUlps, tenUlps, 20));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(tenUlps, -tenUlps, 20));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(-tenUlps, tenUlps, 19));

      Assert.IsTrue(FloatHelper.AreAlmostEqual(-tenUlps, 0, 10));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(0, -tenUlps, 10));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(-tenUlps, 0, 9));

      Assert.IsTrue(FloatHelper.AreAlmostEqual(0, tenUlps, 10));
      Assert.IsTrue(FloatHelper.AreAlmostEqual(tenUlps, 0, 10));
      Assert.IsFalse(FloatHelper.AreAlmostEqual(0, tenUlps, 9));
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
