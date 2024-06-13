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
    public static void SkipNumericals(string text, ref int index) {
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

        SkipNumericals(text, ref nextIndex);
        if(nextIndex == (index + 1)) {
          return false;
        }
      } else {
        nextIndex = index;

        SkipNumericals(text, ref nextIndex);
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

    /// <summary>Skips a floating point value appearing in the input text</summary>
    /// <param name="text">Text in which a floating point value will be skipped</param>
    /// <param name="index">Index at which the floating point value begins</param>
    /// <returns>True if the floating point value was skipped, otherwise false</returns>
    public static bool SkipFloat(string text, ref int index) {
      if(SkipInteger(text, ref index)) {
        if(index < text.Length) {
          if(text[index] == '.') {
            ++index;
            SkipNumericals(text, ref index);
          }
          if((text[index] == 'e') || (text[index] == 'E')) {
            throw new NotImplementedException("Exponential format not supported yet");
          }
        }

        return true;
      }

      return false;
    }

  }

} // namespace Nuclex.Support.Parsing
