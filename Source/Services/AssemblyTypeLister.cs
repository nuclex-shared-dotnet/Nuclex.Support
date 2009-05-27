﻿#region CPL License
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
using System.Reflection;

namespace Nuclex.Support.Services {

  /// <summary>Lists all types in a changing set of assemblies</summary>
  public abstract class AssemblyTypeLister : ITypeLister {

    #region class AssemblyTypes

    /// <summary>Caches the list of types types for an assembly</summary>
    private class AssemblyTypes {

      /// <summary>Initializes a new cached assembly types list</summary>
      /// <param name="assembly">Assembly the types are found in</param>
      /// <param name="types">Types defined in the assembly</param>
      public AssemblyTypes(Assembly assembly, Type[] types) {
        this.Assembly = assembly;
        this.Types = types;
      }

      /// <summary>Assembly the types are found in</summary>
      public Assembly Assembly;
      /// <summary>Types defined in the assembly</summary>
      public Type[] Types;

    }

    #endregion // class AssemblyTypes

    /// <summary>Initializes a new assembly type lister</summary>
    public AssemblyTypeLister() {
      this.assemblyTypes = new LinkedList<AssemblyTypes>();
    }

    /// <summary>Enumerates all types in the lister's assembly set</summary>
    /// <returns>An enumerator over all types in the lister's assembly set</returns>
    public IEnumerable<Type> GetTypes() {

      // Make sure the assembly list is filled and up-to-date
      if(this.assemblyTypes.Count == 0) {
        enlistAssembliesFirstTime();
      } else {
        updateAssemblyList();
      }

      // Iterate over all types in all assemblies
      LinkedListNode<AssemblyTypes> node = this.assemblyTypes.First;
      while(node != null) {
        Type[] types = node.Value.Types;
        for(int index = 0; index < types.Length; ++index) {
          yield return types[index];
        }

        node = node.Next;
      }

    }

    /// <summary>Called when the assemblies set is queried for the first time</summary>
    private void enlistAssembliesFirstTime() {
      foreach(Assembly assembly in GetAssemblies()) {
        this.assemblyTypes.AddLast(new AssemblyTypes(assembly, assembly.GetTypes()));
      }
    }

    /// <summary>Called to update the assembly list if it has changed</summary>
    private void updateAssemblyList() {
      LinkedListNode<AssemblyTypes> node = this.assemblyTypes.First;

      foreach(Assembly assembly in GetAssemblies()) {

        // If we reached the end of the cache, this automatically becomes a new entry
        if(node == null) {
          this.assemblyTypes.AddLast(new AssemblyTypes(assembly, assembly.GetTypes()));
        } else { // Otherwise, figure out whether the assembly list has changed

          // Try to locate the cached entry for this assembly. If we have to skip entries,
          // we know that an assembly might have been removed from the set.
          LinkedListNode<AssemblyTypes> existingNode = node;
          while(existingNode.Value.Assembly != assembly) {
            existingNode = existingNode.Next;
            if(existingNode == null) {
              break;
            }
          }

          // If this assembly wasn't found in the cache, add it in the same order
          // it was returned by the enumerator.
          if(existingNode == null) {
            this.assemblyTypes.AddBefore(
              node, new AssemblyTypes(assembly, assembly.GetTypes())
            );
          } else if(existingNode != node) { // Cached assemblies were skipped, reorder
            this.assemblyTypes.Remove(existingNode);
            this.assemblyTypes.AddBefore(node, existingNode);
          } else { // Everything as expected
            node = node.Next;
          }
        }

      }

      // Any nodes behind the last checked node contain cached type lists for assemblies
      // that are no longer in the set, so these will be removed.
      while(node != null) {
        LinkedListNode<AssemblyTypes> nextNode = node.Next;
        this.assemblyTypes.Remove(node);
        node = nextNode;
      }
    }

    /// <summary>Obtains a list of any assemblies whose types should be listed</summary>
    /// <returns>A list of any assemblies whose types to list</returns>
    protected abstract IEnumerable<Assembly> GetAssemblies();

    /// <summary>Cached assembly type lists</summary>
    private LinkedList<AssemblyTypes> assemblyTypes;

  }

} // namespace Nuclex.Support.Services
