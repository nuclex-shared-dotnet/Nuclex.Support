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

namespace Nuclex.Support.Cloning {

  /// <summary>Constructs new objects by cloning existing objects</summary>
  public interface ICloneFactory {

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
    /// <remarks>
    ///   A property-based clone is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based clone, internal proxy fields would be cloned
    ///   and might cause problems with the ORM.
    /// </remarks>
    TCloned DeepClone<TCloned>(TCloned objectToClone, bool usePropertyBasedClone);

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <param name="usePropertyBasedClone">
    ///   Whether to clone the object based on its properties only
    /// </param>
    /// <returns>A shallow clone of the provided object</returns>
    /// <remarks>
    ///   A property-based clone is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based clone, internal proxy fields would be cloned
    ///   and might cause problems with the ORM.
    /// </remarks>
    TCloned ShallowClone<TCloned>(TCloned objectToClone, bool usePropertyBasedClone);

  }

} // namespace Nuclex.Support.Cloning
