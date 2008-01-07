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
using System.Diagnostics;

namespace Nuclex.Support.SpatialPartitioning {

  /// <summary>Node in a two-dimensional R-Tree</summary>
  /// <typeparam name="ItemType">Type of items that the R-Tree manages</typeparam>
  internal class RTreeNode2<ItemType> {

    /// <summary>Initializes a new R-Tree node</summary>
    /// <param name="capacity">Number of items that can fit in the node</param>
    public RTreeNode2(int capacity) {
      this.leafs = new RTreeLeaf2<ItemType>[capacity];
      this.leafCount = 0;
    }

    /// <summary>Inserts an item into this node</summary>
    /// <param name="item">Item to be inserted</param>
    /// <param name="boundingRectangle">Bounding rectangle of the item</param>
    private void insertEntry(ItemType item, BoundingRectangle boundingRectangle) {
      Debug.Assert(leafCount < this.leafs.Length);

      this.leafs[this.leafCount].Item = item;
      this.leafs[this.leafCount].BoundingRectangle = boundingRectangle;

      BoundingRectangle.CreateMerged(
        ref this.boundingRectangle, ref boundingRectangle,
        out this.boundingRectangle
      );
    }

    /// <summary>The node's minimum bounding rectangle</summary>
    /// <remarks>
    ///   This bounding rectangle is just large enough to contain all the items
    ///   belonging to this node and recursively all of its child nodes.
    /// </remarks>
    private BoundingRectangle boundingRectangle;

    /// <summary>Leafs of this node</summary>
    private RTreeLeaf2<ItemType>[] leafs;
    /// <summary>Number of leafes in use</summary>
    private int leafCount;

  }

} // namespace Nuclex.Support.SpatialPartitioning
