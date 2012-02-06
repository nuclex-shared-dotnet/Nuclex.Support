﻿#region CPL License
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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
      shallowCloners = new ConcurrentDictionary<Type, Func<object, object>>();
      deepCloners = new ConcurrentDictionary<Type, Func<object, object>>();
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
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      if(usePropertyBasedClone) {
        throw new NotImplementedException("Not implemented yet");
      } else {
        Func<object, object> cloner = getOrCreateDeepFieldBasedCloner(typeof(TCloned));
        return (TCloned)cloner(objectToCloneAsObject);
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
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateDeepFieldBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!deepCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createDeepFieldBasedCloner(clonedType);
        deepCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>
    ///   Generates state transfer expressions to copy an array of primitive types
    /// </summary>
    /// <param name="clonedType">Type of array that will be cloned</param>
    /// <param name="original">Variable expression for the original array</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    /// <returns>The variable holding the cloned array</returns>
    private static Expression generatePrimitiveArrayTransferExpressions(
      Type clonedType,
      Expression original,
      ICollection<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      MethodInfo arrayCloneMethodInfo = typeof(Array).GetMethod("Clone");
      return Expression.Convert(
        Expression.Call(
          Expression.Convert(original, typeof(Array)), arrayCloneMethodInfo
        ),
        clonedType
      );
    }

    /// <summary>
    ///   Generates state transfer expressions to copy an array of complex types
    /// </summary>
    /// <param name="clonedType">Type of array that will be cloned</param>
    /// <param name="original">Variable expression for the original array</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    /// <returns>The variable holding the cloned array</returns>
    private static ParameterExpression generateComplexArrayTransferExpressions(
      Type clonedType,
      Expression original,
      IList<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      // We need a temporary variable in order to transfer the elements of the array
      ParameterExpression clone = Expression.Variable(clonedType);
      variables.Add(clone);
      ParameterExpression typedOriginal = Expression.Variable(clonedType);
      variables.Add(typedOriginal);

      transferExpressions.Add(
        Expression.Assign(typedOriginal, Expression.Convert(original, clonedType))
      );

      int dimensionCount = clonedType.GetArrayRank();
      int baseVariableIndex = variables.Count;
      var arrayTransferExpressions = new List<Expression>();
      Type elementType = clonedType.GetElementType();

      // Retrieve the length of each of the array's dimensions
      MethodInfo arrayGetLengthMethodInfo = typeof(Array).GetMethod("GetLength");
      for(int index = 0; index < dimensionCount; ++index) {
        ParameterExpression length = Expression.Variable(typeof(int));
        variables.Add(length);
        arrayTransferExpressions.Add(
          Expression.Assign(
            length,
            Expression.Call(
              typedOriginal, arrayGetLengthMethodInfo, Expression.Constant(index)
            )
          )
        );
      }

      // Create a new array of identical size
      switch(dimensionCount) {
        case 1: {
          MethodInfo arrayCreateInstanceMethodInfo = typeof(Array).GetMethod(
            "CreateInstance", new Type[] { typeof(Type), typeof(int) }
          );
          arrayTransferExpressions.Add(
            Expression.Assign(
              clone,
              Expression.Convert(
                Expression.Call(
                  arrayCreateInstanceMethodInfo,
                  Expression.Constant(elementType),
                  variables[baseVariableIndex]
                ),
                clonedType
              )
            )
          );
          break;
        }
        case 2: {
          MethodInfo arrayCreateInstanceMethodInfo = typeof(Array).GetMethod(
            "CreateInstance", new Type[] { typeof(Type), typeof(int), typeof(int) }
          );
          arrayTransferExpressions.Add(
            Expression.Assign(
              clone,
              Expression.Convert(
                Expression.Call(
                  arrayCreateInstanceMethodInfo,
                  Expression.Constant(elementType),
                  variables[baseVariableIndex],
                  variables[baseVariableIndex + 1]
                ),
                clonedType
              )
            )
          );
          break;
        }
        case 3: {
          MethodInfo arrayCreateInstanceMethodInfo = typeof(Array).GetMethod(
            "CreateInstance", new Type[] { typeof(Type), typeof(int), typeof(int), typeof(int) }
          );
          arrayTransferExpressions.Add(
            Expression.Assign(
              clone,
              Expression.Convert(
                Expression.Call(
                  arrayCreateInstanceMethodInfo,
                  Expression.Constant(elementType),
                  variables[baseVariableIndex],
                  variables[baseVariableIndex + 1],
                  variables[baseVariableIndex + 2]
                ),
                clonedType
              )
            )
          );
          break;
        }
        default: {
          throw new InvalidOperationException("Unsupported array dimension count");
        }
      }

      // Only execute the array transfer expressions if the array is not null
      transferExpressions.Add(
        Expression.IfThen(
          Expression.NotEqual(original, Expression.Constant(null)),
          Expression.Block(arrayTransferExpressions)
        )
      );

      return clone;
    }

    /// <summary>Generates state transfer expressions to copy a complex type</summary>
    /// <param name="clonedType">Complex type that will be cloned</param>
    /// <param name="original">Variable expression for the original instance</param>
    /// <param name="clone">Variable expression for the cloned instance</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    private static void generateComplexTypeTransferExpressions(
      Type clonedType, // Actual, concrete type (not declared type)
      Expression original, // Expected to be an object
      Expression clone,	// As actual, concrete type
      IList<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      // To access the fields of the original type, we need it to be of the actual
      // type instead of an object, so perform a downcast
      ParameterExpression typedOriginal = Expression.Variable(clonedType);
      variables.Add(typedOriginal);
      transferExpressions.Add(
        Expression.Assign(typedOriginal, Expression.Convert(original, clonedType))
      );

      // Now enumerate all of the type's fields and generate transfer expressions for
      // each of them
      FieldInfo[] fieldInfos = clonedType.GetFields(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        Type fieldType = fieldInfo.FieldType;

        if(fieldType.IsPrimitive || (fieldType == typeof(string))) {
          // Primitive types and strings can be transferred by simple assignment
          transferExpressions.Add(
            Expression.Assign(
              Expression.Field(clone, fieldInfo),
              Expression.Field(typedOriginal, fieldInfo)
            )
          );
        } else if(fieldType.IsValueType) {
          generateComplexTypeTransferExpressions(
            fieldType,
            Expression.Field(typedOriginal, fieldInfo),
            Expression.Field(clone, fieldInfo),
            variables,
            transferExpressions
          );
        } else {
          var fieldTransferExpressions = new List<Expression>();
          var fieldVariables = new List<ParameterExpression>();

          Expression fieldClone;
          if(fieldType.IsArray) {
            Type elementType = fieldType.GetElementType();
            if(elementType.IsPrimitive || (elementType == typeof(string))) {
              fieldClone = generatePrimitiveArrayTransferExpressions(
                fieldType,
                Expression.Field(typedOriginal, fieldInfo),
                fieldVariables,
                fieldTransferExpressions
              );
            } else {
              fieldClone = generateComplexArrayTransferExpressions(
                fieldType,
                Expression.Field(typedOriginal, fieldInfo),
                fieldVariables,
                fieldTransferExpressions
              );
            }

            fieldTransferExpressions.Add(
              Expression.Assign(Expression.Field(clone, fieldInfo), fieldClone)
            );
          } else {
            MethodInfo getOrCreateClonerMethodInfo = typeof(ExpressionTreeCloner).GetMethod(
              "getOrCreateDeepFieldBasedCloner",
              BindingFlags.NonPublic | BindingFlags.Static
            );
            MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
            MethodInfo invokeMethodInfo = typeof(Func<object, object>).GetMethod("Invoke");

            fieldTransferExpressions.Add(
              Expression.Assign(
                Expression.Field(clone, fieldInfo),
                Expression.Convert(
                  Expression.Call(
                    Expression.Call(
                      getOrCreateClonerMethodInfo,
                      Expression.Call(
                        Expression.Field(typedOriginal, fieldInfo), getTypeMethodInfo
                      )
                    ),
                    invokeMethodInfo,
                    Expression.Field(typedOriginal, fieldInfo)
                  ),
                  fieldType
                )
              )
            );
          }

          transferExpressions.Add(
            Expression.IfThen(
              Expression.NotEqual(
                Expression.Field(typedOriginal, fieldInfo), Expression.Constant(null)
              ),
              Expression.Block(fieldVariables, fieldTransferExpressions)
            )
          );

        }
      }
    }

    /// <summary>Compiles a method that creates a clone of an object</summary>
    /// <param name="clonedType">Type for which a clone method will be created</param>
    /// <returns>A method that clones an object of the provided type</returns>
    private static Func<object, object> createDeepFieldBasedCloner(Type clonedType) {
      ParameterExpression original = Expression.Parameter(typeof(object), "original");

      var transferExpressions = new List<Expression>();
      var variables = new List<ParameterExpression>();

      if(clonedType.IsPrimitive || (clonedType == typeof(string))) {
        // Primitives and strings are copied on direct assignment
        transferExpressions.Add(original);
      } else if(clonedType.IsArray) {
        // Arrays need to be cloned element-by-element
        Type elementType = clonedType.GetElementType();

        if(elementType.IsPrimitive || (elementType == typeof(string))) {
          // For primitive arrays, the Array.Clone() method is sufficient
          transferExpressions.Add(
            generatePrimitiveArrayTransferExpressions(
              clonedType, original, variables, transferExpressions
            )
          );
        } else {
          // Arrays of complex types require manual cloning
          transferExpressions.Add(
            generateComplexArrayTransferExpressions(
              clonedType, original, variables, transferExpressions
            )
          );
        }
      } else {
        // We need a variable to hold the clone because due to the assignments it
        // won't be last in the block when we're finished
        ParameterExpression clone = Expression.Variable(clonedType);
        variables.Add(clone);

        // Give it a new instance of the type being cloned
        transferExpressions.Add(Expression.Assign(clone, Expression.New(clonedType)));

        // Generate the expressions required to transfer the type field by field
        generateComplexTypeTransferExpressions(
          clonedType, original, clone, variables, transferExpressions
        );

        // Make sure the clone is the last thing in the block to set the return value
        transferExpressions.Add(clone);
      }

      // Turn all transfer expressions into a single block if necessary
      Expression resultExpression;
      if((transferExpressions.Count == 1) && (variables.Count == 0)) {
        resultExpression = transferExpressions[0];
      } else {
        resultExpression = Expression.Block(variables, transferExpressions);
      }

      // Value types require manual boxing
      if(clonedType.IsValueType) {
        resultExpression = Expression.Convert(resultExpression, typeof(object));
      }

      Expression<Func<object, object>> expression = Expression.Lambda<Func<object, object>>(
        resultExpression, original
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
    private static ConcurrentDictionary<Type, Func<object, object>> shallowCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> deepCloners;

  }

} // namespace Nuclex.Support.Cloning

#endif // !(XBOX360 || WINDOWS_PHONE)
