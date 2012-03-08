﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2012 Nuclex Development Labs

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
using System.Reflection;

namespace Nuclex.Support {

  /// <summary>Helper methods for the reflection Type class</summary>
  public static class TypeHelper {

#if !(XBOX360 || WINDOWS_PHONE)

    #region class MemberInfoComparer

    /// <summary>Determines whether member informations relate to the same member</summary>
    private class MemberInfoComparer : IEqualityComparer<MemberInfo> {

      /// <summary>Default instance of the comparer</summary>
      public static readonly MemberInfoComparer Default = new MemberInfoComparer();

      /// <summary>Checks whether two member informations are equal</summary>
      /// <param name="left">Informations about the left member in the comaprison</param>
      /// <param name="right">Informations about the right member in the comparison</param>
      /// <returns>True if the two member informations relate to the same member</returns>
      public bool Equals(MemberInfo left, MemberInfo right) {
        return
          (left.DeclaringType == right.DeclaringType) &&
          (left.Name == right.Name);
      }

      /// <summary>Determines the hash code of the specified member informations</summary>
      /// <param name="memberInfo">
      ///   Member informations whose hash code will be determined
      /// </param>
      /// <returns>The hash code of the specified member informations</returns>
      public int GetHashCode(MemberInfo memberInfo) {
        return (memberInfo.DeclaringType.GetHashCode() ^ memberInfo.Name.GetHashCode());
      }

    }

    #endregion // class MemberInfoComparer

    /// <summary>
    ///   Returns all the fields of a type, including those defined in the type's base classes
    /// </summary>
    /// <param name="type">Type whose fields will be returned</param>
    /// <param name="bindingFlags">Binding flags to use when querying the fields</param>
    /// <returns>All of the type's fields, including its base types</returns>
    public static FieldInfo[] GetFieldInfosIncludingBaseClasses(
      this Type type, BindingFlags bindingFlags
    ) {
      FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

      // If this class doesn't have a base, don't waste any time
      if(type.BaseType != typeof(object)) {
        var fieldInfoSet = new HashSet<FieldInfo>(fieldInfos, MemberInfoComparer.Default);
        while(type.BaseType != typeof(object)) {
          type = type.BaseType;
          fieldInfos = type.GetFields(bindingFlags);
          for(int index = 0; index < fieldInfos.Length; ++index) {
            fieldInfoSet.Add(fieldInfos[index]);
          }
        }

        fieldInfos = new FieldInfo[fieldInfoSet.Count];
        fieldInfoSet.CopyTo(fieldInfos);
      }

      return fieldInfos;
    }

#else // !(XBOX360 || WINDOWS_PHONE)

    /// <summary>
    ///   Returns all the fields of a type, including those defined in the type's base classes
    /// </summary>
    /// <param name="type">Type whose fields will be returned</param>
    /// <param name="bindingFlags">Binding flags to use when querying the fields</param>
    /// <returns>All of the type's fields, including its base types</returns>
    public static FieldInfo[] GetFieldInfosIncludingBaseClasses(
      this Type type, BindingFlags bindingFlags
    ) {
      FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

      // If this class doesn't have a base, don't waste any time
      if(type.BaseType != typeof(object)) {
        var fieldInfoList = new List<FieldInfo>(fieldInfos.Length * 2);
        for(int index = 0; index < fieldInfos.Length; ++index) {
          fieldInfoList.Add(fieldInfos[index]);
        }

        while(type.BaseType != typeof(object)) {
          type = type.BaseType;
          fieldInfos = type.GetFields(bindingFlags);

          for(int index = 0; index < fieldInfos.Length; ++index) {
            addIfNotExists(fieldInfoList, fieldInfos[index]);
          }
        }

        fieldInfos = fieldInfoList.ToArray();
      }

      return fieldInfos;
    }

    /// <summary>
    ///   Adds field informations to a list if they're not already contained in it
    /// </summary>
    /// <param name="fieldInfos">List the field informations will be added to</param>
    /// <param name="fieldInfo">Field informations that will be added to the list</param>
    private static void addIfNotExists(IList<FieldInfo> fieldInfos, FieldInfo fieldInfo) {
      bool matchFound = false;

      for(int index = 0; index < fieldInfos.Count; ++index) {
        FieldInfo current = fieldInfos[index];

        matchFound =
          (current.DeclaringType == fieldInfo.DeclaringType) &&
          (current.Name == fieldInfo.Name);

        if(matchFound) {
          break;
        }
      }

      if(!matchFound) {
        fieldInfos.Add(fieldInfo);
      }
    }

#endif // !(XBOX360 || WINDOWS_PHONE)

    /// <summary>Determines whether the given type has a default constructor</summary>
    /// <param name="type">Type which is to be checked</param>
    /// <returns>True if the type has a default constructor</returns>
    public static bool HasDefaultConstructor(this Type type) {
      ConstructorInfo[] constructors = type.GetConstructors();

      for(int index = 0; index < constructors.Length; ++index) {
        ConstructorInfo constructor = constructors[index];
        if(constructor.IsPublic && (constructor.GetParameters().Length == 0)) {
          return true;
        }
      }

      return false;
    }

  }

} // namespace Nuclex.Support
