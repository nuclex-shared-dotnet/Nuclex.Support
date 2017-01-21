﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2017 Nuclex Development Labs

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

namespace Nuclex.Support.Settings {

  /// <summary>Interface by which settings and configuration data can be accessed</summary>
  /// <remarks>
  ///   <para>
  ///     The intended usage pattern for options is for your application to simply read and
  ///     write whatever options it needs using the type it expects them to be.
  ///   </para>
  ///   <para>
  ///     When you enumerate the options appearing under a category, the settings store will
  ///     try to guess the likely type of an option, but this is not always accurate. For
  ///     example, assigning the text 'true' to an option in a .cfg or .ini file could mean
  ///     that the option is a boolean or it could simply be a coincidence. When you read
  ///     this value as a boolean, the settings store will correctly convert it to a boolean,
  ///     when you read it as a string, you will get back "true" in plain text.
  ///   </para>
  ///   <para>
  ///     Which types of values a settings store can accept can also change between different
  ///     settings store implementations. The windows registry supports string and byte
  ///     arrays whereas configuration files have no standardized way of holding these.
  ///     Any property store must support a minimal subset including booleans, integers,
  ///     floating point values and strings.
  ///   </para>
  /// </remarks>
  public interface ISettingsStore {

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    IEnumerable<string> EnumerateCategories();

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    IEnumerable<OptionInfo> EnumerateOptions(string category = null);

    /// <summary>Retrieves the value of the specified option</summary>
    /// <typeparam name="TValue">Type the option will be converted to</typeparam>
    /// <param name="category">Category the option can be found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be looked up</param>
    /// <returns>The value of the option with the specified name</returns>
    TValue Get<TValue>(string category, string optionName);

    /// <summary>Tries to retrieve the value of the specified option</summary>
    /// <typeparam name="TValue">Type the option will be converted to</typeparam>
    /// <param name="category">Category the option can be found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be looked up</param>
    /// <param name="value">Will receive the value of the option, if found</param>
    /// <returns>
    ///   True if the option existed and its value was written into the <paramref name="value" />
    ///   parameter, false otherwise
    /// </returns>
    bool TryGet<TValue>(string category, string optionName, out TValue value);

    /// <summary>Saves an option in the settings store</summary>
    /// <typeparam name="TValue">Type of value that will be saved</typeparam>
    /// <param name="category">Category the option will be placed in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be saved</param>
    /// <param name="value">The value under which the option will be saved</param>
    void Set<TValue>(string category, string optionName, TValue value);

    /// <summary>Removes the option with the specified name</summary>
    /// <param name="category">Category the option is found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be removed</param>
    /// <returns>True if the option was found and removed</returns>
    bool Remove(string category, string optionName);

  }

} // namespace Nuclex.Support.Settings

#if WANT_TO_SUPPORT_MESSED_UP_CONFIGURATION_FILES
    /// <remarks>
    ///   Some settings stores allow multiple options with the same name to exist.
    ///   If you request a collection of values (IEnumerable, ICollection, IList or their
    ///   generic variants), you will be given a collection of all values appearing
    ///   in the scope you specified.
    /// </remarks>
#endif