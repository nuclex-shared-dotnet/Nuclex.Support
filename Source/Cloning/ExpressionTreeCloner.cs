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

namespace Nuclex.Support.Cloning {

  /// <summary>
  ///   Cloning factory which uses expression trees to improve performance when cloning
  ///   is a high-frequency action.
  /// </summary>
  public partial class ExpressionTreeCloner : ICloneFactory {

    /// <summary>Initializes the static members of the expression tree cloner</summary>
    static ExpressionTreeCloner() {
      shallowFieldBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
      deepFieldBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
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
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      if(usePropertyBasedClone) {
        throw new NotImplementedException("Not implemented yet");
      } else {
        Func<object, object> cloner = getOrCreateShallowFieldBasedCloner(typeof(TCloned));
        return (TCloned)cloner(objectToCloneAsObject);
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

#endif

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateShallowFieldBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!shallowFieldBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createShallowFieldBasedCloner(clonedType);
        shallowFieldBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateDeepFieldBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!deepFieldBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createDeepFieldBasedCloner(clonedType);
        deepFieldBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>Compiled cloners that perform shallow clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> shallowFieldBasedCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> deepFieldBasedCloners;
    /// <summary>Compiled cloners that perform shallow clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> shallowPropertyBasedCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> deepPropertyBasedCloners;

  }

} // namespace Nuclex.Support.Cloning

#endif // !(XBOX360 || WINDOWS_PHONE)
