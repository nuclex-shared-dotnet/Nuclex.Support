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

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Parses command line strings</summary>
    private static class Parser {

      /// <summary>Parses a string containing command line arguments</summary>
      /// <param name="commandLineString">String that will be parsed</param>
      /// <returns>The parsed command line arguments from the string</returns>
      public static CommandLine Parse(string commandLineString) {
        CommandLine commandLine = new CommandLine();
        if(commandLineString == null) {
          return commandLine;
        }

/*
        for(int index = 0; index < commandLineString.Length; ) {
          char currentCharacter = commandLineString[index];

          // We ignore whitespaces outside of quoted values
          if(char.IsWhiteSpace(currentCharacter)) {
            continue;
          }

          switch(currentCharacter) {
            case '-':
            case '/': {
              parseArgument(commandLine, commandLineString, ref index);
              break;
            }
            case '"': {
              parseQuotedValue(commandLine, commandLineString, ref index);
              break;
            }
            default: {
              parseUnquotedValue(commandLine, commandLineString, ref index);
              break;
            }
          }
        }
*/
        return null;
      }

/*
      private static void parseArgument(
        CommandLine commandLine, string commandLineString, ref int index
      ) {
      }

      private static void parseQuotedValue(
        CommandLine commandLine, string commandLineString, ref int index
      ) {
      }

      private static void parseUnquotedValue(
        CommandLine commandLine, string commandLineString, ref int index
      ) {
        int endIndex = commandLineString.IndexOfAny(WhitespaceCharacters, index);
        
        StringSegment argument = new StringSegment(commandLineString, index, endIndex - index);
        
      }
      
      private static readonly char[] WhitespaceCharacters = new char[] { ' ', '\t' };
*/
    }

  }

} // namespace Nuclex.Support.Parsing
