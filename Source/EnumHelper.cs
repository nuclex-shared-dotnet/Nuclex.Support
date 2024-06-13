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

namespace Nuclex.Support {

  /// <summary>Helper methods for enumerations</summary>
  public static class EnumHelper {

    /// <summary>Returns the highest value encountered in an enumeration</summary>
    /// <typeparam name="TEnumeration">
    ///   Enumeration of which the highest value will be returned
    /// </typeparam>
    /// <returns>The highest value in the enumeration</returns>
    public static TEnumeration GetHighestValue<TEnumeration>()
      where TEnumeration : IComparable {
      TEnumeration[] values = GetValues<TEnumeration>();

      // If the enumeration is empty, return nothing
      if(values.Length == 0) {
        return default(TEnumeration);
      }

      // Look for the highest value in the enumeration. We initialize the highest value
      // to the first enumeration value so we don't have to use some arbitrary starting
      // value which might actually appear in the enumeration.
      TEnumeration highestValue = values[0];
      for(int index = 1; index < values.Length; ++index) {
        if(values[index].CompareTo(highestValue) > 0) {
          highestValue = values[index];
        }
      }

      return highestValue;
    }

    /// <summary>Returns the lowest value encountered in an enumeration</summary>
    /// <typeparam name="TEnumeration">
    ///   Enumeration of which the lowest value will be returned
    /// </typeparam>
    /// <returns>The lowest value in the enumeration</returns>
    public static TEnumeration GetLowestValue<TEnumeration>()
      where TEnumeration : IComparable {
      TEnumeration[] values = GetValues<TEnumeration>();

      // If the enumeration is empty, return nothing
      if(values.Length == 0) {
        return default(TEnumeration);
      }

      // Look for the lowest value in the enumeration. We initialize the lowest value
      // to the first enumeration value so we don't have to use some arbitrary starting
      // value which might actually appear in the enumeration.
      TEnumeration lowestValue = values[0];
      for(int index = 1; index < values.Length; ++index) {
        if(values[index].CompareTo(lowestValue) < 0) {
          lowestValue = values[index];
        }
      }

      return lowestValue;
    }

    /// <summary>Retrieves a list of all values contained in an enumeration</summary>
    /// <typeparam name="TEnum">
    ///   Type of the enumeration whose values will be returned
    /// </typeparam>
    /// <returns>All values contained in the specified enumeration</returns>
    /// <remarks>
    ///   This method produces collectable garbage so it's best to only call it once
    ///   and cache the result.
    /// </remarks>
    public static TEnum[] GetValues<TEnum>() {
      return (TEnum[])Enum.GetValues(typeof(TEnum));
    }

  }

} // namespace Nuclex.Support