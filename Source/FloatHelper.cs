using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nuclex.Support {

  /// <summary>Helper routines for working with floating point numbers</summary>
  /// <remarks>
  ///   The floating point comparison code is based on this excellent article:
  ///   http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm
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
    ///     Floating point values can only represent a limited series of natural numbers.
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
    ///     Implementation partially follows the code outlined here:
    ///     http://www.anttirt.net/2007/08/19/proper-floating-point-comparisons/
    ///   </para>
    /// </remarks>
    public static bool AreAlmostEqual(float left, float right, int maxUlps) {
      FloatIntUnion leftUnion = new FloatIntUnion();
      FloatIntUnion rightUnion = new FloatIntUnion();

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
    ///   </para>
    /// </remarks>
    public static bool AreAlmostEqual(double left, double right, long maxUlps) {
      DoubleLongUnion leftUnion = new DoubleLongUnion();
      DoubleLongUnion rightUnion = new DoubleLongUnion();

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

  }

} // namespace Nuclex.Support
