#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2013 Nuclex Development Labs

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

namespace Nuclex.Support.Parsing {

  /// <summary>Provides helper methods for parsers</summary>
  public class ParserHelper {

    /// <summary>Advances the index past any whitespace in the string</summary>
    /// <param name="text">String which is being indexed</param>
    /// <param name="index">Index that will be advanced</param>
    public static void SkipSpaces(string text, ref int index) {
      if(text == null) {
        return;
      }

      int length = text.Length;
      while(index < length) {
        if(!char.IsWhiteSpace(text, index)) {
          break;
        }

        ++index;
      }
    }

    /// <summary>Advances the index to the next whitespace in the string</summary>
    /// <param name="text">String which is being indexed</param>
    /// <param name="index">Index that will be advanced</param>
    public static void SkipNonSpaces(string text, ref int index) {
      if(text == null) {
        return;
      }

      int length = text.Length;
      while(index < length) {
        if(char.IsWhiteSpace(text, index)) {
          break;
        }

        ++index;
      }
    }

    /// <summary>Advances the index to the next character that isn't numeric</summary>
    /// <param name="text">String which is being indexed</param>
    /// <param name="index">Index that will be advanced</param>
    /// <remarks>
    ///   This skips only numeric characters, but not complete numbers -- if the number
    ///   begins with a minus or plus sign, for example, this function will not skip it.
    /// </remarks>
    public static void SkipNumbers(string text, ref int index) {
      if(text == null) {
        return;
      }

      int length = text.Length;
      while(index < length) {
        if(!char.IsNumber(text, index)) {
          break;
        }

        ++index;
      }
    }

    /// <summary>Skips an integer in the provided string</summary>
    /// <param name="text">String in which an integer will be skipped</param>
    /// <param name="index">Index at which the integer begins</param>
    /// <returns>True if an integer was found and skipped, otherwise false</returns>
    public static bool SkipInteger(string text, ref int index) {
      if(text == null) {
        return false;
      }

      int length = text.Length;
      if(index >= length) {
        return false;
      }

      // If the number begins with a minus or plus sign, skip over the sign
      int nextIndex;
      if((text[index] == '-') || (text[index] == '+')) {
        nextIndex = index + 1;

        SkipNumbers(text, ref nextIndex);
        if(nextIndex == (index + 1)) {
          return false;
        }
      } else {
        nextIndex = index;

        SkipNumbers(text, ref nextIndex);
        if(nextIndex == index) {
          return false;
        }
      }

      index = nextIndex;
      return true;
    }

    /// <summary>Skips a string appearing in the input text</summary>
    /// <param name="text">Text in which a string will be skipped</param>
    /// <param name="index">Index at which the string begins</param>
    /// <returns>True if a string was found and skipped, otherwise false</returns>
    public static bool SkipString(string text, ref int index) {
      if(text == null) {
        return false;
      }

      int length = text.Length;
      if(index >= length) {
        return false;
      }

      // If the string begins with an opening quote, look for the closing quote
      if(text[index] == '"') {

        int endIndex = text.IndexOf('"', index + 1);
        if(endIndex == -1) {
          return false;
        }

        index = endIndex + 1;
        return true;

      } else { // Normal strings end with the first whitespace

        int startIndex = index;
        SkipNonSpaces(text, ref index);

        return (index != startIndex);

      }
    }

  }

} // namespace Nuclex.Support.Parsing
