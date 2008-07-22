using System;
using System.Collections.Generic;

namespace Nuclex.Support {

  /// <summary>Helper methods for working with integer types</summary>
  public static class IntegerHelper {

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static long NextPowerOf2(long value) {
      return (long)NextPowerOf2((ulong)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static ulong NextPowerOf2(ulong value) {
      if(value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      value |= value >> 32;
      ++value;

      return value;
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static int NextPowerOf2(int value) {
      return (int)NextPowerOf2((uint)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static uint NextPowerOf2(uint value) {
      if(value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      ++value;

      return value;
    }

  }

} // namespace Nuclex.Support
