﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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
using System.IO;

#if ENABLE_BROKEN_COMMAND_LINE_PARSER

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Parses command line strings</summary>
    private class Parser {

      /// <summary>Initializes a new command line parser</summary>
      /// <param name="windowsMode">Whether the / character initiates an argument</param>
      private Parser(bool windowsMode) {
        this.windowsMode = windowsMode;
        this.commandLine = new CommandLine();
      }

      /// <summary>Parses a string containing command line arguments</summary>
      /// <param name="commandLineString">String that will be parsed</param>
      /// <param name="windowsMode">Whether the / character initiates an argument</param>
      /// <returns>The parsed command line arguments from the string</returns>
      public static CommandLine Parse(string commandLineString, bool windowsMode) {
        Console.WriteLine("Parsing '" + commandLineString + "'");
        Parser theParser = new Parser(windowsMode);
        theParser.parseFullCommandLine(commandLineString);
        return theParser.commandLine;
      }

      /// <summary>
      ///   Parses the provided string and adds the parameters found to
      ///   the command line representation
      /// </summary>
      /// <param name="commandLineString">
      ///   String containing the command line arguments that will be parsed
      /// </param>
      private void parseFullCommandLine(string commandLineString) {
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
          parseChunk(commandLineString, ref index);

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
      private void parseChunk(string commandLineString, ref int index) {
        int startIndex = index;

        char currentCharacter = commandLineString[index];
        switch(currentCharacter) {

          // Unix style argument using either '-' or "--" as its initiator
          case '-': {
            ++index;

            // Does the string end here? Stop parsing.
            if(index >= commandLineString.Length) {
              this.commandLine.addValue(new StringSegment(commandLineString, startIndex, 1));
              break;
            }

            // Does another '-' follow? Might be a unix style option or a loose "--"
            if(commandLineString[index] == '-') {
              ++index;
            }

            parsePotentialOption(commandLineString, startIndex, ref index);

            break;
          }

          // Windows style argument using '/' as its initiator
          case '/': {
            // The '/ character is only used to initiate argument on windows and can be
            // toggled off. The application decides, whether this is done depending on the
            // operating system or whether uniform behavior across platforms is desired.
            if(!this.windowsMode) {
              goto default;
            }

            ++index;
            parsePotentialOption(commandLineString, startIndex, ref index);
            break;
          }

          // Quoted loose value
          case '"': {
            parseQuotedValue(commandLineString, ref index);
            break;
          }

          // Unquoted loose value
          default: {
            parseNakedValue(commandLineString, ref index);
            break;
          }

        }
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
      private void parsePotentialOption(
        string commandLineString, int initiatorStartIndex, ref int index
      ) {

        // If the string ends here this can only be considered as a loose value
        if(index == commandLineString.Length) {
          this.commandLine.addValue(
            new StringSegment(
              commandLineString,
              initiatorStartIndex,
              commandLineString.Length - initiatorStartIndex
            )
          );
          return;
        }

        int nameStartIndex = index;

        // Look for the first character that ends the option. If it is not an actual option,
        // the very first character might be the end
        index = commandLineString.IndexOfAny(OptionNameEndingCharacters, nameStartIndex);
        if(index == -1) {
          index = commandLineString.Length;
        }

        // If the first character of the supposed option is not valid for an option name,
        // we have to consider this to be a loose value        
        if(index == nameStartIndex) {
          index = commandLineString.IndexOfAny(WhitespaceCharacters, index);
          if(index == -1) {
            index = commandLineString.Length;
          }

          commandLine.addValue(
            new StringSegment(
              commandLineString, initiatorStartIndex, index - initiatorStartIndex
            )
          );
          return;
        }

        parseOptionAssignment(
          commandLineString, initiatorStartIndex, nameStartIndex, ref index
        );
      }

      /// <summary>Parses the value assignment in a command line option</summary>
      /// <param name="commandLineString">String containing the command line arguments</param>
      /// <param name="initiatorStartIndex">
      ///   Position of the character that started the option
      /// </param>
      /// <param name="nameStartIndex">
      ///   Position of the first character in the option's name
      /// </param>
      /// <param name="index">Index at which the option name ended</param>
      private void parseOptionAssignment(
        string commandLineString, int initiatorStartIndex, int nameStartIndex, ref int index
      ) {
        int nameEndIndex = index;
        int valueStartIndex;
        int valueEndIndex;

        if(index == commandLineString.Length) {
          valueStartIndex = -1;
          valueEndIndex = -1;
        } else {

          char currentCharacter = commandLineString[index];
          bool isAssignment =
            (currentCharacter == ':') ||
            (currentCharacter == '=');

          // Does the string end after the suspected assignment character?
          bool argumentEndReached = ((index + 1) == commandLineString.Length);

          if(isAssignment) {
            parseOptionValue(commandLineString, initiatorStartIndex, nameStartIndex, ref index);
            return;
          } else {

            bool isModifier =
              (currentCharacter == '+') ||
              (currentCharacter == '-');

            if(isModifier) {
              valueStartIndex = index;
              ++index;
              valueEndIndex = index;
            } else {
              valueStartIndex = -1;
              valueEndIndex = -1;
            }
          }
        }

        int argumentLength = index - initiatorStartIndex;
        this.commandLine.addOption(
          new Option(
            new StringSegment(commandLineString, initiatorStartIndex, argumentLength),
            nameStartIndex, nameEndIndex - nameStartIndex,
            valueStartIndex, valueEndIndex - valueStartIndex
          )
        );
      }

      /// <summary>Parses the value assignment in a command line option</summary>
      /// <param name="commandLineString">String containing the command line arguments</param>
      /// <param name="initiatorStartIndex">
      ///   Position of the character that started the option
      /// </param>
      /// <param name="nameStartIndex">
      ///   Position of the first character in the option's name
      /// </param>
      /// <param name="index">Index at which the option name ended</param>
      private void parseOptionValue(
        string commandLineString, int initiatorStartIndex, int nameStartIndex, ref int index
      ) {
        int nameEndIndex = index;
        int valueStartIndex, valueEndIndex;

        // Does the string end after the suspected assignment character?
        bool argumentEndReached = ((index + 1) == commandLineString.Length);

        if(argumentEndReached) {
          ++index;
          valueStartIndex = -1;
          valueEndIndex = -1;
        } else {
          char nextCharacter = commandLineString[index + 1];

          // Is this a quoted assignment
          if(nextCharacter == '"') {
            index += 2;
            valueStartIndex = index;
            index = commandLineString.IndexOf('"', index);
            if(index == -1) {
              index = commandLineString.Length;
              valueEndIndex = index;
            } else {
              valueEndIndex = index;
              ++index;
            }
          } else { // Nope, assuming unquoted assignment or empty assignment
            ++index;
            valueStartIndex = index;
            index = commandLineString.IndexOfAny(WhitespaceCharacters, index);
            if(index == -1) {
              index = commandLineString.Length;
              valueEndIndex = index;
            } else {
              if(index == valueStartIndex) {
                valueStartIndex = -1;
                valueEndIndex = -1;
              } else {
                valueEndIndex = index;
              }
            }
          }
        }

        int argumentLength = index - initiatorStartIndex;
        this.commandLine.addOption(
          new Option(
            new StringSegment(commandLineString, initiatorStartIndex, argumentLength),
            nameStartIndex, nameEndIndex - nameStartIndex,
            valueStartIndex, valueEndIndex - valueStartIndex
          )
        );
      }

      /// <summary>Parses a quoted value from the input string</summary>
      /// <param name="commandLineString">String the quoted value is parsed from</param>
      /// <param name="index">Index at which the quoted value begins</param>
      private void parseQuotedValue(string commandLineString, ref int index) {
        char quoteCharacter = commandLineString[index];
        int startIndex = index + 1;

        // Search for the closing quote
        index = commandLineString.IndexOf(quoteCharacter, startIndex);
        if(index == -1) {
          index = commandLineString.Length; // value ends at string end
          commandLine.addValue(
            new StringSegment(commandLineString, startIndex, index - startIndex)
          );
        } else { // A closing quote was found
          commandLine.addValue(
            new StringSegment(commandLineString, startIndex, index - startIndex)
          );
          ++index; // Skip the closing quote
        }
      }

      /// <summary>Parses a plain, unquoted value from the input string</summary>
      /// <param name="commandLineString">String containing the value to be parsed</param>
      /// <param name="index">Index at which the value begins</param>
      private void parseNakedValue(string commandLineString, ref int index) {
        int startIndex = index;

        index = commandLineString.IndexOfAny(WhitespaceCharacters, index);
        if(index == -1) {
          index = commandLineString.Length;
        }

        commandLine.addValue(
          new StringSegment(commandLineString, startIndex, index - startIndex)
        );
      }

      /// <summary>Characters which end an option name when they are encountered</summary>
      private static readonly char[] OptionNameEndingCharacters = new char[] {
        ' ', '\t', '=', ':', '/', '-', '+', '"'
      };

      /// <summary>Characters the parser considers to be whitespace</summary>
      private static readonly char[] WhitespaceCharacters = new char[] { ' ', '\t' };

      /// <summary>Command line currently being built by the parser</summary>
      private CommandLine commandLine;
      /// <summary>Whether the '/' character initiates an argument</summary>
      private bool windowsMode;

    }

  }

} // namespace Nuclex.Support.Parsing

#endif // ENABLE_BROKEN_COMMAND_LINE_PARSER
