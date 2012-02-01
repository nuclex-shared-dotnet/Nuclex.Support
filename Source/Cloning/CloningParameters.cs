#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

namespace Nuclex.Support.Cloning {

  /// <summary>Parameters of a cloning operation</summary>
  public struct CloningParameters {

    /// <summary>Initializes new cloning parameters</summary>
    /// <param name="type">Type that will be cloned</param>
    /// <param name="deep">Whether to perform a deep clone</param>
    public CloningParameters(Type type, bool deep) {
      this.Type = type;
      this.Deep = deep;
    }

    /// <summary>Returns a string description of the instance</summary>
    /// <returns>The instance's string description</returns>
    public override string ToString() {
      if(this.Deep) {
        return "Deep clone of " + this.Type.ToString();
      } else {
        return "Shallow clone of " + this.Type.ToString();
      }
    }

    /// <summary>
    ///   Returns a hash code that is identical for instances with identical state
    /// </summary>
    /// <returns>The instance's hash code</returns>
    public override int GetHashCode() {
      if(this.Deep) {
        return this.Type.GetHashCode();
      } else {
        return ~this.Type.GetHashCode();
      }
    }

    /// <summary>Type that will be cloned</summary>
    public Type Type;
    /// <summary>Whether a deep clone will be performed</summary>
    public bool Deep;

  }

} // namespace Nuclex.Support.Cloning
