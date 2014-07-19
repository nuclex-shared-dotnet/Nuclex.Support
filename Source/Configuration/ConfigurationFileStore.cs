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

using System;
using System.Collections.Generic;
using System.IO;

using Nuclex.Support.Parsing;

namespace Nuclex.Support.Configuration {

  /// <summary>Represents an ini- or cfg-like configuration file</summary>
  /// <remarks>
  ///   This class tries its best to preserve the formatting of configuration files.
  ///   Changing a value will keep the line it appears in intact.
  /// </remarks>
  public partial class ConfigurationFileStore : ISettingsStore {

    #region class Category

    /// <summary>Stores informations about a category found in the configuration file</summary>
    private class Category {

      /// <summary>Index of the line the category is defined in</summary>
      public int LineIndex;

      /// <summary>Name of the category as a string</summary>
      public StringSegment CategoryName;

    }

    #endregion // class Category

    #region class Option

    /// <summary>Stores informations about an option found in the configuration file</summary>
    private class Option {

      /// <summary>Index of the line the option is defined in</summary>
      public int LineIndex;

      /// <summary>Name of the option as a string</summary>
      public StringSegment OptionName;

      /// <summary>Value of the option as a string</summary>
      public StringSegment OptionValue;

    }

    #endregion // class Option

    /// <summary>Initializes a new, empty configuration file</summary>
    public ConfigurationFileStore() {
      this.lines = new List<string>();
      this.categories = new List<Category>();
      this.options = new List<Option>();
    }

    /// <summary>Saves the configuration file into the specified writer</summary>
    /// <param name="writer">Writer the configuration file will be saved into</param>
    public void Save(TextWriter writer) {
      for(int index = 0; index < this.lines.Count; ++index) {
        writer.WriteLine(this.lines[index]);
      }
    }

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    public IEnumerable<string> EnumerateCategories() {
      for(int index = 0; index < this.categories.Count; ++index) {
        yield return this.categories[index].CategoryName.ToString();
      }
    }

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    public IEnumerable<OptionInfo> EnumerateOptions(string category = null) {
      for(int index = 0; index < this.options.Count; ++index) {
        OptionInfo optionInfo = new OptionInfo() {
          Name = this.options[index].OptionName.ToString(),
          OptionType = getBestMatchingType(ref this.options[index].OptionValue)
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
            "There is no option named '" + optionName + "' in the configuration file"
          );
        } else {
          throw new KeyNotFoundException(
            "There is no option named '" + optionName + "' under the category '" +
            category + "' in the configuration file"
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

    /// <summary>Lines contained in the configuration file</summary>
    private IList<string> lines;
    /// <summary>Records where categories are stored in the configuration file</summary>
    private IList<Category> categories;
    /// <summary>Records where options are stored in the configuration file</summary>
    private IList<Option> options;

  }

} // namespace Nuclex.Support.Configuration
