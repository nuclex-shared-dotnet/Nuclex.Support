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

using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Nuclex.Support.Parsing {

  /// <summary>Parses an application's command line</summary>
  /// <remarks>
  ///   <para>
  ///     Based on an article Richard Lopes published on "The Code Project" at
  ///     http://www.codeproject.com/csharp/command_line.asp
  ///   </para>
  ///   <para>
  ///    Valid forms for command line arguments: {-|/|--}param[{ |=|:}[{"|'}]value[{"|'}]]
  ///   </para>
  ///   <example>
  ///     <code>
  ///       -param1 value1
  ///       --param2
  ///       /param3:"Test-:-work"
  ///       /param4=happy
  ///       -param5 '--=nice=--'
  ///     </code>
  ///   </example>
  /// </remarks>
  public class CommandLineParser {

    /// <summary>
    ///   Initializes a new command line parser using the running program's command line
    /// </summary>
    public CommandLineParser() : this(System.Environment.CommandLine) { }

    /// <summary>Initializes a new command line parser</summary>
    /// <param name="arguments">All supplied command line arguments as a single string</param>
    public CommandLineParser(string arguments)
      : this(arguments.Split(new char[] { ' ', '\t' })) { }

    /// <summary>Initializes a new command line parser</summary>
    /// <param name="arguments">Arguments that have been passed in the command line</param>
    public CommandLineParser(string[] arguments) {
      this.arguments = new StringDictionary();
      this.values = new StringCollection();

      string activeParameter = null;

      foreach(string argument in arguments) {

        // Look for arguments ('-', '/', '--') with their assignments ('=', ':')
        string[] parts = splitter.Split(argument, 3);
        switch(parts.Length) {

          // Value found without an argument being specified (eg. file name)
          case 1: {

            if(activeParameter != null) {
              if(!this.arguments.ContainsKey(activeParameter)) {
                parts[0] = remover.Replace(parts[0], "$1");
                this.arguments.Add(activeParameter, parts[0]);
              }
              activeParameter = null;
            } else {
              this.values.Add(parts[0]);
            }

            // Error: No argument is waiting for a value. Skip this argument.
            break;
          }

          // Found an argument with no value assignment
          case 2: {

            // In case the previous argument is still waiting for a value we need to finish
            // it up before switching to the argument we just found.
            if(activeParameter != null)
              if(!this.arguments.ContainsKey(activeParameter))
                this.arguments.Add(activeParameter, null);

            // Remember argument to allow for a later value assignment
            activeParameter = parts[1];

            break;
          }

          // Found an argument with a proper assignment declaration
          case 3: {

            // In case the previous argument is still waiting for a value we need to finish
            // it up before switching to the argument we just found.
            if(activeParameter != null)
              if(!this.arguments.ContainsKey(activeParameter))
                this.arguments.Add(activeParameter, null);

            activeParameter = parts[1];

            // Remove any quotes that might be enclosing this argument (",')
            if(!this.arguments.ContainsKey(activeParameter)) {
              parts[2] = remover.Replace(parts[2], "$1");
              this.arguments.Add(activeParameter, parts[2]);
            }

            activeParameter = null;

            break;
          }
        }
      }

      // In case the previous argument is still waiting for a value we need to finish
      // it up before leaving the parsing method.
      if(activeParameter != null) {
        if(!this.arguments.ContainsKey(activeParameter)) {
          this.arguments.Add(activeParameter, null);
        }
      }
    }

    /// <summary>Returns the value of an argument by the argument's name</summary>
    /// <param name="argumentName">Name of the argument whose value will be returned</param>
    /// <returns>The value of the argument with the specified name</returns>
    public string this[string argumentName] {
      get { return this.arguments[argumentName]; }
    }

    /// <summary>
    ///   Checks whether the specified argument was specified on the command line
    /// </summary>
    /// <param name="argumentName">Name of the argument to check</param>
    /// <returns>True if the specified command was given on the command line</returns>
    public bool HasArgument(string argumentName) {
      return this.arguments.ContainsKey(argumentName);
    }

    /// <summary>
    ///   Any values loosely specified on the command line without being assigned
    ///   to an argument.
    /// </summary>
    public StringCollection Values {
      get { return this.values; }
    }

    /// <summary>
    ///   Regular Expression used to split the arguments and their assigned values
    /// </summary>
    private static Regex splitter =
      new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    ///   Regular Expression used to remove quotations around an argument's value
    /// </summary>
    private static Regex remover =
      new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Stores the parsed arguments</summary>
    private StringDictionary arguments;
    /// <summary>
    ///   Stores any values passed on the command line without assigning an argument
    /// </summary>
    private StringCollection values;

  }

} // namespace Nuclex.Support.Parsing
