﻿#region CPL License
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
using System.IO;

using Nuclex.Support.Parsing;

namespace Nuclex.Support.Configuration {

  partial class ConfigurationFileStore {

    /// <summary>Parses a configuration file from the specified text reader</summary>
    /// <param name="reader">Reader the configuration file will be parsed from</param>
    /// <returns>The configuration file parsed from the specified reader</returns>
    public static ConfigurationFileStore Parse(TextReader reader) {
      var store = new ConfigurationFileStore();

      for(; ; ) {
        string line = reader.ReadLine();
        if(line == null) {
          return store;
        }


      }
    }

    /// <summary>Determines the best matching type for an option value</summary>
    /// <param name="value">Value for which the best matching type will be found</param>
    /// <returns>The best matching type for the specified option value</returns>
    private static Type getBestMatchingType(ref StringSegment value) {
      if(value.Count == 0) {
        return typeof(string);
      }

      // If there are at least two characters, it may be an integer with
      // a sign in front of it
      if(value.Count >= 2) {
        int index = value.Offset;
        if(ParserHelper.SkipInteger(value.Text, ref index)) {
          if(index < value.Offset + value.Count) {
            if(value.Text[index] == '.') {
              return typeof(float);
            }
          }

          return typeof(int);
        }
      } else if(value.Count >= 1) { // If it's at least one character, it may be a number
        int index = value.Offset;
        ParserHelper.SkipNumericals(value.Text, ref index);
        if(index > value.Offset) {
          return typeof(int);
        }
      }

      // If it parses as a boolean literal, then it must be a boolean
      if(parseBooleanLiteral(ref value) != null) {
        return typeof(bool);
      }

      return typeof(string);
    }

    /// <summary>Tried to parse a boolean literal</summary>
    /// <param name="value">Value that will be parsed as a boolean literal</param>
    /// <returns>
    ///   True or false if the value was a boolean literal, null if it wasn't
    /// </returns>
    private static bool? parseBooleanLiteral(ref StringSegment value) {
      switch(value.Count) {

        // If the string spells 'no', it is considered a boolean
        case 2: {
          bool isSpellingNo =
            ((value.Text[value.Offset + 0] == 'n') || (value.Text[value.Offset + 0] == 'N')) &&
            ((value.Text[value.Offset + 1] == 'o') || (value.Text[value.Offset + 1] == 'O'));
          return isSpellingNo ? new Nullable<bool>(false) : null;
        }

        // If the string spells 'yes', it is considered a boolean
        case 3: {
          bool isSpellingYes =
            ((value.Text[value.Offset + 0] == 'y') || (value.Text[value.Offset + 0] == 'Y')) &&
            ((value.Text[value.Offset + 1] == 'e') || (value.Text[value.Offset + 1] == 'E')) &&
            ((value.Text[value.Offset + 2] == 's') || (value.Text[value.Offset + 2] == 'S'));
          return isSpellingYes ? new Nullable<bool>(true) : null;
        }

        // If the string spells 'true', it is considered a boolean
        case 4: {
          bool isSpellingTrue =
            ((value.Text[value.Offset + 0] == 't') || (value.Text[value.Offset + 0] == 'T')) &&
            ((value.Text[value.Offset + 1] == 'r') || (value.Text[value.Offset + 1] == 'R')) &&
            ((value.Text[value.Offset + 2] == 'u') || (value.Text[value.Offset + 2] == 'U')) &&
            ((value.Text[value.Offset + 3] == 'e') || (value.Text[value.Offset + 3] == 'E'));
          return isSpellingTrue ? new Nullable<bool>(true) : null;
        }

        // If the string spells 'false', it is considered a boolean
        case 5: {
          bool isSpellingFalse =
            ((value.Text[value.Offset + 0] == 'f') || (value.Text[value.Offset + 0] == 'F')) &&
            ((value.Text[value.Offset + 1] == 'a') || (value.Text[value.Offset + 1] == 'A')) &&
            ((value.Text[value.Offset + 2] == 'l') || (value.Text[value.Offset + 2] == 'L')) &&
            ((value.Text[value.Offset + 3] == 's') || (value.Text[value.Offset + 3] == 'S')) &&
            ((value.Text[value.Offset + 4] == 'e') || (value.Text[value.Offset + 4] == 'E'));
          return isSpellingFalse ? new Nullable<bool>(false) : null;
        }

        // Anything else is not considered a boolean
        default: {
          return null;
        }

      }
    }

  }

} // namespace Nuclex.Support.Configuration
