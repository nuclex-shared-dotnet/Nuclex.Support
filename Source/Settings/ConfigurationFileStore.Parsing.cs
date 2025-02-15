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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Nuclex.Support.Parsing;

namespace Nuclex.Support.Settings {

  partial class ConfigurationFileStore {

    #region class ParserState

    /// <summary>Remembers the target store and current category of the parser</summary>
    private class ParserState {

      /// <summary>Store to which the parsed categories and options will be added</summary>
      public ConfigurationFileStore Store;

      /// <summary>Current category options belong to</summary>
      public Category Category;

    }

    #endregion // class ParserState

    /// <summary>Parses a configuration file from the specified text reader</summary>
    /// <param name="reader">Reader the configuration file will be parsed from</param>
    /// <returns>The configuration file parsed from the specified reader</returns>
    public static ConfigurationFileStore Parse(TextReader reader) {
      var store = new ConfigurationFileStore();
      var state = new ParserState() {
        Store = store,
        Category = store.rootCategory
      };

      for(; ; ) {
        string line = reader.ReadLine();
        if(line == null) {
          break;
        }

        parseLine(state, line);
      }

      return store;
    }

    /// <summary>Incrementally parses a line read from a configuration file</summary>
    /// <param name="state">Current parser state</param>
    /// <param name="line">Line that has been read</param>
    private static void parseLine(ParserState state, string line) {

      // If the line is empty, ignore it
      int length = line.Length;
      if(length == 0) {
        return;
      }

      // Skip all spaces at the beginning of the line
      int firstCharacterIndex = 0;
      ParserHelper.SkipSpaces(line, ref firstCharacterIndex);

      // If the line contained nothing but spaces, ignore it
      if(firstCharacterIndex == length) {
        return;
      }

      // If the line is a comment, ignore it
      if((line[firstCharacterIndex] == '#') || (line[firstCharacterIndex] == ';')) {
        return;
      }

      // Now the line is either a category definition or some attempt to set an option
      if(line[firstCharacterIndex] == '[') {
        parseCategory(state, line, firstCharacterIndex);
      } else {
        parseOption(state, line, firstCharacterIndex);
      }
      state.Category.Lines.Add(line);

    }

    /// <summary>Parses a category definition encountered on a line</summary>
    /// <param name="state">Current parser state</param>
    /// <param name="line">Line containing the category definition</param>
    /// <param name="firstCharacterIndex">Index of the definition's first character</param>
    private static void parseCategory(
      ParserState state, string line, int firstCharacterIndex
    ) {
      Debug.Assert(line[firstCharacterIndex] == '[');

      int nameStartIndex = firstCharacterIndex + 1;
      ParserHelper.SkipSpaces(line, ref nameStartIndex);

      int lastCharacterIndex = line.Length - 1;
      if(nameStartIndex >= lastCharacterIndex) {
        return; // No space left for closing brace
      }

      int nameEndIndex = line.IndexOf(']', nameStartIndex);
      if(nameEndIndex == -1) {
        return; // No closing brace in line
      }

      // Skip any whitespaces between the last character and the closing brace
      do {
        --nameEndIndex;
      } while(char.IsWhiteSpace(line, nameEndIndex));

      // Now we know that the line holds a category definition and where exactly in
      // the line the category name is located. Create the category.
      state.Category = new Category() {
        CategoryName = new StringSegment(
          line, nameStartIndex, nameEndIndex - nameStartIndex + 1
        ),
        OptionLookup = new Dictionary<string, Option>(),
        Lines = new List<string>()
      };
      state.Store.categoryLookup.Add(state.Category.CategoryName.ToString(), state.Category);
    }

    /// <summary>Parses an option definition encountered on a line</summary>
    /// <param name="state">Current parser state</param>
    /// <param name="line">Line containing the option definition</param>
    /// <param name="firstCharacterIndex">Index of the definition's first character</param>
    private static void parseOption(
      ParserState state, string line, int firstCharacterIndex
    ) {
      int assignmentIndex = line.IndexOf('=', firstCharacterIndex + 1);
      if(assignmentIndex == -1) {
        return; // No assignment took place
      }

      // Cut off any whitespaces between the option name and the assignment
      int nameEndIndex = assignmentIndex;
      do {
        --nameEndIndex;
      } while(char.IsWhiteSpace(line, nameEndIndex));

      // We have enough information to know that this is an assignment of some kind
      Option option = new Option() {
        LineIndex = state.Category.Lines.Count,
        OptionName = new StringSegment(
          line, firstCharacterIndex, nameEndIndex - firstCharacterIndex + 1
        )
      };

      parseOptionValue(option, line, assignmentIndex + 1);

      // We've got the option assignment, either with an empty or proper value
      state.Store.options.Add(option);
      state.Category.OptionLookup.Add(option.OptionName.ToString(), option);
    }

    /// <summary>Parses the value assigned to an option</summary>
    /// <param name="option">Option to which a value is being assigned</param>
    /// <param name="line">Line containing the option assignment</param>
    /// <param name="assignmentEndIndex">Index one after the assignment character</param>
    private static void parseOptionValue(Option option, string line, int assignmentEndIndex) {
      int firstCharacterIndex = assignmentEndIndex;
      ParserHelper.SkipSpaces(line, ref firstCharacterIndex);

      // Just for beauty, when the option value is empty but padded with spaces,
      // leave one space between the equals sign and the value.
      if(firstCharacterIndex > assignmentEndIndex) {
        ++assignmentEndIndex;
      }

      // If the line consists of only whitespace, create an empty value
      if(firstCharacterIndex == line.Length) {
        option.OptionValue = new StringSegment(line, assignmentEndIndex, 0);
        return;
      }

      char firstCharacter = line[firstCharacterIndex];

      // Values can be quoted to allow for comments characters appearing in them
      int lastCharacterIndex;
      if(firstCharacter == '"') {
        lastCharacterIndex = line.LastIndexOf('"');
      } else {
        lastCharacterIndex = firstCharacterIndex;
      }

      int commentStartIndex = line.IndexOf(';', lastCharacterIndex);
      if(commentStartIndex == -1) {
        commentStartIndex = line.IndexOf('#', lastCharacterIndex);
      }
      if(commentStartIndex == -1) {
        lastCharacterIndex = line.Length - 1;
      } else {
        lastCharacterIndex = commentStartIndex - 1;
      }

      while(lastCharacterIndex > firstCharacterIndex) {
        if(char.IsWhiteSpace(line, lastCharacterIndex)) {
          --lastCharacterIndex;
        } else {
          break;
        }
      }

      option.OptionValue = new StringSegment(
        line, firstCharacterIndex, lastCharacterIndex - firstCharacterIndex + 1
      );
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
          if(index >= value.Offset + value.Count) {
            return typeof(int);
          }
          if(value.Text[index] == '.') {
            return typeof(float);
          }
        }
      } else { // If it's just a single character, it may be a number
        if(char.IsNumber(value.Text, value.Offset)) {
          return typeof(int);
        }
      }

      // If it parses as a boolean literal, then it must be a boolean
      if(parseBooleanLiteral(ref value) != null) {
        return typeof(bool);
      }

      return typeof(string);
    }

    /// <summary>Tries to parse a boolean literal</summary>
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
