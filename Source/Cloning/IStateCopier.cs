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

namespace Nuclex.Support.Cloning {

  /// <summary>Copies the state of objects</summary>
  public interface IStateCopier {

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void DeepCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class;

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void DeepCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct;

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void ShallowCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class;

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void ShallowCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct;

  }

} // namespace Nuclex.Support.Cloning
