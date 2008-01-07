#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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

namespace Nuclex.Support.Plugins {

  /// <summary>Abstract factory</summary>
  /// <typeparam name="T">Interface or base class of the product of the factory</typeparam>
  public interface IFactory<T> {

    /// <summary>The concrete type as implemented by the factory instance</summary>
    Type ConcreteType { get; }

    /// <summary>Creates a new instance of the type to which the factory is specialized</summary>
    /// <returns>The newly created instance</returns>
    T CreateInstance();

  }

  /// <summary>Employer to create factories of suiting types found in plugins</summary>
  /// <typeparam name="T">Interface or base class that the types need to implement</typeparam>
  /// <remarks>
  ///   <para>
  ///     This employer will not directly instanciate any compatible types found in
  ///     plugin assemblies, but generated runtime-factories of these types, enabling the
  ///     user to decide when and how many instances of a type will be created.
  ///   </para>
  ///   <para>
  ///     This approach has the advantage that it enables even assemblies that were not
  ///     intended to be plugins can be loaded as plugins, without risking an instanciation
  ///     or complex and possibly heavy-weight types. The disadvantage is that the
  ///     runtime-factory can not provide decent informationa about the plugin type like
  ///     a human-readable name, capabilities or an icon.
  ///   </para>
  /// </remarks>
  public class FactoryEmployer<T> : Employer {

    #region class Factory

    /// <summary>Concrete factory for the types in a plugin assembly</summary>
    private class Factory : IFactory<T> {

      /// <summary>Initializes a factory and configures it for the specified product</summary>
      /// <param name="type">Type of which the factory creates instances</param>
      public Factory(Type type) {
        this.concreteType = type;
      }

      /// <summary>The concrete type as produced by the factory</summary>
      public Type ConcreteType {
        get { return this.concreteType; }
      }

      /// <summary>Create a new instance of the type that the factory is configured to</summary>
      /// <returns>The newly created instance</returns>
      public T CreateInstance() {
        return (T)Activator.CreateInstance(this.concreteType);
      }

      /// <summary>Concrete product which the factory instance creates</summary>
      private Type concreteType;

    }

    #endregion // class Factory

    /// <summary>Initializes a new FactoryEmployer</summary>
    public FactoryEmployer() {
      this.employedFactories = new List<IFactory<T>>();
    }

    /// <summary>List of all factories that the instance employer has created</summary>
    public List<IFactory<T>> Factories {
      get { return this.employedFactories; }
    }

    /// <summary>Determines whether the type suites the employer's requirements</summary>
    /// <param name="type">Type which will be assessed</param>
    /// <returns>True if the type can be employed</returns>
    public override bool CanEmploy(Type type) {
      return
        Support.HasDefaultConstructor(type) &&
        typeof(T).IsAssignableFrom(type);
    }

    /// <summary>Employs the specified plugin type</summary>
    /// <param name="type">Type to be employed</param>
    public override void Employ(Type type) {
      this.employedFactories.Add(new Factory(type));
    }

    /// <summary>All factories that the instance employer has created</summary>
    private List<IFactory<T>> employedFactories;

  }

} // namespace Nuclex.Support.Plugins
