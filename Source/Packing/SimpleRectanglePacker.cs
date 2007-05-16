#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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

using Microsoft.Xna.Framework;

namespace Nuclex.Support.Packing {

  /// <summary>Simplified packer for rectangles which don't vary greatly in size</summary>
  /// <remarks>
  ///   This is a highly performant packer that sacrifices space efficiency for
  ///   low memory usage and runtime performance. It achieves good results with
  ///   near-uniform sized rectangles but will waste lots of space with rectangles
  ///   of varying dimensions.
  /// </remarks>
  public class SimpleRectanglePacker : RectanglePacker {

    /// <summary>Initializes a new rectangle packer</summary>
    /// <param name="maxPackingAreaWidth">Maximum width of the packing area</param>
    /// <param name="maxPackingAreaHeight">Maximum height of the packing area</param>
    public SimpleRectanglePacker(int maxPackingAreaWidth, int maxPackingAreaHeight)
      : base(maxPackingAreaWidth, maxPackingAreaHeight) { }

    /// <summary>Tries to allocate space for a rectangle in the packing area</summary>
    /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
    /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
    /// <param name="placement">Output parameter receiving the rectangle's placement</param>
    /// <returns>True if space for the rectangle could be allocated</returns>
    public override bool TryAllocate(
      int rectangleWidth, int rectangleHeight, out Point placement
    ) {

      // If the rectangle is larger than the packing area in any dimension,
      // it will never fit!
      if(
        (rectangleWidth > MaxPackingAreaWidth) || (rectangleHeight > MaxPackingAreaHeight)
      ) {
        placement = Point.Zero;
        return false;
      }

      // Do we have to start a new line ?
      if(this.column + rectangleWidth > MaxPackingAreaWidth) {
        this.currentLine += this.lineHeight;
        this.lineHeight = 0;
        this.column = 0;
      }

      // If it doesn't fit vertically now, the packing area is considered full
      if(this.currentLine + rectangleHeight > MaxPackingAreaHeight) {
        placement = Point.Zero;
        return false;
      }

      // The rectangle appears to fit at the current location
      placement = new Point(this.column, this.currentLine);

      this.column += rectangleWidth; // Can be larger than cache width till next run
      if(rectangleHeight > this.lineHeight)
        this.lineHeight = rectangleHeight;

      return true;
    }

    /// <summary>Current packing line</summary>
    private int currentLine;
    /// <summary>Height of the current packing line</summary>
    private int lineHeight;
    /// <summary>Current column in the current packing line</summary>
    private int column;

  }

} // namespace Nuclex.Support.Packing
