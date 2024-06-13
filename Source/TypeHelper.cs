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
using System.Collections.Generic;
using System.Reflection;

namespace Nuclex.Support {

  /// <summary>Helper methods for the reflection Type class</summary>
  public static class TypeHelper {

    #region class FieldInfoComparer

    /// <summary>Determines whether member informations relate to the same member</summary>
    private class FieldInfoComparer : IEqualityComparer<FieldInfo> {

      /// <summary>Default instance of the comparer</summary>
      public static readonly FieldInfoComparer Default = new FieldInfoComparer();

      /// <summary>Checks whether two member informations are equal</summary>
      /// <param name="left">Informations about the left member in the comaprison</param>
      /// <param name="right">Informations about the right member in the comparison</param>
      /// <returns>True if the two member informations relate to the same member</returns>
      public bool Equals(FieldInfo left, FieldInfo right) {
        return
          (left.DeclaringType == right.DeclaringType) &&
          (left.Name == right.Name);
      }

      /// <summary>Determines the hash code of the specified member informations</summary>
      /// <param name="FieldInfo">
      ///   Member informations whose hash code will be determined
      /// </param>
      /// <returns>The hash code of the specified member informations</returns>
      public int GetHashCode(FieldInfo FieldInfo) {
        return (FieldInfo.DeclaringType.GetHashCode() ^ FieldInfo.Name.GetHashCode());
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
        var fieldInfoSet = new Dictionary<FieldInfo, object>(FieldInfoComparer.Default);
        for(int index = 0; index < fieldInfos.Length; ++index) {
          fieldInfoSet.Add(fieldInfos[index], null);
        }

        while(type.BaseType != typeof(object)) {
          type = type.BaseType;
          fieldInfos = type.GetFields(bindingFlags);

          for(int index = 0; index < fieldInfos.Length; ++index) {
            addIfNotExists(fieldInfoSet, fieldInfos[index]);
          }
        }

        fieldInfos = new FieldInfo[fieldInfoSet.Count];
        fieldInfoSet.Keys.CopyTo(fieldInfos, 0);
      }

      return fieldInfos;
    }

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

    /// <summary>Determines whether the type has the specified attribute</summary>
    /// <typeparam name="TAttribute">Attribute the type will be checked for</typeparam>
    /// <param name="type">
    ///   Type that will be checked for presence of the specified attribute
    /// </param>
    /// <returns>True if the type has the specified attribute, otherwise false</returns>
    public static bool HasAttribute<TAttribute>(this Type type) {
      return type.HasAttribute(typeof(TAttribute));
    }

    /// <summary>Determines whether the type has the specified attribute</summary>
    /// <param name="type">
    ///   Type that will be checked for presence of the specified attribute
    /// </param>
    /// <param name="attributeType">Attribute the type will be checked for</param>
    /// <returns>True if the type has the specified attribute, otherwise false</returns>
    public static bool HasAttribute(this Type type, Type attributeType) {
      object[] attributes = type.GetCustomAttributes(attributeType, true);
      return (attributes != null) && (attributes.Length > 0);
    }

    /// <summary>
    ///   Adds field informations to a list if they're not already contained in it
    /// </summary>
    /// <param name="fieldInfos">List the field informations will be added to</param>
    /// <param name="fieldInfo">Field informations that will be added to the list</param>
    private static void addIfNotExists(
      IDictionary<FieldInfo, object> fieldInfos, FieldInfo fieldInfo
    ) {
      if(!fieldInfos.ContainsKey(fieldInfo)) {
        fieldInfos.Add(fieldInfo, null);
      }
    }

  }

} // namespace Nuclex.Support
