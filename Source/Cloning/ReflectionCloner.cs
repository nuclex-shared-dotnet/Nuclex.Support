#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.Reflection;

namespace Nuclex.Support.Cloning {

  // TODO: Doesn't clone arrays yet

  /// <summary>Clones objects using reflection</summary>
  public class ReflectionCloner : ICloneFactory, IStateCopier {

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <param name="usePropertyBasedClone">
    ///   Whether to clone the object based on its properties only
    /// </param>
    /// <returns>A shallow clone of the provided object</returns>
    public TCloned ShallowClone<TCloned>(TCloned objectToClone, bool usePropertyBasedClone) {
      if(typeof(TCloned).IsValueType) {
        TCloned clone = Activator.CreateInstance<TCloned>();
        if(usePropertyBasedClone) {
          shallowCopyValueTypePropertyBased<TCloned>(ref objectToClone, ref clone);
        } else {
          shallowCopyValueTypeFieldBased<TCloned>(ref objectToClone, ref clone);
        }
        return clone;
      } else {
        TCloned clone = (TCloned)Activator.CreateInstance(objectToClone.GetType());
        if(usePropertyBasedClone) {
          shallowCopyReferenceTypePropertyBased(objectToClone.GetType(), objectToClone, clone);
        } else {
          shallowCopyReferenceTypeFieldBased(objectToClone.GetType(), objectToClone, clone);
        }
        return clone;
      }
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <param name="usePropertyBasedClone">
    ///   Whether to clone the object based on its properties only
    /// </param>
    /// <returns>A deep clone of the provided object</returns>
    public TCloned DeepClone<TCloned>(TCloned objectToClone, bool usePropertyBasedClone) {
      if(typeof(TCloned).IsValueType) {
        TCloned clone = Activator.CreateInstance<TCloned>();
        if(usePropertyBasedClone) {
          deepCopyValueTypePropertyBased<TCloned>(ref objectToClone, ref clone);
        } else {
          deepCopyValueTypeFieldBased<TCloned>(ref objectToClone, ref clone);
        }
        return clone;
      } else {
        TCloned clone = (TCloned)Activator.CreateInstance(objectToClone.GetType());
        if(usePropertyBasedClone) {
          deepCopyReferenceTypePropertyBased(objectToClone.GetType(), objectToClone, clone);
        } else {
          deepCopyReferenceTypeFieldBased(objectToClone.GetType(), objectToClone, clone);
        }
        return clone;
      }
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      if(propertyBased) {
        shallowCopyReferenceTypePropertyBased(typeof(TState), original, target);
      } else {
        shallowCopyReferenceTypeFieldBased(typeof(TState), original, target);
      }
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(
      ref TState original, ref TState target, bool propertyBased
    ) where TState : struct {
      if(propertyBased) {
        shallowCopyValueTypePropertyBased<TState>(ref original, ref target);
      } else {
        shallowCopyValueTypeFieldBased<TState>(ref original, ref target);
      }
    }

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void DeepCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      if(propertyBased) {
        deepCopyReferenceTypePropertyBased(typeof(TState), original, target);
      } else {
        deepCopyReferenceTypeFieldBased(typeof(TState), original, target);
      }
    }

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void DeepCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct {
      if(propertyBased) {
        deepCopyValueTypePropertyBased<TState>(ref original, ref target);
      } else {
        deepCopyValueTypeFieldBased<TState>(ref original, ref target);
      }
    }

    /// <summary>Creates a field-based shallow copy of a reference type</summary>
    /// <param name="copiedType">Type the copy will be based upon</param>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void shallowCopyReferenceTypeFieldBased(
      Type copiedType, object original, object target
    ) {
      FieldInfo[] fieldInfos = copiedType.GetFields(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        fieldInfo.SetValue(target, fieldInfo.GetValue(original));
      }
    }

    /// <summary>Creates a property-based shallow copy of a reference type</summary>
    /// <param name="copiedType">Type the copy will be based upon</param>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void shallowCopyReferenceTypePropertyBased(
      Type copiedType, object original, object target
    ) {
      PropertyInfo[] propertyInfos = copiedType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          if(propertyInfo.PropertyType.IsPrimitive) {
            propertyInfo.SetValue(
              target,
              propertyInfo.GetValue(original, null),
              null
            );
          } else if(propertyInfo.PropertyType.IsValueType) {
            // Recurse into the value type - value types are seen as part of
            // the outer type and a shallow copy does follow their hierarchy.
            // This is equivalent to what would happen if you directly assign
            // one value type to another.
            object boxedOriginalValue = propertyInfo.GetValue(original, null);
            Type originalType = boxedOriginalValue.GetType();
            object boxedClonedValue = Activator.CreateInstance(originalType);
            shallowCopyReferenceTypePropertyBased(
              originalType, boxedOriginalValue, boxedClonedValue
            );
            propertyInfo.SetValue(target, boxedClonedValue, null);
          } else {
            propertyInfo.SetValue(
              target,
              propertyInfo.GetValue(original, null),
              null
            );
          }
        }
      }
    }

    /// <summary>Creates a property-based shallow copy of a value type</summary>
    /// <typeparam name="TState">Value type that will be copied</typeparam>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void shallowCopyValueTypePropertyBased<TState>(
      ref TState original, ref TState target
    ) {
      object boxedOriginal = original;
      object boxedTarget = target;

      PropertyInfo[] propertyInfos = typeof(TState).GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];

        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          if(propertyInfo.PropertyType.IsPrimitive) {
            propertyInfo.SetValue(
              boxedTarget,
              propertyInfo.GetValue(boxedOriginal, null),
              null
            );
          } else if(propertyInfo.PropertyType.IsValueType) {
            object boxedOriginalValue = propertyInfo.GetValue(original, null);
            Type originalType = boxedOriginalValue.GetType();
            object boxedClonedValue = Activator.CreateInstance(originalType);
            deepCopyReferenceTypePropertyBased(
              originalType, boxedOriginalValue, boxedClonedValue
            );
            propertyInfo.SetValue(boxedTarget, boxedClonedValue, null);
          } else {
            propertyInfo.SetValue(
              boxedTarget,
              propertyInfo.GetValue(boxedOriginal, null),
              null
            );
          }
        }
      }

      target = (TState)boxedTarget;
    }

    /// <summary>Creates a field-based shallow copy of a value type</summary>
    /// <typeparam name="TState">Value type that will be copied</typeparam>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void shallowCopyValueTypeFieldBased<TState>(
      ref TState original, ref TState target
    ) {
      target = original; // hehe
    }

    /// <summary>Creates a field-based deep copy of a reference type</summary>
    /// <param name="copiedType">Type the copy will be based upon</param>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void deepCopyReferenceTypeFieldBased(
      Type copiedType, object original, object target
    ) {
      FieldInfo[] fieldInfos = copiedType.GetFields(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        if(fieldInfo.FieldType.IsPrimitive) {
          fieldInfo.SetValue(target, fieldInfo.GetValue(original));
        } else if(fieldInfo.FieldType.IsValueType) {
          object boxedOriginalValue = fieldInfo.GetValue(original);
          Type originalType = boxedOriginalValue.GetType();
          object boxedClonedValue = Activator.CreateInstance(originalType);
          deepCopyReferenceTypeFieldBased(originalType, boxedOriginalValue, boxedClonedValue);
          fieldInfo.SetValue(target, boxedClonedValue);
        } else {
          object originalValue = fieldInfo.GetValue(original);
          if(originalValue != null) {
            Type originalType = originalValue.GetType();
            object clonedValue = Activator.CreateInstance(originalType);
            deepCopyReferenceTypeFieldBased(originalType, originalValue, clonedValue);
            fieldInfo.SetValue(target, clonedValue);
          }
        }
      }
    }

    /// <summary>Creates a property-based deep copy of a reference type</summary>
    /// <param name="copiedType">Type the copy will be based upon</param>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void deepCopyReferenceTypePropertyBased(
      Type copiedType, object original, object target
    ) {
      PropertyInfo[] propertyInfos = copiedType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          if(propertyInfo.PropertyType.IsPrimitive) {
            propertyInfo.SetValue(
              target,
              propertyInfo.GetValue(original, null),
              null
            );
          } else if(propertyInfo.PropertyType.IsValueType) {
            object boxedOriginalValue = propertyInfo.GetValue(original, null);
            Type originalType = boxedOriginalValue.GetType();
            object boxedClonedValue = Activator.CreateInstance(originalType);
            deepCopyReferenceTypePropertyBased(
              originalType, boxedOriginalValue, boxedClonedValue
            );
            propertyInfo.SetValue(target, boxedClonedValue, null);
          } else {
            object originalValue = propertyInfo.GetValue(original, null);
            if(originalValue != null) {
              Type originalType = originalValue.GetType();
              object clonedValue = Activator.CreateInstance(originalType);
              deepCopyReferenceTypePropertyBased(
                originalType, originalValue, clonedValue
              );
              propertyInfo.SetValue(target, clonedValue, null);
            }
          }
        }
      }
    }

    /// <summary>Creates a field-based deep copy of a value type</summary>
    /// <typeparam name="TState">Value type that will be copied</typeparam>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void deepCopyValueTypeFieldBased<TState>(
      ref TState original, ref TState target
    ) {
      object boxedOriginal = original;
      object boxedTarget = Activator.CreateInstance(original.GetType());

      FieldInfo[] fieldInfos = typeof(TState).GetFields(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        if(fieldInfo.FieldType.IsPrimitive) {
          object boxedValue = fieldInfo.GetValue(boxedOriginal);
          fieldInfo.SetValue(boxedTarget, boxedValue);
        } else if(fieldInfo.FieldType.IsValueType) {
          object boxedOriginalValue = fieldInfo.GetValue(boxedOriginal);
          Type originalType = boxedOriginalValue.GetType();
          object boxedClonedValue = Activator.CreateInstance(originalType);
          deepCopyReferenceTypeFieldBased(originalType, boxedOriginalValue, boxedClonedValue);
          fieldInfo.SetValue(boxedTarget, boxedClonedValue);
        } else {
          object originalValue = fieldInfo.GetValue(boxedOriginal);
          if(originalValue != null) {
            Type originalType = originalValue.GetType();
            object clonedValue = Activator.CreateInstance(originalType);
            deepCopyReferenceTypeFieldBased(originalType, originalValue, clonedValue);
            fieldInfo.SetValue(boxedTarget, clonedValue);
          }
        }
      }

      target = (TState)boxedTarget;
    }

    /// <summary>Creates a property-based shallow copy of a value type</summary>
    /// <typeparam name="TState">Value type that will be copied</typeparam>
    /// <param name="original">Original object that will be copied</param>
    /// <param name="target">Target object into which copied values will be written</param>
    private static void deepCopyValueTypePropertyBased<TState>(
      ref TState original, ref TState target
    ) {
      object boxedOriginal = original;
      object boxedTarget = Activator.CreateInstance(original.GetType());

      PropertyInfo[] propertyInfos = typeof(TState).GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          if(propertyInfo.PropertyType.IsPrimitive) {
            propertyInfo.SetValue(
              boxedTarget,
              propertyInfo.GetValue(boxedOriginal, null),
              null
            );
          } else if(propertyInfo.PropertyType.IsValueType) {
            object boxedOriginalValue = propertyInfo.GetValue(boxedOriginal, null);
            Type originalType = boxedOriginalValue.GetType();
            object boxedClonedValue = Activator.CreateInstance(originalType);
            deepCopyReferenceTypePropertyBased(
              originalType, boxedOriginalValue, boxedClonedValue
            );
            propertyInfo.SetValue(boxedTarget, boxedClonedValue, null);
          } else {
            object originalValue = propertyInfo.GetValue(boxedOriginal, null);
            if(originalValue != null) {
              Type originalType = originalValue.GetType();
              object clonedValue = Activator.CreateInstance(originalType);
              deepCopyReferenceTypePropertyBased(originalType, originalValue, clonedValue);
              propertyInfo.SetValue(boxedTarget, clonedValue, null);
            }
          }
        }
      }

      target = (TState)boxedTarget;
    }

  }

} // namespace Nuclex.Support.Cloning
