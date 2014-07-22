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
using System.Globalization;
using System.IO;
using System.Text;

using Nuclex.Support.Parsing;

namespace Nuclex.Support.Settings {

  /// <summary>Represents an ini- or cfg-like configuration file</summary>
  /// <remarks>
  ///   This class tries its best to preserve the formatting of configuration files.
  ///   Changing a value will keep the line it appears in intact.
  /// </remarks>
  public partial class ConfigurationFileStore : ISettingsStore {

    #region class Category

    /// <summary>Stores informations about a category found in the configuration file</summary>
    private class Category {

      /// <summary>Name of the category as a string</summary>
      public StringSegment CategoryName;

      /// <summary>Lookup table for the options in this category</summary>
      public IDictionary<string, Option> OptionLookup;

      /// <summary>Lines this category and its options consist of</summary>
      public IList<string> Lines;

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
      this.options = new List<Option>();
      this.categoryLookup = new Dictionary<string, Category>();
      this.RootCategory = new Category() {
        OptionLookup = new Dictionary<string, Option>(),
        Lines = new List<string>()
      };
    }

    /// <summary>Saves the configuration file into the specified writer</summary>
    /// <param name="writer">Writer the configuration file will be saved into</param>
    public void Save(TextWriter writer) {
      for(int index = 0; index < this.RootCategory.Lines.Count; ++index) {
        writer.WriteLine(this.RootCategory.Lines[index]);
      }
      foreach(Category category in this.categoryLookup.Values) {
        for(int index = 0; index < category.Lines.Count; ++index) {
          writer.WriteLine(category.Lines[index]);
        }
      }
    }

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    public IEnumerable<string> EnumerateCategories() {
      return this.categoryLookup.Keys;
    }

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    public IEnumerable<OptionInfo> EnumerateOptions(string category = null) {
      Category enumeratedCategory = getCategoryByName(category);
      if(enumeratedCategory == null) {
        yield break;
      }

      foreach(Option option in this.RootCategory.OptionLookup.Values) {
        OptionInfo optionInfo = new OptionInfo() {
          Name = option.OptionName.ToString(),
          OptionType = getBestMatchingType(ref option.OptionValue)
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
      Category containingCategory = getCategoryByName(category);
      if(containingCategory != null) {
        Option option;
        if(containingCategory.OptionLookup.TryGetValue(optionName, out option)) {
          if(typeof(TValue) == typeof(bool)) {
            bool? boolean = ParserHelper.ParseBooleanLiteral(ref option.OptionValue);
            if(boolean.HasValue) {
              value = (TValue)(object)boolean.Value;
              return true;
            } else {
              throw new FormatException(
                "The value '" + option.OptionValue.ToString() + "' is not a boolean"
              );
            }
          } else {
            value = (TValue)Convert.ChangeType(option.OptionValue.ToString(), typeof(TValue));
            return true;
          }
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
      string valueAsString = (string)Convert.ChangeType(
        value, typeof(string), CultureInfo.InvariantCulture
      );

      Category targetCategory;
      if(string.IsNullOrEmpty(category)) {
        targetCategory = this.RootCategory;
      } else if(!this.categoryLookup.TryGetValue(category, out targetCategory)) {
        targetCategory = createCategory(category);
        createOption(targetCategory, optionName, valueAsString);
        return;
      }

      Option targetOption;
      if(targetCategory.OptionLookup.TryGetValue(optionName, out targetOption)) {
        changeOption(targetCategory, targetOption, valueAsString);
      } else {
        createOption(targetCategory, optionName, valueAsString);
      }
    }

    /// <summary>Removes the option with the specified name</summary>
    /// <param name="category">Category the option is found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be removed</param>
    /// <returns>True if the option was found and removed</returns>
    public bool Remove(string category, string optionName) {
      Category sourceCategory = getCategoryByName(category);
      if(sourceCategory == null) {
        return false;
      }

      Option option;
      if(!sourceCategory.OptionLookup.TryGetValue(optionName, out option)) {
        return false;
      }

      sourceCategory.Lines.RemoveAt(option.LineIndex);
      sourceCategory.OptionLookup.Remove(optionName);

      foreach(Option shiftedOption in sourceCategory.OptionLookup.Values) {
        if(shiftedOption.LineIndex > option.LineIndex) {
          --shiftedOption.LineIndex;
        }
      }

      return true;
    }

    /// <summary>Looks a category up by its name</summary>
    /// <param name="categoryName">
    ///   Name of the category. Can be null for the root category
    /// </param>
    /// <returns>The category with the specified name</returns>
    private Category getCategoryByName(string categoryName) {
      Category category;

      if(string.IsNullOrEmpty(categoryName)) {
        category = this.RootCategory;
      } else if(!this.categoryLookup.TryGetValue(categoryName, out category)) {
        return null;
      }

      return category;
    }

    /// <summary>Creates a new option</summary>
    /// <param name="category">Category the option will be added to</param>
    /// <param name="name">Name of the option</param>
    /// <param name="value">Value that will be assigned to the option</param>
    private void createOption(Category category, string name, string value) {
      int valueLength;
      if(value == null) {
        valueLength = 0;
      } else {
        valueLength = value.Length;
      }

      // Build the complete line containing the option assignment
      string line;
      {
        StringBuilder builder = new StringBuilder(name.Length + 3 + valueLength);

        builder.Append(name);
        builder.Append(" = ");
        if(valueLength > 0) {
          builder.Append(value);
        }

        line = builder.ToString();
      }

      Option newOption = new Option() {
        OptionName = new StringSegment(line, 0, name.Length),
        OptionValue = new StringSegment(line, name.Length + 3, valueLength)
      };

      // Figure out which line the new option should be put in
      int lastLineIndex = category.Lines.Count - 1;
      if((lastLineIndex > 0) && (category.Lines[lastLineIndex].Length == 0)) {
        newOption.LineIndex = lastLineIndex;
        category.Lines.Insert(lastLineIndex, line);
      } else {
        newOption.LineIndex = category.Lines.Count;
        category.Lines.Add(line);
        category.Lines.Add(string.Empty);
      }

      category.OptionLookup.Add(name, newOption);
    }

    /// <summary>Changes the value of an option</summary>
    /// <param name="category">Category that holds the option</param>
    /// <param name="option">Option whose value will be changed</param>
    /// <param name="newValue">New value that will be assigned to the option</param>
    private void changeOption(Category category, Option option, string newValue) {
      int newValueLength;
      if(newValue == null) {
        newValueLength = 0;
      } else {
        newValueLength = newValue.Length;
      }

      // Form the new line
      string line = option.OptionValue.Text;
      {
        StringBuilder builder = new StringBuilder(
          line.Length - option.OptionValue.Count + newValueLength
        );

        // Stuff before the value
        if(option.OptionValue.Offset > 0) {
          builder.Append(line, 0, option.OptionValue.Offset);
        }

        // The value itself
        if(newValueLength > 0) {
          builder.Append(newValue);
        }

        // Stuff after the value
        int endIndex = option.OptionValue.Offset + option.OptionValue.Count;
        if(endIndex < line.Length) {
          builder.Append(line, endIndex, line.Length - endIndex);
        }

        line = builder.ToString();
      }

      option.OptionValue = new StringSegment(line, option.OptionValue.Offset, newValueLength);
      category.Lines[option.LineIndex] = line;
    }

    /// <summary>Creates a new category in the configuration file</summary>
    /// <param name="category">Name of the new category</param>
    /// <returns>The category that was created</returns>
    private Category createCategory(string category) {
      string categoryDefinition;
      {
        StringBuilder builder = new StringBuilder(category.Length + 2);
        builder.Append('[');
        builder.Append(category);
        builder.Append(']');
        categoryDefinition = builder.ToString();
      }

      Category newCategory = new Category() {
        CategoryName = new StringSegment(categoryDefinition, 1, category.Length),
        OptionLookup = new Dictionary<string, Option>(),
        Lines = new List<string>()
      };

      newCategory.Lines.Add(categoryDefinition);
      newCategory.Lines.Add(string.Empty);

      this.categoryLookup.Add(category, newCategory);

      return newCategory;
    }

    /// <summary>Records where options are stored in the configuration file</summary>
    private IList<Option> options;

    /// <summary>Root category where options above any category definition go</summary>
    private Category RootCategory;
    /// <summary>Lookup table for all categories by their name</summary>
    private IDictionary<string, Category> categoryLookup;

  }

} // namespace Nuclex.Support.Settings
