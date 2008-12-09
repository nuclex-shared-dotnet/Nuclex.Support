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
using System.Text;

namespace Nuclex.Support.Parsing {

  /// <summary>
  ///   Manages an application's command line parameters
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     At the time of the creation of this component, there are already several command
  ///     line parsing libraries out there. Most of them, however, do way too much at once
  ///     or at the very least rely on huge, untested clutters of classes and methods to
  ///     arrive at their results.
  ///   </para>
  ///   <para>
  ///     This class does nothing more than represent the command line to the application.
  ///     It can parse a command line
  ///     parse the command line arguments. It doesn't
  ///     interpret them and it doesn't check them for validity. This promotes simplicity
  ///     and allows t
  ///     be unit-tested and is an ideal building block to create actual command line
  ///     interpreters that connect the parameters to program instructions and or fill
  ///     structures in code.
  ///   </para>
  ///   <para>
  ///     Terminology
  ///     <list type="table">
  ///       <item>
  ///         <term>Command line</term>
  ///         <description>
  ///           The entire command line either as a string or as
  ///           an already parsed data structure
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Option / Argument</term>
  ///         <description>
  ///           Can be specified on the command line and typically alters the behavior
  ///           of the application or changes a setting. For example, '--normalize' or
  ///           '/safemode'.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Value</term>
  ///         <description>
  ///           Can either sit loosely in the command line (eg. 'update' or 'textfile.txt')
  ///           or as assignment to an option (eg. '--width=1280' or '/overwrite:always')
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  /// </remarks>
  public partial class CommandLine {

    /// <summary>Initializes a new command line</summary>
    public CommandLine() { }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <returns>The parsed command line</returns>
    public static CommandLine Parse(string commandLineString) {
      return Parser.Parse(commandLineString);
    }

  }

} // namespace Nuclex.Support.Parsing
