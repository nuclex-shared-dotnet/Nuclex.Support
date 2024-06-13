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
using System.Runtime.InteropServices;

namespace Nuclex.Support {

  /// <summary>Helper routines for working with floating point numbers</summary>
  /// <remarks>
  ///   <para>
  ///     The floating point comparison code is based on this excellent article:
  ///     http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm
  ///   </para>
  ///   <para>
  ///     "ULP" means Unit in the Last Place and in the context of this library refers to
  ///     the distance between two adjacent floating point numbers. IEEE floating point
  ///     numbers can only represent a finite subset of natural numbers, with greater
  ///     accuracy for smaller numbers and lower accuracy for very large numbers.
  ///   </para>
  ///   <para>
  ///     If a comparison is allowed "2 ulps" of deviation, that means the values are
  ///     allowed to deviate by up to 2 adjacent floating point values, which might be
  ///     as low as 0.0000001 for small numbers or as high as 10.0 for large numbers.
  ///   </para>
  /// </remarks>
  public static class FloatHelper {

    #region struct FloatIntUnion

    /// <summary>Union of a floating point variable and an integer</summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct FloatIntUnion {

      /// <summary>The union's value as a floating point variable</summary>
      [FieldOffset(0)]
      public float Float;

      /// <summary>The union's value as an integer</summary>
      [FieldOffset(0)]
      public int Int;

      /// <summary>The union's value as an unsigned integer</summary>
      [FieldOffset(0)]
      public uint UInt;

    }

    #endregion // struct FloatIntUnion

    #region struct DoubleLongUnion

    /// <summary>Union of a double precision floating point variable and a long</summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct DoubleLongUnion {

      /// <summary>The union's value as a double precision floating point variable</summary>
      [FieldOffset(0)]
      public double Double;

      /// <summary>The union's value as a long</summary>
      [FieldOffset(0)]
      public long Long;

      /// <summary>The union's value as an unsigned long</summary>
      [FieldOffset(0)]
      public ulong ULong;

    }

    #endregion // struct DoubleLongUnion

    /// <summary>A floating point value that holds a positive zero</summary>
    public const float PositiveZeroFloat = +0.0f;

    /// <summary>A floating point value that holds a negative zero</summary>
    /// <remarks>
    ///   Negative zeros have a special representation in IEEE 752 floating point math
    /// </remarks>
    public const float NegativeZeroFloat = -0.0f;

    /// <summary>A double precision floating point value that holds a positive zero</summary>
    public const double PositiveZeroDouble = +0.0;

    /// <summary>A doublep precision floating point value that holds a negative zero</summary>
    /// <remarks>
    ///   Negative zeros have a special representation in IEEE 752 floating point math
    /// </remarks>
    public const double NegativeZeroDouble = -0.0;

    /// <summary>Checks whether the floating point value is exactly zero</summary>
    /// <param name="value">Value that will be checked for being zero</param>
    /// <returns>True if the value is zero, false otherwise</returns>
    public static bool IsZero(float value) {
      return (value == PositiveZeroFloat) || (value == NegativeZeroFloat);
    }

    /// <summary>
    ///   Checks whether the double precision floating point value is exactly zero
    /// </summary>
    /// <param name="value">Value that will be checked for being zero</param>
    /// <returns>True if the value is zero, false otherwise</returns>
    public static bool IsZero(double value) {
      return (value == PositiveZeroDouble) || (value == NegativeZeroDouble);
    }

    /// <summary>Compares two floating point values for equality</summary>
    /// <param name="left">First floating point value to be compared</param>
    /// <param name="right">Second floating point value t be compared</param>
    /// <param name="maxUlps">
    ///   Maximum number of representable floating point values that are allowed to
    ///   be between the left and the right floating point values
    /// </param>
    /// <returns>True if both numbers are equal or close to being equal</returns>
    /// <remarks>
    ///   <para>
    ///     Floating point values can only represent a finite subset of natural numbers.
    ///     For example, the values 2.00000000 and 2.00000024 can be stored in a float,
    ///     but nothing inbetween them.
    ///   </para>
    ///   <para>
    ///     This comparison will count how many possible floating point values are between
    ///     the left and the right number. If the number of possible values between both
    ///     numbers is less than or equal to maxUlps, then the numbers are considered as
    ///     being equal.
    ///   </para>
    ///   <para>
    ///     Implementation partially follows the code outlined here (link now defunct):
    ///     http://www.anttirt.net/2007/08/19/proper-floating-point-comparisons/
    ///     And here:
    ///     http://www.altdevblogaday.com/2012/02/22/comparing-floating-point-numbers-2012-edition/
    ///   </para>
    /// </remarks>
    public static bool AreAlmostEqual(float left, float right, int maxUlps) {
      var leftUnion = new FloatIntUnion();
      var rightUnion = new FloatIntUnion();

      leftUnion.Float = left;
      rightUnion.Float = right;

      if(leftUnion.Int < 0) {
        leftUnion.Int = unchecked((int)0x80000000 - leftUnion.Int);
      }
      if(rightUnion.Int < 0) {
        rightUnion.Int = unchecked((int)0x80000000 - rightUnion.Int);
      }

      return Math.Abs(rightUnion.Int - leftUnion.Int) <= maxUlps;
    }
#if false
    public static bool OldAreAlmostEqual(float left, float right, int maxUlps) {
      FloatInt32Union leftUnion = new FloatInt32Union();
      FloatInt32Union rightUnion = new FloatInt32Union();

      leftUnion.Float = left;
      rightUnion.Float = right;

      uint leftSignMask = (leftUnion.UInt >> 31);
      uint rightSignMask = (rightUnion.UInt >> 31);

      uint leftTemp = ((0x80000000 - leftUnion.UInt) & leftSignMask);
      leftUnion.UInt = leftTemp | (leftUnion.UInt & ~leftSignMask);

      uint rightTemp = ((0x80000000 - rightUnion.UInt) & rightSignMask);
      rightUnion.UInt = rightTemp | (rightUnion.UInt & ~rightSignMask);

      return (Math.Abs(leftUnion.Int - rightUnion.Int) <= maxUlps);
    }
#endif

    /// <summary>Compares two double precision floating point values for equality</summary>
    /// <param name="left">First double precision floating point value to be compared</param>
    /// <param name="right">Second double precision floating point value t be compared</param>
    /// <param name="maxUlps">
    ///   Maximum number of representable double precision floating point values that are
    ///   allowed to be between the left and the right double precision floating point values
    /// </param>
    /// <returns>True if both numbers are equal or close to being equal</returns>
    /// <remarks>
    ///   <para>
    ///     Double precision floating point values can only represent a limited series of
    ///     natural numbers. For example, the values 2.0000000000000000 and 2.0000000000000004
    ///     can be stored in a double, but nothing inbetween them.
    ///   </para>
    ///   <para>
    ///     This comparison will count how many possible double precision floating point
    ///     values are between the left and the right number. If the number of possible
    ///     values between both numbers is less than or equal to maxUlps, then the numbers
    ///     are considered as being equal.
    ///   </para>
    ///   <para>
    ///     Implementation partially follows the code outlined here:
    ///     http://www.anttirt.net/2007/08/19/proper-floating-point-comparisons/
    ///     And here:
    ///     http://www.altdevblogaday.com/2012/02/22/comparing-floating-point-numbers-2012-edition/
    ///   </para>
    /// </remarks>
    public static bool AreAlmostEqual(double left, double right, long maxUlps) {
      var leftUnion = new DoubleLongUnion();
      var rightUnion = new DoubleLongUnion();

      leftUnion.Double = left;
      rightUnion.Double = right;

      if(leftUnion.Long < 0) {
        leftUnion.Long = unchecked((long)0x8000000000000000 - leftUnion.Long);
      }
      if(rightUnion.Long < 0) {
        rightUnion.Long = unchecked((long)0x8000000000000000 - rightUnion.Long);
      }

      return Math.Abs(rightUnion.Long - leftUnion.Long) <= maxUlps;
    }
#if false
    public static bool OldAreAlmostEqual(double left, double right, long maxUlps) {
      DoubleInt64Union leftUnion = new DoubleInt64Union();
      DoubleInt64Union rightUnion = new DoubleInt64Union();

      leftUnion.Double = left;
      rightUnion.Double = right;

      ulong leftSignMask = (leftUnion.ULong >> 63);
      ulong rightSignMask = (rightUnion.ULong >> 63);

      ulong leftTemp = ((0x8000000000000000 - leftUnion.ULong) & leftSignMask);
      leftUnion.ULong = leftTemp | (leftUnion.ULong & ~leftSignMask);

      ulong rightTemp = ((0x8000000000000000 - rightUnion.ULong) & rightSignMask);
      rightUnion.ULong = rightTemp | (rightUnion.ULong & ~rightSignMask);

      return (Math.Abs(leftUnion.Long - rightUnion.Long) <= maxUlps);
    }
#endif

    /// <summary>
    ///   Reinterprets the memory contents of a floating point value as an integer value
    /// </summary>
    /// <param name="value">
    ///   Floating point value whose memory contents to reinterpret
    /// </param>
    /// <returns>
    ///   The memory contents of the floating point value interpreted as an integer
    /// </returns>
    public static int ReinterpretAsInt(this float value) {
      FloatIntUnion union = new FloatIntUnion();
      union.Float = value;
      return union.Int;
    }

    /// <summary>
    ///   Reinterprets the memory contents of a double precision floating point
    ///   value as an integer value
    /// </summary>
    /// <param name="value">
    ///   Double precision floating point value whose memory contents to reinterpret
    /// </param>
    /// <returns>
    ///   The memory contents of the double precision floating point value
    ///   interpreted as an integer
    /// </returns>
    public static long ReinterpretAsLong(this double value) {
      DoubleLongUnion union = new DoubleLongUnion();
      union.Double = value;
      return union.Long;
    }

    /// <summary>
    ///   Reinterprets the memory contents of an integer as a floating point value
    /// </summary>
    /// <param name="value">Integer value whose memory contents to reinterpret</param>
    /// <returns>
    ///   The memory contents of the integer value interpreted as a floating point value
    /// </returns>
    public static float ReinterpretAsFloat(this int value) {
      FloatIntUnion union = new FloatIntUnion();
      union.Int = value;
      return union.Float;
    }

    /// <summary>
    ///   Reinterprets the memory contents of an integer value as a double precision
    ///   floating point value
    /// </summary>
    /// <param name="value">Integer whose memory contents to reinterpret</param>
    /// <returns>
    ///   The memory contents of the integer interpreted as a double precision
    ///   floating point value
    /// </returns>
    public static double ReinterpretAsDouble(this long value) {
      DoubleLongUnion union = new DoubleLongUnion();
      union.Long = value;
      return union.Double;
    }

  }

} // namespace Nuclex.Support
