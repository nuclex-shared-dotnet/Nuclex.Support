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

  /// <summary>Constructs new objects by cloning existing objects</summary>
  public interface ICloneFactory {

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    /// <remarks>
    ///   Field-based clones are guaranteed to be complete - there will be no missed
    ///   members. This type of clone is also able to clone types that do not provide
    ///   a default constructor.
    /// </remarks>
    TCloned ShallowFieldClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    /// <remarks>
    ///   <para>
    ///     A property-based clone is useful if you're using dynamically generated proxies,
    ///     such as when working with entities returned by an ORM like NHibernate.
    ///     When not using a property-based clone, internal proxy fields would be cloned
    ///     and might cause problems with the ORM.
    ///   </para>
    ///   <para>
    ///     Property-based clones require a default constructor because there's no guarantee
    ///     that all fields will are assignable through properties and starting with
    ///     an uninitialized object is likely to end up with a broken clone.
    ///   </para>
    /// </remarks>
    TCloned ShallowPropertyClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    /// <remarks>
    ///   Field-based clones are guaranteed to be complete - there will be no missed
    ///   members. This type of clone is also able to clone types that do not provide
    ///   a default constructor.
    /// </remarks>
    TCloned DeepFieldClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    /// <remarks>
    ///   <para>
    ///     A property-based clone is useful if you're using dynamically generated proxies,
    ///     such as when working with entities returned by an ORM like NHibernate.
    ///     When not using a property-based clone, internal proxy fields would be cloned
    ///     and might cause problems with the ORM.
    ///   </para>
    ///   <para>
    ///     Property-based clones require a default constructor because there's no guarantee
    ///     that all fields will are assignable through properties and starting with
    ///     an uninitialized object is likely to end up with a broken clone.
    ///   </para>
    /// </remarks>
    TCloned DeepPropertyClone<TCloned>(TCloned objectToClone);

  }

} // namespace Nuclex.Support.Cloning
