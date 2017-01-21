#region CPL License
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

  /// <summary>Stores settings in memory</summary>
  public class MemoryStore : ISettingsStore {

    /// <summary>Initializes a new settings store managing settings in memory</summary>
    public MemoryStore() {
      this.options = new Dictionary<string, IDictionary<string, object>>();
      this.rootOptions = new Dictionary<string, object>();
    }

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    public IEnumerable<string> EnumerateCategories() {
      return this.options.Keys;
    }

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    public IEnumerable<OptionInfo> EnumerateOptions(string category = null) {
      IDictionary<string, object> categoryOptions;
      if(string.IsNullOrEmpty(category)) {
        categoryOptions = this.rootOptions;
      } else if(!this.options.TryGetValue(category, out categoryOptions)) {
        yield break;
      }

      foreach(KeyValuePair<string, object> option in categoryOptions) {
        OptionInfo optionInfo = new OptionInfo() {
          Name = option.Key,
          OptionType = option.Value.GetType()
        };
        yield return optionInfo;
      }
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
            "There is no option named '" + optionName + "' in the settings"
          );
        } else {
          throw new KeyNotFoundException(
            "There is no option named '" + optionName + "' under the category '" +
            category + "' in the settings"
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
      IDictionary<string, object> categoryOptions = getCategoryByName(category);
      if(categoryOptions != null) {
        object valueAsObject;
        if(categoryOptions.TryGetValue(optionName, out valueAsObject)) {
          value = (TValue)Convert.ChangeType(valueAsObject, typeof(TValue));
          return true;
        }
      }

      value = default(TValue);
      return false;
    }

    /// <summary>Saves an option in the settings store</summary>
    /// <typeparam name="TValue">Type of value that will be saved</typeparam>
    /// <param name="category">Category the option will be placed in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be saved</param>
    /// <param name="value">The value under which the option will be saved</param>
    public void Set<TValue>(string category, string optionName, TValue value) {
      IDictionary<string, object> targetCategory;

      if(string.IsNullOrEmpty(category)) {
        targetCategory = this.rootOptions;
      } else if(!this.options.TryGetValue(category, out targetCategory)) {
        targetCategory = new Dictionary<string, object>();
        this.options.Add(category, targetCategory);
        targetCategory.Add(optionName, value);
        return;
      }

      targetCategory[optionName] = value;
    }

    /// <summary>Removes the option with the specified name</summary>
    /// <param name="category">Category the option is found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be removed</param>
    /// <returns>True if the option was found and removed</returns>
    public bool Remove(string category, string optionName) {
      IDictionary<string, object> targetCategory = getCategoryByName(category);
      if(targetCategory == null) {
        return false;
      }

      return targetCategory.Remove(optionName);
    }

    /// <summary>Looks up a category by its name</summary>
    /// <param name="name">Name of the category that will be looked up</param>
    /// <returns>The category with the specified name if found, null otherwise</returns>
    private IDictionary<string, object> getCategoryByName(string name) {
      IDictionary<string, object> category;

      if(string.IsNullOrEmpty(name)) {
        category = this.rootOptions;
      } else if(!this.options.TryGetValue(name, out category)) {
        return null;
      }

      return category;
    }

    /// <summary>Categories and the options stored in them</summary>
    private IDictionary<string, IDictionary<string, object>> options;
    /// <summary>Options stored at the root level</summary>
    private IDictionary<string, object> rootOptions;

  }

} // namespace Nuclex.Support.Settings
