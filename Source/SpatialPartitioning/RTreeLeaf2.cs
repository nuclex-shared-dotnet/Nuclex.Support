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

namespace Nuclex.Support.SpatialPartitioning {

  /// <summary>Leaf of an R-Tree</summary>
  internal class RTreeLeaf2<ItemType> {

    public BoundingRectangle BoundingRectangle;
    public ItemType Item;


  }

} // namespace Nuclex.Support.SpatialPartitioning
