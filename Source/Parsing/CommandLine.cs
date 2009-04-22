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
using System.Text;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Parsing {

  /// <summary>Parses and stores an application's command line parameters</summary>
  /// <remarks>
  ///   <para>
  ///     At the time of the creation of this component, there are already several command
  ///     line parsing libraries out there. Most of them, however, do way too much at once
  ///     or at the very least rely on huge, untested clutters of classes and methods to
  ///     arrive at their results.
  ///   </para>
  ///   <para>
  ///     This command line parser does nothing more than represent the command line to
  ///     the application through a convenient interface. It parses a command line and
  ///     extracts the arguments, but doesn't interpret them and or check them for validity.
  ///   </para>
  ///   <para>
  ///     This design promotes simplicity and makes is an ideal building block to create
  ///     actual command line interpreters that connect the parameters to program
  ///     instructions and or fill structures in code.
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
  ///           Either an option or a loose value (see below) being specified on
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
  /// </remarks>
  public partial class CommandLine {

    /// <summary>Initializes a new command line</summary>
    public CommandLine() : this(new List<Argument>()) { }

    /// <summary>Initializes a new command line</summary>
    /// <param name="argumentList">List containing the parsed arguments</param>
    private CommandLine(List<Argument> argumentList) {
      this.arguments = argumentList;
    }

#if ENABLE_TOKENIZED_COMMAND_LINE_PARSING // don't enable, it's broken!
    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineArguments">Command line tokens that will be parsed</param>
    /// <returns>The parsed command line</returns>
    public static CommandLine Parse(string[] commandLineArguments) {
      bool windowsMode = (Path.DirectorySeparatorChar != '/');
      return Parse(commandLineArguments, windowsMode);
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineArguments">Command line tokens that will be parsed</param>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    /// <returns>The parsed command line</returns>
    public static CommandLine Parse(string[] commandLineArguments, bool windowsMode) {
      return new CommandLine(
        Parser.Parse(commandLineArguments, windowsMode)
      );
    }
#endif // ENABLE_TOKENIZED_COMMAND_LINE_PARSING

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <returns>The parsed command line</returns>
    /// <remarks>
    ///   You should always pass Environment.CommandLine to this methods to avoid
    ///   some problems with the build-in command line tokenizer in .NET
    ///   (which splits '--test"hello world"/v' into '--testhello world/v')
    /// </remarks>
    public static CommandLine Parse(string commandLineString) {
      bool windowsMode = (Path.DirectorySeparatorChar != '/');
      return Parse(commandLineString, windowsMode);
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    /// <returns>The parsed command line</returns>
    /// <remarks>
    ///   You should always pass Environment.CommandLine to this methods to avoid
    ///   some problems with the build-in command line tokenizer in .NET
    ///   (which splits '--test"hello world"/v' into '--testhello world/v')
    /// </remarks>
    public static CommandLine Parse(string commandLineString, bool windowsMode) {
      return new CommandLine(
        Parser.Parse(commandLineString, windowsMode)
      );
    }

    /// <summary>Returns whether an argument with the specified name exists</summary>
    /// <param name="name">Name of the argument whose existence will be checked</param>
    /// <returns>True if an argument with the specified name exists</returns>
    public bool HasArgument(string name) {
      return (indexOfArgument(name) != -1);
    }

#if false
    /// <summary>Retrieves the value of the specified argument</summary>
    /// <param name="name">Name of the argument whose value will be retrieved</param>
    /// <returns>The value of the specified argument</returns>
    public string GetValue(string name) {
      int index = indexOfArgument(name);
      if(index == -1) {
        return null;
      }

      // Does this argument have a value?
      Argument argument = this.arguments[index];
      if(argument.Value != null) {
        return argument.Value;
      } else { // No, it might be a spaced argument

        // See if anything more follows this argument
        ++index;
        if(index < this.arguments.Count) {

          // If something follows the argument, and it is not an option of its own,
          // use its value as the value for the preceding argument
          argument = this.arguments[index];
          if(argument.Name == null) {
            return argument.Value;
          }

        }

      }

      // No argument found
      return null;
    }
#endif

    /// <summary>Retrieves the index of the argument with the specified name</summary>
    /// <param name="name">Name of the argument whose index will be returned</param>
    /// <returns>
    ///   The index of the indicated argument of -1 if no argument with that name exists
    /// </returns>
    private int indexOfArgument(string name) {
      for(int index = 0; index < this.arguments.Count; ++index) {
        if(this.arguments[index].Name == name) {
          return index;
        }
      }

      return -1;
    }


    /// <summary>Options that were specified on the command line</summary>
    public IList<Argument> Arguments {
      get { return this.arguments; }
    }

    /// <summary>Options that were specified on the command line</summary>
    private List<Argument> arguments;

  }

} // namespace Nuclex.Support.Parsing
