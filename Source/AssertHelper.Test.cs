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

using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the special floating point assertions</summary>
  [TestFixture]
  public class AssertHelperTest {

    /// <summary>
    ///   Tests whether the almost equal check works with floating point values
    /// </summary>
    [Test]
    public void TestAlmostEqualWithFloats() {
      AssertHelper.AreAlmostEqual(exactFloat, minusOneFloat, 1);
      AssertHelper.AreAlmostEqual(exactFloat, plusOneFloat, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check detects a floating point value that is
    ///   just barely too low
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithTooLowFloat() {
      AssertHelper.AreAlmostEqual(exactFloat, minusTwoFloat, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check detects a floating point value that is
    ///   just barely too high
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithTooHighFloat() {
      AssertHelper.AreAlmostEqual(exactFloat, plusTwoFloat, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check works with double precision floating points
    /// </summary>
    [Test]
    public void TestAlmostEqualWithDoubles() {
      AssertHelper.AreAlmostEqual(exactDouble, minusOneDouble, 1);
      AssertHelper.AreAlmostEqual(exactDouble, plusOneDouble, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check detects a double precision floating point
    ///   value that is just barely too low
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithTooLowDouble() {
      AssertHelper.AreAlmostEqual(exactDouble, minusTwoDouble, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check detects a double precision floating point
    ///   value that is just barely too high
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithTooHighDouble() {
      AssertHelper.AreAlmostEqual(exactDouble, plusTwoDouble, 1);
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper works correctly when comparing
    ///   two floating point arrays that are both null
    /// </summary>
    [Test]
    public void TestAlmostEqualWithNullFloatArrays() {
      float[] nullArray = null;

      AssertHelper.AreAlmostEqual(nullArray, nullArray, 1);
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper works correctly when comparing
    ///   two floating point arrays that are within the allowed deviation
    /// </summary>
    [Test]
    public void TestAlmostEqualWithFloatArrays() {
      float[] referenceArray = new float[] { exactFloat, exactFloat, exactFloat };
      float[] testArray = new float[] { exactFloat, plusOneFloat, minusOneFloat };

      AssertHelper.AreAlmostEqual(referenceArray, testArray, 1);
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper throws an exception if two arrays
    ///   of different length are compared to each other
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithFloatArraysOfDifferentLength() {
      float[] referenceArray = new float[] { exactFloat, exactFloat, exactFloat };
      float[] testArray = new float[] { exactFloat, exactFloat };

      AssertHelper.AreAlmostEqual(referenceArray, testArray, 1);
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper throws an exception if the two
    ///   arrays contain elements that deviate by more than the allowed amount
    /// </summary>
    [Test, ExpectedException(typeof(AssertionException))]
    public void TestThrowOnAlmostEqualWithBarelyDifferingFloatArrays() {
      float[] referenceArray = new float[] { exactFloat, exactFloat, exactFloat };
      float[] testArray = new float[] { plusOneFloat, minusOneFloat, plusTwoFloat };

      AssertHelper.AreAlmostEqual(referenceArray, testArray, 1);
    }

    /// <summary>
    ///   Adjusts a floating point value by the specified amount of neighbouring
    ///   representable values
    /// </summary>
    /// <param name="value">Floating point value to be adjusted</param>
    /// <param name="ulps">Numbers of neighbouring representable values to step</param>
    /// <returns>The adjusted floating point value</returns>
    private static float adjust(float value, int ulps) {
      return FloatHelper.ReinterpretAsFloat(FloatHelper.ReinterpretAsInt(value) + ulps);
    }

    /// <summary>
    ///   Adjusts a double precision floating point value by the specified amount of
    ///   neighbouring representable values
    /// </summary>
    /// <param name="value">Double precision floating point value to be adjusted</param>
    /// <param name="ulps">Numbers of neighbouring representable values to step</param>
    /// <returns>The adjusted double precision floating point value</returns>
    private static double adjust(double value, long ulps) {
      return FloatHelper.ReinterpretAsDouble(FloatHelper.ReinterpretAsLong(value) + ulps);
    }

    /// <summary>The exact test value as a float</summary>
    private static readonly float exactFloat = 1234.5678f;
    /// <summary>The second next possible smaller float from the test value</summary>
    private static readonly float minusTwoFloat = adjust(exactFloat, -2);
    /// <summary>The next possible smaller float from the test value</summary>
    private static readonly float minusOneFloat = adjust(exactFloat, -1);
    /// <summary>The next possible greater float from the test value</summary>
    private static readonly float plusOneFloat = adjust(exactFloat, +1);
    /// <summary>The second next possible greater float from the test value</summary>
    private static readonly float plusTwoFloat = adjust(exactFloat, +2);

    /// <summary>The exact test value as a float</summary>
    private static readonly double exactDouble = 1234.5678f;
    /// <summary>The second next possible smaller float from the test value</summary>
    private static readonly double minusTwoDouble = adjust(exactDouble, -2);
    /// <summary>The next possible smaller float from the test value</summary>
    private static readonly double minusOneDouble = adjust(exactDouble, -1);
    /// <summary>The next possible greater float from the test value</summary>
    private static readonly double plusOneDouble = adjust(exactDouble, +1);
    /// <summary>The second next possible greater float from the test value</summary>
    private static readonly double plusTwoDouble = adjust(exactDouble, +2);

  }

} // namespace Nuclex.Support

#endif // UNITTEST
