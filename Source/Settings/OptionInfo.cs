#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2014 Nuclex Development Labs

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

namespace Nuclex.Support.Settings {

  /// <summary>Informations about an option stored in a settings container</summary>
  public struct OptionInfo {

    /// <summary>Name of the option</summary>
    public string Name;
    /// <summary>Data type of the option</summary>
    public Type OptionType;

  }

} // namespace Nuclex.Support.Settings
