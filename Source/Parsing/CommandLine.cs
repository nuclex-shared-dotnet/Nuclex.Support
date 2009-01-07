#region CPL License
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
using System.IO;

using Nuclex.Support.Collections;

#if ENABLE_BROKEN_COMMAND_LINE_PARSER

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
  ///         <term>Argument</term>
  ///         <description>
  ///           Either an option or a loose value (see below) that being specified on
  ///           the command line
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Option</term>
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
  ///   <para>
  ///     What this parser doesn't support is spaced assignments (eg. '--format png') since
  ///     these are ambiguous if the parser doesn't know beforehand whether "format" accepts
  ///     a non-optional argument.
  ///   </para>
  /// </remarks>
  public partial class CommandLine {

    /// <summary>Initializes a new command line</summary>
    public CommandLine() {
      this.options = new List<Option>();
      this.values = new List<string>();
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <returns>The parsed command line</returns>
    public static CommandLine Parse(string commandLineString) {
      bool windowsMode = (Path.DirectorySeparatorChar != '/');
      return Parser.Parse(commandLineString, windowsMode);
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    /// <returns>The parsed command line</returns>
    public static CommandLine Parse(string commandLineString, bool windowsMode) {
      return Parser.Parse(commandLineString, windowsMode);
    }

    #region To Be Removed

    /// <summary>Adds a loose value to the command line</summary>
    /// <param name="value">Value taht will be added</param>
    internal void addValue(StringSegment value) {
      Console.WriteLine("Discovered loose value: '" + value.ToString() + "'");

      this.values.Add(value.ToString());
    }

    /// <summary>Adds an option to the command line</summary>
    /// <param name="option">Option that will be added</param>
    internal void addOption(Option option) {
      Console.WriteLine("Discovered option: '" + option.Raw.ToString() + "'");
      Console.WriteLine("  Name: '" + option.Name + "'");
      if(option.Value != null) {
        Console.WriteLine("  Value: '" + option.Value + "'");
      }

      this.options.Add(option);
    }

    #endregion // To Be Removed

    /// <summary>Options that were specified on the command line</summary>
    public IList<Option> Options {
      get { return this.options; }
    }

    /// <summary>Loose values that were given on the command line</summary>
    public IList<string> Values {
      get { return this.values; }
    }

    /// <summary>Options that were specified on the command line</summary>
    private List<Option> options;
    /// <summary>Loose values that were given on the command line</summary>
    private List<string> values;

  }

} // namespace Nuclex.Support.Parsing

#endif // ENABLE_BROKEN_COMMAND_LINE_PARSER
