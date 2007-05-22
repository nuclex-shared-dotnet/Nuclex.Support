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

  /// <summary>Packer using a custom algorithm by Markus 'Cygon' Ewald</summary>
  /// <remarks>
  ///   <para>
  ///     Algorithm conceived by Markus Ewald (cygon at nuclex dot org), thought
  ///     I'm quite sure I'm not the first one to invent this algorithm :)
  ///   </para>
  ///   <para>
  ///     This algorithm always places rectangles as low as possible. So, for any
  ///     new rectangle that is to be added into the packing area, the packer has
  ///     to determine the X coordinate at which the rectangle has the lowest height.
  ///     To quickly discover these locations, the packer keeps a dynamically updated
  ///     list of "height slices" which store the silhouette of the rectangles that
  ///     have been placed so far.
  ///   </para>
  /// </remarks>
  public class CygonRectanglePacker : RectanglePacker {

    /// <summary>By how much the wasted area influences a placement's score</summary>
    /// <remarks>
    ///   <para>
    ///     The score of a potential position for a new rectangle is how far the
    ///     rectangle is from the lower end of the packing area. The lower the
    ///     score, the better the position.
    ///   </para>
    ///   <para>
    ///     Often, however, it's better to choose a position that's farther from
    ///     the lower end of the packing area to not block a gap in which a future
    ///     rectangle might still fit in. To account for this fact, the packer
    ///     calculates a "wasted area factor", which is the amount of area that
    ///     would forever go to waste in relation of the area the rectangle has.
    ///   </para>
    ///   <para>
    ///     This value controls the weighting of this wasted area factor in the
    ///     potential position's score. Finding a value that works well for typical
    ///     packing problems is a matter of trial and error, as it seems :)
    ///   </para>
    /// </remarks>
    private const int WastedAreaScoreWeight = 3;

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
    /// <param name="packingAreaWidth">Maximum width of the packing area</param>
    /// <param name="packingAreaHeight">Maximum height of the packing area</param>
    public CygonRectanglePacker(int packingAreaWidth, int packingAreaHeight)
      : base(packingAreaWidth, packingAreaHeight) {

      this.heightSlices = new List<Point>();

      // At the beginning, the packing area is a single slice of height 0
      this.heightSlices.Add(new Point(0, 0));

    }

    /// <summary>Tries to allocate space for a rectangle in the packing area</summary>
    /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
    /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
    /// <param name="placement">Output parameter receiving the rectangle's placement</param>
    /// <returns>True if space for the rectangle could be allocated</returns>
    public override bool TryPack(
      int rectangleWidth, int rectangleHeight, out Point placement
    ) {
      // If the rectangle is larger than the packing area in any dimension,
      // it will never fit!
      if(
        (rectangleWidth > PackingAreaWidth) || (rectangleHeight > PackingAreaHeight)
      ) {
        placement = Point.Zero;
        return false;
      }

      bool fits = findBestPosition(rectangleWidth, rectangleHeight, out placement);
      if(fits)
        integrateRectangle(placement.X, rectangleWidth, placement.Y + rectangleHeight);

      return fits;
    }

    /// <summary>Finds the best position for a rectangle of the given width</summary>
    /// <param name="rectangleWidth">Width of the rectangle to find a position for</param>
    /// <param name="rectangleHeight">Height of the rectangle to find a position for</param>
    /// <returns>The best position for a rectangle with the specified width</returns>
    private bool findBestPosition(
      int rectangleWidth, int rectangleHeight, out Point placement
    ) {

      // Total surface area of the rectangle
      int rectangleArea = rectangleWidth * rectangleHeight;

      // Slice index, vertical position and score of the best placement we could find
      int bestSliceIndex = -1; // Slice index where the best placement was found
      int bestSliceY = 0; // Y position of the best placement found
      int bestScore = PackingAreaWidth * PackingAreaHeight; // lower == better!

      // This is the counter for the currently checked position. The search works by
      // skipping from slice to slice, determining the suitability of the location for the
      // placement of the rectangle.
      int leftSliceIndex = 0;

      // Determine the slice in which the right end of the rectangle is located
      int rightSliceIndex = this.heightSlices.BinarySearch(
        new Point(rectangleWidth, 0), SliceStartComparer.Default
      );
      if(rightSliceIndex < 0)
        rightSliceIndex = ~rightSliceIndex;

      while(rightSliceIndex <= this.heightSlices.Count) {

        // Determine the highest slice within the slices covered by the rectangle at
        // its current placement. We cannot put the rectangle any lower than this without
        // overlapping the other rectangles.
        int highest = this.heightSlices[leftSliceIndex].Y;
        for(int index = leftSliceIndex + 1; index < rightSliceIndex; ++index)
          if(this.heightSlices[index].Y > highest)
            highest = this.heightSlices[index].Y;

        if((highest + rectangleHeight < PackingAreaHeight)) {
          int score = highest;

          // WASTED AREA CALCULATION --------------------------------------------------

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

          // Limit wasted area to the area taken up by the rectangle. This prevents
          // a "build-up" of wasted area that become more expensive the higher the packing
          // area is filled.
          wastedArea = Math.Min(wastedArea, rectangleArea);

          // Alter the score by the amount of wasted area in relation to
          score += (wastedArea * WastedAreaScoreWeight / rectangleArea);

          // WASTED AREA CALCULATION --------------------------------------------------

          if(score < bestScore) {
            bestSliceIndex = leftSliceIndex;
            bestSliceY = highest;
            bestScore = score;
          }
        }

        // Advance the starting slice to the next slice start
        ++leftSliceIndex;
        if(leftSliceIndex >= this.heightSlices.Count)
          break;

        // Advance the ending slice until we're on the right slice again, given the new
        // starting position of the rectangle.
        int rightRectangleEnd = this.heightSlices[leftSliceIndex].X + rectangleWidth;
        for(; rightSliceIndex <= this.heightSlices.Count; ++rightSliceIndex) {
          int rightSliceStart;
          if(rightSliceIndex == this.heightSlices.Count)
            rightSliceStart = PackingAreaWidth;
          else
            rightSliceStart = this.heightSlices[rightSliceIndex].X;

          // Is this the slice we're looking for?
          if(rightSliceStart > rightRectangleEnd)
            break;
        }

        // If we crossed the end of the slice array, the rectangle's right end has left
        // the packing area, and thus, our search ends.
        if(rightSliceIndex > this.heightSlices.Count)
          break;

      }

      // Return the index of the best slice we found for this rectangle. If the rectangle
      // didn't fit, this variable will still have its initialization value of -1.
      if(bestSliceIndex == -1) {
        placement = Point.Zero;
        return false;
      } else {
        placement = new Point(this.heightSlices[bestSliceIndex].X, bestSliceY);
        return true;
      }

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
        // exact same width the packing area has), add another slice to return to
        // the original height at the end of the rectangle.
        if(right < PackingAreaWidth)
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

          // Remove all slices covered by the rectangle and begin a new slice at its end
          // to return back to the height of the slice on which the rectangle ends.
          this.heightSlices.RemoveRange(startSlice, endSlice - startSlice);
          if(right < PackingAreaWidth)
            this.heightSlices.Insert(startSlice, new Point(right, returnHeight));

        }

      }

    }

    /// <summary>Stores the height silhouette of the rectangles</summary>
    private List<Point> heightSlices;

  }

} // namespace Nuclex.Support.Packing
