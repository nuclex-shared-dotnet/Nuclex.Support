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

  /// <summary>Packer using a custom algorithm by Markus Ewald</summary>
  public class CygonRectanglePacker : RectanglePacker {

    #region class SliceStartComparer

    /// <summary>Compares the starting position of height slices</summary>
    private class SliceStartComparer : IComparer<Point> {

      /// <summary>Provides a default instance for the anchor rank comparer</summary>
      public static SliceStartComparer Default = new SliceStartComparer();

      /// <summary>Compares the starting position of two height slices</summary>
      /// <param name="left">Left slice start that will be compared</param>
      /// <param name="right">Right slice start that will be compared</param>
      /// <returns>The relation of the two slice starts ranks to each other</returns>
      public int Compare(Point left, Point right) {
        return left.X - right.X;
      }

    }

    #endregion

    /// <summary>Initializes a new rectangle packer</summary>
    /// <param name="maxPackingAreaWidth">Maximum width of the packing area</param>
    /// <param name="maxPackingAreaHeight">Maximum height of the packing area</param>
    public CygonRectanglePacker(int maxPackingAreaWidth, int maxPackingAreaHeight)
      : base(maxPackingAreaWidth, maxPackingAreaHeight) {

      this.heightSlices = new List<Point>();

      // At the beginning, the packing area is a single slice of height 0
      this.heightSlices.Add(new Point(0, 0));

    }

    /// <summary>Tries to allocate space for a rectangle in the packing area</summary>
    /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
    /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
    /// <param name="placement">Output parameter receiving the rectangle's placement</param>
    /// <returns>True if space for the rectangle could be allocated</returns>
    public override bool TryAllocate(
      int rectangleWidth, int rectangleHeight, out Point placement
    ) {
      integrateRectangle(0, 1, 5);
      integrateRectangle(20, 5, 30);
      integrateRectangle(10, 10, 50);
      integrateRectangle(10, 15, 25);
      integrateRectangle(35, 20, 25);
      integrateRectangle(40, 25, 15);

      placement = Point.Zero;
      return false;
    }

    /// <summary>Integrates a new rectangle into the height slice table</summary>
    /// <param name="left">Position of the rectangle's left side</param>
    /// <param name="bottom">Position of the rectangle's lower side</param>
    /// <param name="width">Width of the rectangle</param>
    private void integrateRectangle(int left, int bottom, int width) {

      // Find the first slice that is touched by the rectangle
      int startSlice = this.heightSlices.BinarySearch(
        new Point(left, 0), SliceStartComparer.Default
      );
      int firstSliceOriginalHeight;

      // Did we score a direct hit on an existing slice start?
      if(startSlice >= 0) {

        // We scored a direct hit, so we can replace the slice we have hit
        firstSliceOriginalHeight = this.heightSlices[startSlice].Y;
        this.heightSlices[startSlice] = new Point(left, bottom);
        ++startSlice;

      } else { // No direct hit, slice starts inside another slice

        // Add a new slice after the slice in which we start
        startSlice = ~startSlice;
        firstSliceOriginalHeight = this.heightSlices[startSlice - 1].Y;
        this.heightSlices.Insert(startSlice, new Point(left, bottom));

      }

      // Special case, the rectangle was on the last slice, so we cannot
      // use the start slice + 1 as start index for the binary search
      if(startSlice >= this.heightSlices.Count) {
      } else {
        int right = left + width;

        int endSlice = this.heightSlices.BinarySearch(
          startSlice, this.heightSlices.Count - startSlice,
          new Point(right, 0), SliceStartComparer.Default
        );
      }

      //this.heightSlices.RemoveRange(startSlice, endSlice - startSlice);
/*
      int nextSlice = firstSlice + 1;
      bool isLastSlice = (nextSlice >= this.heightSlices.Count);

*/
/*      


        int nextSlice = firstSlice + 1;
        bool isLastSlice = (nextSlice >= this.heightSlices.Count);

        // 
        bool endsInFirstSlice;
        if(isLastSlice)
          endsInFirstSlice = right < MaxPackingAreaWidth;
        else
          endsInFirstSlice = right < this.heightSlices[nextSlice].X;

        if(endsInFirstSlice) {
          this.heightSlices.Insert(
            nextSlice, new Point(right, this.heightSlices[firstSlice].Y)
          );
          this.heightSlices[firstSlice] = new Point(left, bottom);
          return;
        } else { // Integrated rect continues beyond the discovered slice
          this.heightSlices[firstSlice] = new Point(left, bottom);
        }

      } else {

        firstSlice = ~firstSlice;

        //firstSliceOriginalHeight = this.heightSlices[firstSlice].Y;

        this.heightSlices.Insert(firstSlice, new Point(left, bottom));
        ++firstSlice;

      }
*/
    }

    /// <summary>Stores the height silhouette of the rectangles</summary>
    private List<Point> heightSlices;

  }

} // namespace Nuclex.Support.Packing
