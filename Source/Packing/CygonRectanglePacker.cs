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
  /// <remarks>
  ///   This algorithms always places as close to the top as possible. So, for any new
  ///   rectangle, the packer has to determine a X coordinate at which the rectangle
  ///   can be placed at the highest point. To quickly discover these locations,
  ///   the packer keeps a dynamically list of "height slices", which store the
  ///   baseline of the rectangles that have been placed so far.
  /// </remarks>
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
      int sliceIndex = findBestPosition(rectangleWidth, rectangleHeight);

      // TODO: Rectangle might not even fit there!
      if(sliceIndex == -1) {

        placement = Point.Zero;
        return false;

      } else {

        placement = this.heightSlices[sliceIndex];

        integrateRectangle(
          this.heightSlices[sliceIndex].X,
          rectangleWidth,
          this.heightSlices[sliceIndex].Y + rectangleHeight
        );

        return true;

      }
    }

    /// <summary>Finds the best position for a rectangle of the given width</summary>
    /// <param name="rectangleWidth">Width of the rectangle to find a position for</param>
    /// <param name="rectangleHeight">Height of the rectangle to find a position for</param>
    /// <returns>The best position for a rectangle with the specified width</returns>
    private int findBestPosition(int rectangleWidth, int rectangleHeight) {
      int leftSliceIndex = 0;
      int rightSliceIndex = this.heightSlices.BinarySearch(
        new Point(rectangleWidth, 0), SliceStartComparer.Default
      );
      if(rightSliceIndex < 0)
        rightSliceIndex = ~rightSliceIndex;

      int leastWastedSliceIndex = -1;
      int leastWastedArea = MaxPackingAreaWidth * MaxPackingAreaHeight;

      while(rightSliceIndex <= this.heightSlices.Count) {
        // final time (this is the special case where the rectangle is attempted
        // to be placed at the rightmost end of the packing area)
      
        /*
                // Determine the highest slice at this position. We cannot put the rectangle
                // any lower than this without colliding into other rectangles
                int highest = this.heightSlices[leftSliceIndex].Y;
                for(int index = leftSliceIndex + 1; index < rightSliceIndex; ++index)
                  highest = Math.Max(highest, this.heightSlices[index].Y);

                // Calculate the amount of space that would go to waste if the rectangle
                // would be placed at this location
                int wastedArea = 0;
                for(int index = leftSliceIndex; index < rightSliceIndex - 1; ++index) {
                  int sliceWidth = this.heightSlices[index + 1].X - this.heightSlices[index].X;
                  wastedArea += (highest - this.heightSlices[index].Y) * sliceWidth;
                }
                wastedArea +=
                  (highest - this.heightSlices[rightSliceIndex - 1].Y) *
                  (
                    (this.heightSlices[leftSliceIndex].X + rectangleWidth) -
                    this.heightSlices[rightSliceIndex - 1].X
                  );

                // If this beats the current record for the least wasted area, remember this as
                // being the best position found so far
                if(
                  (wastedArea < leastWastedArea) &&
                  (this.heightSlices[leftSliceIndex].Y + rectangleHeight < MaxPackingAreaHeight)
                ) {
                  leastWastedArea = wastedArea;
                  leastWastedSliceIndex = leftSliceIndex;

                  // No sense looking any further if we found the perfect place!
                  if(leastWastedArea == 0)
                    break;
                }
        */


        // If this already was the loop after the final slice, terminate it now!
        if(rightSliceIndex == this.heightSlices.Count)
          break;

        // Advance the starting slice to the next slice start
        ++leftSliceIndex;
        int rightEnd = this.heightSlices[leftSliceIndex].X + rectangleWidth;

        // Advance the ending slice to where the rectangle ends now
        while(rightEnd > this.heightSlices[rightSliceIndex].X) {
          ++rightSliceIndex;

          // If the end is reached, stop shifting and make the outer loop run one final time
          if(rightSliceIndex == this.heightSlices.Count)
            break;
        }
      }

      return leastWastedSliceIndex;
    }

    /// <summary>Integrates a new rectangle into the height slice table</summary>
    /// <param name="left">Position of the rectangle's left side</param>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="bottom">Position of the rectangle's lower side</param>
    private void integrateRectangle(int left, int width, int bottom) {

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

      } else { // No direct hit, slice starts inside another slice

        // Add a new slice after the slice in which we start
        startSlice = ~startSlice;
        firstSliceOriginalHeight = this.heightSlices[startSlice - 1].Y;
        this.heightSlices.Insert(startSlice, new Point(left, bottom));

      }

      int right = left + width;
      ++startSlice;

      // Special case, the rectangle started on the last slice, so we cannot
      // use the start slice + 1 for the binary search and the possibly already
      // modified start slice height now only remains in our temporary
      // firstSliceOriginalHeight variable
      if(startSlice >= this.heightSlices.Count) {

        // If the slice ends within the last slice (usual case, unless it has the
        // exact same with the packing area has), add another slice to return to the
        // original height at the end of the rectangle.
        if(right < MaxPackingAreaWidth)
          this.heightSlices.Add(new Point(right, firstSliceOriginalHeight));

      } else { // The rectangle doesn't start on the last slice

        int endSlice = this.heightSlices.BinarySearch(
          startSlice, this.heightSlices.Count - startSlice,
          new Point(right, 0), SliceStartComparer.Default
        );

        // Another direct hit on the final slice's end?
        if(endSlice > 0) {

          this.heightSlices.RemoveRange(startSlice, endSlice - startSlice);

        } else { // No direct hit, rectangle ends inside another slice

          endSlice = ~endSlice;

          // Find out to which height we need to return at the right end of
          // the rectangle
          int returnHeight;
          if(endSlice == startSlice)
            returnHeight = firstSliceOriginalHeight;
          else
            returnHeight = this.heightSlices[endSlice - 1].Y;

          // Remove all slices covered by the rectangle and began a new slice at
          // its end to return to the height the slice in which the rectangle ends
          // has had.
          this.heightSlices.RemoveRange(startSlice, endSlice - startSlice);
          if(right < MaxPackingAreaWidth)
            this.heightSlices.Insert(startSlice, new Point(right, returnHeight));

        }

      }

    }

    /// <summary>Stores the height silhouette of the rectangles</summary>
    private List<Point> heightSlices;

  }

} // namespace Nuclex.Support.Packing
