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
using System.Globalization;

using Microsoft.Win32;

using Nuclex.Support.Parsing;

namespace Nuclex.Support.Settings {

  /// <summary>Stores settings in the registry on Windows operating systems</summary>
  public class WindowsRegistryStore : ISettingsStore, IDisposable {

    /// <summary>Initializes a new settings store on the specified registry path</summary>
    /// <param name="hive">Hive in which to look</param>
    /// <param name="directory">Base path of the settings in the specified hive</param>
    /// <param name="writable">Whether to open the registry in writable mode</param>
    public WindowsRegistryStore(RegistryHive hive, string directory, bool writable = true) {
      using(RegistryKey hiveKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default)) {
        this.rootKey = hiveKey.OpenSubKey(directory, writable);
      }
      this.writable = writable;
    }

    /// <summary>Initializes a new settings store on the specified registry key</summary>
    /// <param name="rootKey">Registry key the settings are stored under</param>
    /// <param name="writable">Whether the registry was opened in writable mode</param>
    /// <remarks>
    ///   This constructor takes ownership of the registry key. It will be disposed when
    ///   the settings store is disposed.
    /// </remarks>
    public WindowsRegistryStore(RegistryKey rootKey, bool writable = true) {
      this.rootKey = rootKey;
      this.writable = writable;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.rootKey != null) {
        this.rootKey.Dispose();
        this.rootKey = null;
      }
    }

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
    public IEnumerable<string> EnumerateCategories() {
      return this.rootKey.GetSubKeyNames();
    }

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    public IEnumerable<OptionInfo> EnumerateOptions(string category = null) {
      if(string.IsNullOrEmpty(category)) {
        string[] valueNames = this.rootKey.GetValueNames();
        for(int index = 0; index < valueNames.Length; ++index) {
          yield return new OptionInfo() {
            Name = valueNames[index],
            OptionType = getBestMatchingType(this.rootKey, valueNames[index])
          };
        }
      } else {
        using(RegistryKey categoryKey = this.rootKey.OpenSubKey(category, this.writable)) {
          string[] valueNames = categoryKey.GetValueNames();
          for(int index = 0; index < valueNames.Length; ++index) {
            yield return new OptionInfo() {
              Name = valueNames[index],
              OptionType = getBestMatchingType(categoryKey, valueNames[index])
            };
          }
        }
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
      if(string.IsNullOrEmpty(category)) {
        return tryGetValueFromKey(this.rootKey, optionName, out value);
      } else {
				RegistryKey categoryKey = this.rootKey.OpenSubKey(category, this.writable);
				if(categoryKey == null) {
					value = default(TValue);
					return false;
				}
        using(categoryKey) {
          return tryGetValueFromKey(categoryKey, optionName, out value);
        }
      }
    }

    /// <summary>Saves an option in the settings store</summary>
    /// <typeparam name="TValue">Type of value that will be saved</typeparam>
    /// <param name="category">Category the option will be placed in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be saved</param>
    /// <param name="value">The value under which the option will be saved</param>
    public void Set<TValue>(string category, string optionName, TValue value) {
      if(string.IsNullOrEmpty(category)) {
        setValue(this.rootKey, optionName, value);
      } else {
				RegistryKey categoryKey = this.rootKey.OpenSubKey(category, this.writable);
				if(categoryKey == null) {
					categoryKey = this.rootKey.CreateSubKey(category);
				}
        using(categoryKey) {
          setValue(categoryKey, optionName, value);
        }
      }
    }

    /// <summary>Writes a setting to the registry</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="registryKey"></param>
    /// <param name="optionName"></param>
    /// <param name="value"></param>
    private void setValue<TValue>(RegistryKey registryKey, string optionName, TValue value) {
      if(typeof(TValue) == typeof(int)) {
        registryKey.SetValue(optionName, value, RegistryValueKind.DWord);
      } else if(typeof(TValue) == typeof(long)) {
        registryKey.SetValue(optionName, value, RegistryValueKind.QWord);
      } else if(typeof(TValue) == typeof(bool)) {
        registryKey.SetValue(optionName, value, RegistryValueKind.DWord);
      } else if(typeof(TValue) == typeof(string)) {
        registryKey.SetValue(optionName, value, RegistryValueKind.String);
      } else if(typeof(TValue) == typeof(string[])) {
        registryKey.SetValue(optionName, value, RegistryValueKind.MultiString);
      } else if(typeof(TValue) == typeof(byte[])) {
        registryKey.SetValue(optionName, value, RegistryValueKind.Binary);
      } else {
        string valueAsString = (string)Convert.ChangeType(
          value, typeof(string), CultureInfo.InvariantCulture
        );
        registryKey.SetValue(optionName, valueAsString, RegistryValueKind.String);
      }
    }

    /// <summary>Removes the option with the specified name</summary>
    /// <param name="category">Category the option is found in. Can be null.</param>
    /// <param name="optionName">Name of the option that will be removed</param>
    /// <returns>True if the option was found and removed</returns>
    public bool Remove(string category, string optionName) {
      throw new NotImplementedException();
    }

    /// <summary>Tries to retrieve the value of a registry key if it exists</summary>
    /// <typeparam name="TValue">Type of value the registry key is expected to have</typeparam>
    /// <param name="categoryKey">Registry key the value is stored under</param>
    /// <param name="optionName">Name of the option in the registry</param>
    /// <param name="value">Will receive the value read from the registry</param>
    /// <returns>True if the value was found, false otherwise</returns>
    private bool tryGetValueFromKey<TValue>(
      RegistryKey categoryKey, string optionName, out TValue value
    ) {
      object valueAsObject = categoryKey.GetValue(optionName);
      if(valueAsObject == null) {
        value = default(TValue);
        return false;
      } else {
        value = (TValue)Convert.ChangeType(
          valueAsObject, typeof(TValue), CultureInfo.InvariantCulture
        );
        return true;
      }
    }

    /// <summary>Figures out which .NET type best matches the registry value</summary>
    /// <param name="categoryKey">Registry key the key is stored in</param>
    /// <param name="optionName">Name of the option that will be retrieved</param>
    /// <returns>The best matching .NET type for the registry key's value</returns>
    private static Type getBestMatchingType(RegistryKey categoryKey, string optionName) {
      RegistryValueKind valueKind = categoryKey.GetValueKind(optionName);
      switch(valueKind) {
        case RegistryValueKind.Binary: { return typeof(byte[]); }
        case RegistryValueKind.DWord: { return typeof(int); }
        case RegistryValueKind.QWord: { return typeof(long); }
        case RegistryValueKind.MultiString: { return typeof(string[]); }
        case RegistryValueKind.ExpandString:
        case RegistryValueKind.String: {
          string value = (string)categoryKey.GetValue(optionName);
          if(value.Length == 0) {
            return typeof(string);
          }

          // If there are at least two characters, it may be an integer with
          // a sign in front of it
          if(value.Length >= 2) {
            int index = 0;
            if(ParserHelper.SkipInteger(value, ref index)) {
              if(index >= value.Length) {
                return typeof(int);
              }
              if(value[index] == '.') {
                return typeof(float);
              }
            }
          } else { // If it's just a single character, it may be a number
            if(char.IsNumber(value, 0)) {
              return typeof(int);
            }
          }

          return typeof(string);
        }

        case RegistryValueKind.Unknown:
        case RegistryValueKind.None:
        default: { return typeof(string); }
      }
    }

    /// <summary>Key on which the registry store is operating</summary>
    private RegistryKey rootKey;
    /// <summary>Whether the user can write to the registry key</summary>
    private bool writable;

  }

} // namespace Nuclex.Support.Settings

#endif // WINDOWS
