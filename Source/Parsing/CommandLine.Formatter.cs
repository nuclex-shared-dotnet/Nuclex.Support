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
using System.Text;

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Formats a command line instance into a string</summary>
    internal static class Formatter {

      /// <summary>
      ///   Formats all arguments in the provided command line instance into a string
      /// </summary>
      /// <param name="commandLine">Command line instance that will be formatted</param>
      /// <returns>All arguments in the command line instance as a string</returns>
      public static string FormatCommandLine(CommandLine commandLine) {
        int totalLength = 0;
        for(int index = 0; index < commandLine.arguments.Count; ++index) {
          if(index != 0) {
            ++totalLength; // For spacing between arguments
          }

          totalLength += commandLine.arguments[index].RawLength;
        }

        StringBuilder builder = new StringBuilder(totalLength);
        for(int index = 0; index < commandLine.arguments.Count; ++index) {
          if(index != 0) {
            builder.Append(' ');
          }

          builder.Append(commandLine.arguments[index].Raw);
        }

        return builder.ToString();
      }

    }

  }

} // namespace Nuclex.Support.Parsing
