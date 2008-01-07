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
using System.Reflection;
using System.IO;

namespace Nuclex.Support.Plugins {

  /// <summary>Stores loaded plugins</summary>
  /// <remarks>
  ///   This class manages a set of assemblies that have been dynamically loaded 
  ///   as plugins. It usually is shared by multiple PluginHosts that handle
  ///   different interfaces of one plugin type.
  /// </remarks>
  public class PluginRepository {

    /// <summary>Triggered whenever a new assembly is loaded into this repository</summary>
    public event AssemblyLoadEventHandler AssemblyLoaded;

    /// <summary>Initializes a new instance of the plugin repository</summary>
    public PluginRepository() {
      this.assemblies = new List<Assembly>();
    }

    /// <summary>Loads all plugins matching a wildcard specification</summary>
    /// <param name="wildcard">Path of one or more plugins via wildcard</param>
    /// <remarks>
    ///   This function always assumes that a plugin is optional. This means that
    ///   even when you specify a unique file name and a matching file is not found,
    ///   no exception will be raised and the error is silently ignored.
    /// </remarks>
    public void AddFiles(string wildcard) {
      string directory = Path.GetDirectoryName(wildcard);
      string search = Path.GetFileName(wildcard);

      // We'll scan the specified directory for all files matching the specified
      // wildcard. If only a single file is specified, only that file will match
      // the supposed wildcard and everything works as expected
      string[] assemblyFiles = Directory.GetFiles(directory, search);
      foreach(string assemblyFile in assemblyFiles) {

        // A lot of errors can occur when attempting to load an assembly...
        try {
          AddAssembly(Assembly.LoadFile(assemblyFile));
        }
        // File not found - Most likely a missing dependency of the assembly we
        // attempted to load since the assembly itself has been found by the GetFiles() method
        catch(DllNotFoundException exception) {
          System.Console.WriteLine(
            "Assembly not found, missing dependencies? " + exception.Message
          );
        }
        // Unauthorized acccess - Either the assembly is not trusted because it contains
        // code that imposes a security risk on the system or a user rights problem
        catch(UnauthorizedAccessException exception) {
          System.Console.WriteLine(
            "Not authorized, user rights problem? " + exception.Message
          );
        }
        // Bad image format - This exception is often thrown when the assembly we
        // attempted to load requires a different version of the .NET framework
        catch(BadImageFormatException exception) {
          System.Console.WriteLine(
            "Not a .NET assembly or wrong runtime version. " + exception.Message
          );
        }
        // Unknown error - Our last resort is to show a default error message
        catch(Exception exception) {
          System.Console.WriteLine(
            "Failed to load plugin. " + exception.Message
          );
        }

      }
    }

    /// <summary>Adds the specified assembly to the repository</summary>
    /// <remarks>
    ///   Also used internally, so any assembly that is to be put into the repository,
    ///   not matter how, wanders through this method
    /// </remarks>
    public void AddAssembly(Assembly assembly) {
      this.assemblies.Add(assembly);

      // Trigger event in case any subscribers have been registered
      if(AssemblyLoaded != null)
        AssemblyLoaded(this, new AssemblyLoadEventArgs(assembly));
    }

    /// <summary>List of all loaded plugin assemblies in the repository</summary>
    public List<Assembly> LoadedAssemblies {
      get { return this.assemblies; }
    }

    /// <summary>Loaded plugin assemblies</summary>
    private List<Assembly> assemblies;

  }

} // namespace Nuclex.Support.Plugins
