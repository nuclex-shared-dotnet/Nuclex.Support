using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Source.Parsing {

#if false
  /// <summary>
  ///   Parses an application's command line parameters for easier consumption
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     At the time of the creation of this parser, there are already several command line
  ///     parsing libraries out there. Most of them, however, do way too much at once or at
  ///     the very least use one huge, untested clutter of classes and methods to arrive
  ///     at their results.
  ///   </para>
  ///   <para>
  ///     This parser does nothing more than parse the command line arguments. It doesn't
  ///     interpret them and it doesn't check them for validity. Due to this, it can easily
  ///     be unit-tested and is an ideal building block to create actual command line
  ///     interpreters that connect the parameters to program instructions and or fill
  ///     structures in code.
  ///   </para>
  /// </remarks>
  public class CommandLine {
    public static CommandLine Parse(string commandLine) {}
  }
  
  public struct CommandLineOption {
  
    /// <summary>Contains the raw string the command line argument was parsed from</summary>
    public string Raw;
    /// <summary>Method used to specify the argument (either '-', '--' or '/')</summary>
    public string Method;
    /// <summary>Name of the command line argument</summary>
    public string Name;
    /// <summary>Value that has been assigned to the command line argument</summary>
    public string Value;
    /// <summary>Method used to assign the value (either '=', ':' or ' ')</summary>
    public string Assignment;

  }
#endif

}
