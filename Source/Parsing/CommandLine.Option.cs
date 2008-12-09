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

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Option that can be specified on an application's command line</summary>
    public struct Option {

      /// <summary>Contains the raw string the command line argument was parsed from</summary>
      public string Raw; // TODO: ToString() instead
/*
      /// <summary>Method used to specify the argument (either '-', '--' or '/')</summary>
      public string Method;
*/
      /// <summary>Name of the command line argument</summary>
      public string Name;
      /// <summary>Value that has been assigned to the command line argument</summary>
      public string Value;
/*
      /// <summary>Method used to assign the value (either '=', ':' or ' ')</summary>
      public string Assignment;
*/
    }

  }

} // namespace Nuclex.Support.Parsing
