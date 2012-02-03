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

#if !(XBOX360 || WINDOWS_PHONE)

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Nuclex.Support.Cloning {

  /// <summary>An action that takes its arguments as references to a structure</summary>
  /// <typeparam name="TFirst">Type of the first argument to the method</typeparam>
  /// <typeparam name="TSecond">Type of the second argument to the method</typeparam>
  /// <param name="first">First argument to the method</param>
  /// <param name="second">Second argument to the method</param>
  public delegate void ReferenceAction<TFirst, TSecond>(ref TFirst first, ref TSecond second)
    where TFirst : struct
    where TSecond : struct;

  /// <summary>
  ///   Cloning factory which uses expression trees to improve performance when cloning
  ///   is a high-frequency action.
  /// </summary>
  public class ExpressionTreeCloner : ICloneFactory {

    /// <summary>Initializes the static members of the expression tree cloner</summary>
    static ExpressionTreeCloner() {
      shallowCloners = new ConcurrentDictionary<Type, Delegate>();
      deepCloners = new ConcurrentDictionary<Type, Delegate>();
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
    public static TCloned DeepClone<TCloned>(
      TCloned objectToClone, bool usePropertyBasedClone
    ) {
      if(usePropertyBasedClone) {
        throw new NotImplementedException("Not implemented yet");
      } else {
        Func<TCloned, TCloned> cloner = getOrCreateDeepFieldBasedCloner<TCloned>();
        return cloner(objectToClone);
      }
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <param name="usePropertyBasedClone">
    ///   Whether to clone the object based on its properties only
    /// </param>
    /// <returns>A shallow clone of the provided object</returns>
    public static TCloned ShallowClone<TCloned>(
      TCloned objectToClone, bool usePropertyBasedClone
    ) {
      throw new NotImplementedException("Not implemented yet");
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
    TCloned ICloneFactory.DeepClone<TCloned>(
      TCloned objectToClone, bool usePropertyBasedClone
    ) {
      return ExpressionTreeCloner.DeepClone<TCloned>(objectToClone, usePropertyBasedClone);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <param name="usePropertyBasedClone">
    ///   Whether to clone the object based on its properties only
    /// </param>
    /// <returns>A shallow clone of the provided object</returns>
    TCloned ICloneFactory.ShallowClone<TCloned>(
      TCloned objectToClone, bool usePropertyBasedClone
    ) {
      return ExpressionTreeCloner.ShallowClone<TCloned>(objectToClone, usePropertyBasedClone);
    }

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <typeparam name="TCloned">Type for which a clone method will be retrieved</typeparam>
    /// <returns>The clone method for the specified type</returns>
    private static Func<TCloned, TCloned> getOrCreateDeepFieldBasedCloner<TCloned>() {
      Type clonedType = typeof(TCloned);
      Delegate clonerAsDelegate;
      if(deepCloners.TryGetValue(clonedType, out clonerAsDelegate)) {
        return (Func<TCloned, TCloned>)clonerAsDelegate;
      } else {
        Func<TCloned, TCloned> cloner = createDeepFieldBasedCloner<TCloned>();
        deepCloners.TryAdd(clonedType, cloner);
        return cloner;
      }
    }

    /// <summary>Compiles a method that creates a clone of an object</summary>
    /// <param name="type">Type for which a clone method will be created</param>
    /// <returns>A method that clones an object of the provided type</returns>
    private static Func<TCloned, TCloned> createDeepFieldBasedCloner<TCloned>() {
      Type clonedType = typeof(TCloned);
      FieldInfo[] fieldInfos = clonedType.GetFields(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );

      ParameterExpression original = Expression.Parameter(typeof(TCloned), "original");
      ParameterExpression clone = Expression.Variable(typeof(TCloned), "clone");

      var transferExpressions = new List<Expression>();

      if(clonedType.IsPrimitive || (clonedType == typeof(string))) {
        transferExpressions.Add(Expression.Assign(clone, original));
      } else if(clonedType.IsArray) {
        throw new NotImplementedException("Not implemented yet");
      } else {
        transferExpressions.Add(Expression.Assign(clone, Expression.New(clonedType)));

        for(int index = 0; index < fieldInfos.Length; ++index) {
          FieldInfo fieldInfo = fieldInfos[index];
          Type fieldType = fieldInfo.FieldType;

          if(fieldType.IsPrimitive) {
            transferExpressions.Add(
              Expression.Assign(
                Expression.Field(clone, fieldInfo),
                Expression.Field(original, fieldInfo)
              )
            );
          }
        }

        transferExpressions.Add(clone);
      }

      Expression<Func<TCloned, TCloned>> expression =
        Expression.Lambda<Func<TCloned, TCloned>>(
          Expression.Block(
            new[] { clone },
            transferExpressions
          ),
          original
        );

      return expression.Compile();
    }

#if false
    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void DeepCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      throw new NotImplementedException();
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Compiles a method that copies the state of one object into another object
    /// </summary>
    /// <typeparam name="TCloned">Type of object whose state will be copied</typeparam>
    /// <param name="deepClone">Whether to create clones of the referenced objects</param>
    /// <returns>A method that copies the state from one object into another object</returns>
    public static Action<TCloned, TCloned> CreateReferenceCopier<TCloned>(bool deepClone)
      where TCloned : class {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Compiles a method that copies the state of one object into another object
    /// </summary>
    /// <typeparam name="TCloned">Type of object whose state will be copied</typeparam>
    /// <param name="deepClone">Whether to create clones of the referenced objects</param>
    /// <returns>A method that copies the state from one object into another object</returns>
    public static ReferenceAction<TCloned, TCloned> CreateValueCopier<TCloned>(bool deepClone)
      where TCloned : struct {
      throw new NotImplementedException();
    }

    /// <summary>Compiles a method that creates a clone of an object</summary>
    /// <typeparam name="TCloned">Type of object that will be cloned</typeparam>
    /// <param name="deepClone">Whether to create clones of the referenced objects</param>
    /// <returns>A method that clones an object of the provided type</returns>
    public static Func<TCloned, TCloned> CreateCloner<TCloned>(bool deepClone)
      where TCloned : class, new() {
      throw new NotImplementedException();
    }
#endif

    /// <summary>Compiled cloners that perform shallow clone operations</summary>
    private static ConcurrentDictionary<Type, Delegate> shallowCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Delegate> deepCloners;

  }

} // namespace Nuclex.Support.Cloning

#endif // !(XBOX360 || WINDOWS_PHONE)
