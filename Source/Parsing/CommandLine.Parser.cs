#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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
using System.Diagnostics;

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Parses command line strings</summary>
    private class Parser {

      /// <summary>Initializes a new command line parser</summary>
      private Parser() {
        this.commandLine = new CommandLine();
      }

      /// <summary>Parses a string containing command line arguments</summary>
      /// <param name="commandLineString">String that will be parsed</param>
      /// <returns>The parsed command line arguments from the string</returns>
      public static CommandLine Parse(string commandLineString) {
        Parser theParser = new Parser();
        theParser.parse(commandLineString);
        return theParser.commandLine;
      }

      /// <summary>
      ///   Parses the provided string and adds the parameters found to
      ///   the command line representation
      /// </summary>
      /// <param name="commandLineString">
      ///   String containing the command line arguments that will be parsed
      /// </param>
      private void parse(string commandLineString) {
        if(commandLineString == null) {
          return;
        }

        // Walk through the command line character by character and gather
        // the parameters and values to build the command line representation from
        for(int index = 0; index < commandLineString.Length; ) {

          // Look for the next non-whitespace character
          index = StringHelper.IndexNotOfAny(
            commandLineString, WhitespaceCharacters, index
          );
          if(index == -1) {
            break;
          }

          // Parse the chunk of characters at this location and advance the index
          // to the next location after the chunk of characters
          index += parseCharacterChunk(commandLineString, index);
        }
      }

      /// <summary>
      ///   Parses a chunk of characters and adds it as an option or a loose value to
      ///   the command line representation we're building
      /// </summary>
      /// <param name="commandLineString">
      ///   String containing the chunk of characters that will be parsed
      /// </param>
      /// <param name="index">Index in the string at which to begin parsing</param>
      /// <returns>The number of characters that were consumed</returns>
      private int parseCharacterChunk(string commandLineString, int index) {
        int startIndex = index;

        char currentCharacter = commandLineString[index];
        switch(currentCharacter) {

          // Unix style argument using either '-' or "--" as its initiator
          case '-': {
            ++index;

            // Does the string end here? Stop parsing.
            if(index >= commandLineString.Length) {
              addValue("-");
              break;
            }

            // Does another '-' follow? Might be a unix style option or a loose "--"
            if(commandLineString[index] == '-') {
              ++index;
              index += parsePotentialOption(commandLineString, startIndex, index);
            } else { // Nope, it's a normal option or a loose '-'
              index += parsePotentialOption(commandLineString, startIndex, index);
            }

            break;
          }

          // Windows style argument using '/' as its initiator
          case '/': {
            ++index;
            index += parsePotentialOption(commandLineString, startIndex, index);
            break;
          }

          // Quoted loose value
          case '"': {
            StringSegment value = parseQuotedValue(commandLineString, index);
            index += value.Count + 1;
            break;
          }

          // Unquoted loose value
          default: {
            StringSegment value = parseNakedValue(commandLineString, index);
            index += value.Count;
            break;
          }

        }

        return index - startIndex;
      }

      /// <summary>Parses a potential command line option</summary>
      /// <param name="commandLineString">String containing the command line arguments</param>
      /// <param name="initiatorStartIndex">
      ///   Index of the option's initiator ('-' or '--' or '/')
      /// </param>
      /// <param name="index">
      ///   Index at which the option name is supposed start (if it's an actual option)
      /// </param>
      /// <returns>The number of characters consumed</returns>
      private int parsePotentialOption(
        string commandLineString, int initiatorStartIndex, int index
      ) {

        // If the string ends here this can only be considered as a loose value
        if(index >= commandLineString.Length) {
          addValue(commandLineString.Substring(initiatorStartIndex));
          return 0;
        }

        // Look for the first character that ends the option. If it is not an actual option,
        // the very first character might be the end
        int nameEndIndex = commandLineString.IndexOfAny(OptionNameEndingCharacters, index);
        if(nameEndIndex == -1) {
          nameEndIndex = commandLineString.Length;
        }
        
        // If the first character of the supposed option is not valid for an option,
        // we have to consider this to be a loose value        
        if(nameEndIndex == index) {
          // Parse normal unquoted value
          //parseNakedValue(commandLineString, initiatorStartIndex).Count;
          /*
          int endIndex = commandLineString.IndexOfAny(WhitespaceCharacters, index);
          if(endIndex == -1) {
            addValue(commandLineString.Substring(initiatorStartIndex));
            return commandLineString.Length - index;
          } else {
            addValue(
              commandLineString.Substring(initiatorStartIndex, endIndex - initiatorStartIndex)
            );
            return endIndex - index;
          }
          */
        }
        
        Console.WriteLine(
          "Argument name: " + commandLineString.Substring(index, nameEndIndex - index)
        );

        // TODO: Parse argument value (if provided) here!!

        return nameEndIndex - index;
      }

      static readonly char[] OptionNameEndingCharacters = new char[] {
        ' ', '\t', '=', ':', '/', '-', '+', '"'
      };

      /// <summary>Parses a quoted value from the input string</summary>
      /// <param name="commandLineString">String the quoted value is parsed from</param>
      /// <param name="index">Index at which the quoted value begins</param>
      /// <returns>A string segment containing the parsed quoted value</returns>
      /// <remarks>
      ///   The returned string segment does not include the quotes.
      /// </remarks>
      private static StringSegment parseQuotedValue(string commandLineString, int index) {
        char quoteCharacter = commandLineString[index];
        ++index;

        int endIndex = commandLineString.IndexOf(quoteCharacter, index);
        if(endIndex == -1) {
          endIndex = commandLineString.Length;
        }

        // TODO: We don't skip the closing quote, the callee would have to detect it himself
        
        return new StringSegment(commandLineString, index, endIndex - index);
      }

      /// <summary>Parses a plain, unquoted value from the input string</summary>
      /// <param name="commandLineString">String containing the value to be parsed</param>
      /// <param name="index">Index at which the value begins</param>
      /// <returns>A string segment containing the parsed value</returns>
      private static StringSegment parseNakedValue(string commandLineString, int index) {
        int endIndex = commandLineString.IndexOfAny(WhitespaceCharacters, index);
        if(endIndex == -1) {
          endIndex = commandLineString.Length;
        }

        return new StringSegment(commandLineString, index, endIndex - index);
      }

      /// <summary>
      ///   Determines whether the specified character is valid as the first character
      ///   in an option
      /// </summary>
      /// <param name="character">Character that will be tested for validity</param>
      /// <returns>True if the character is valid as the first character in an option</returns>
      private static bool isValidFirstCharacterInOption(char character) {
        const string InvalidCharacters = " \t=:/-+\"";
        return (InvalidCharacters.IndexOf(character) == -1);
      }


      private void addValue(string value) {
        Console.WriteLine("Added Value: '" + value + "'");
      }


      /// <summary>Characters the parser considers to be whitespace</summary>
      private static readonly char[] WhitespaceCharacters = new char[] { ' ', '\t' };

      /// <summary>Command line currently being built by the parser</summary>
      private CommandLine commandLine;

    }

  }

} // namespace Nuclex.Support.Parsing
