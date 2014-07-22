#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2014 Nuclex Development Labs

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

#if WINDOWS

using System;
using System.Collections.Generic;

namespace Nuclex.Support.Settings {

  /// <summary>Stores settings in the registry on Windows operating systems</summary>
  public class WindowsRegistryStore : ISettingsStore, IDisposable {

    

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
    }

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    public IEnumerable<string> EnumerateCategories() {
      throw new NotImplementedException();
    }

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    public IEnumerable<OptionInfo> EnumerateOptions(string category = null) {
      throw new NotImplementedException();
    }

    /// <summary>Retrieves the value of the specified option</summary>
    /// <typeparam name="TValue">Type the option will be converted to</typeparam>
    /// <param name="category">Category the option can be found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be looked up</param>
    /// <returns>The value of the option with the specified name</returns>
    public TValue Get<TValue>(string category, string optionName) {
      TValue value;
      if(TryGet<TValue>(category, optionName, out value)) {
        return value;
      } else {
        if(string.IsNullOrEmpty(category)) {
          throw new KeyNotFoundException(
            "There is no option named '" + optionName + "' in the registry"
          );
        } else {
          throw new KeyNotFoundException(
            "There is no option named '" + optionName + "' under the category '" +
            category + "' in the registry"
          );
        }
      }
    }

    /// <summary>Tries to retrieve the value of the specified option</summary>
    /// <typeparam name="TValue">Type the option will be converted to</typeparam>
    /// <param name="category">Category the option can be found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be looked up</param>
    /// <param name="value">Will receive the value of the option, if found</param>
    /// <returns>
    ///   True if the option existed and its value was written into the <paramref name="value" />
    ///   parameter, false otherwise
    /// </returns>
    public bool TryGet<TValue>(string category, string optionName, out TValue value) {
      throw new NotImplementedException();
    }

    /// <summary>Saves an option in the settings store</summary>
    /// <typeparam name="TValue">Type of value that will be saved</typeparam>
    /// <param name="category">Category the option will be placed in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be saved</param>
    /// <param name="value">The value under which the option will be saved</param>
    public void Set<TValue>(string category, string optionName, TValue value) {
      throw new NotImplementedException();
    }

    /// <summary>Removes the option with the specified name</summary>
    /// <param name="category">Category the option is found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be removed</param>
    /// <returns>True if the option was found and removed</returns>
    public bool Remove(string category, string optionName) {
      throw new NotImplementedException();
    }

  }

} // namespace Nuclex.Support.Settings

#endif // WINDOWS
