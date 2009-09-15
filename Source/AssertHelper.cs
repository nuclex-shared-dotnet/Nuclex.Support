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

#if UNITTEST

using System;

using NUnit.Framework;

// Decide:
// - Move (part of) this to Nuclex.Support? [done]
// - Create new Assemblies Nuclex.NUnit.dll and Nuclex.NUnit.Xna.dll?

namespace Nuclex.Support {

  /// <summary>Contains special assertions for floating numbers</summary>
  public static class AssertHelper {

    /// <summary>Ensures that two double precision floating point values are equal</summary>
    /// <param name="expected">Expected double precision floating point value</param>
    /// <param name="actual">Actual double precision floating point value</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable double precision floating point values
    /// </param>
    public static void AreAlmostEqual(double expected, double actual, int deltaUlps) {
      AreAlmostEqual(expected, actual, deltaUlps, null);
    }

    /// <summary>Ensures that two double precision floating point values are equal</summary>
    /// <param name="expected">Expected double precision floating point value</param>
    /// <param name="actual">Actual double precision floating point value</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable double precision floating point values
    /// </param>
    /// <param name="message">
    ///   Message to display when the double precision floating point values are not equal
    /// </param>
    public static void AreAlmostEqual(
      double expected, double actual, int deltaUlps, string message
    ) {
      if(!FloatHelper.AreAlmostEqual(expected, actual, deltaUlps)) {
        Assert.AreEqual(expected, actual, message);
      }
    }

    /// <summary>Ensures that two floating point values are equal</summary>
    /// <param name="expected">Expected floating point value</param>
    /// <param name="actual">Actual floating point value</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    public static void AreAlmostEqual(float expected, float actual, int deltaUlps) {
      AreAlmostEqual(expected, actual, deltaUlps, null);
    }

    /// <summary>Ensures that two floating point values are equal</summary>
    /// <param name="expected">Expected floating point value</param>
    /// <param name="actual">Actual floating point value</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    /// <param name="message">
    ///   Message to display when the floating point values are not equal
    /// </param>
    public static void AreAlmostEqual(
      float expected, float actual, int deltaUlps, string message
    ) {
      if(!FloatHelper.AreAlmostEqual(expected, actual, deltaUlps)) {
        Assert.AreEqual(expected, actual, message);
      }
    }

    /// <summary>Ensures that two float arrays are equal</summary>
    /// <param name="expected">Expected float array</param>
    /// <param name="actual">Actual float array</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation for each value in representable floating point values
    /// </param>
    public static void AreAlmostEqual(float[] expected, float[] actual, int deltaUlps) {
      AreAlmostEqual(expected, actual, deltaUlps, null);
    }

    /// <summary>Ensures that two float arrays are equal</summary>
    /// <param name="expected">Expected float array</param>
    /// <param name="actual">Actual float array</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation for each value in representable floating point values
    /// </param>
    /// <param name="message">Message to display when the arrays are not equal</param>
    public static void AreAlmostEqual(
      float[] expected, float[] actual, int deltaUlps, string message
    ) {

      // If one is null, the other also has to be null
      if((expected == null) || (actual == null)) {
        Assert.AreEqual(expected, actual, message);
        return; // This will be reached if both are null, which means they're equal
      }

      // If the lengths do not match, let NUnit print its own message saying that
      if(expected.Length != actual.Length) {
        Assert.AreEqual(expected, actual, message); // will always fail
      }

      // Compare the two arrays element by element
      for(int i = 0; i < expected.Length; ++i) {
        if(!FloatHelper.AreAlmostEqual(expected[i], actual[i], deltaUlps)) {
          string safeMessage = (message == null) ? string.Empty : (message + " ");
          Assert.AreEqual(
            actual[i], expected[i],
            safeMessage + "(arrays differ at index " + i.ToString() + ")"
          );
        }
      }

    }

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
