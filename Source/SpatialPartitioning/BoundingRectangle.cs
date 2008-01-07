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

using Microsoft.Xna.Framework;

namespace Nuclex.Support.SpatialPartitioning {

  /// <summary>Two-dimensional bounding rectangle</summary>
  internal struct BoundingRectangle {

    /// <summary>Initializes a new two-dimensional bounding rectangle</summary>
    /// <param name="min">Lesser coordinates of the bounding rectangle</param>
    /// <param name="max">Greater coordinates of the bounding rectangle</param>
    public BoundingRectangle(Vector2 min, Vector2 max) {
      this.Min = min;
      this.Max = max;
    }

    /// <summary>
    ///   Builds the smallest bounding rectangle that contains the two
    ///   specified bounding rectangle.
    /// </summary>
    /// <param name="original">One of the bounding rectangles to contain</param>
    /// <param name="additional">One of the bounding rectangles to contain</param>
    /// <returns>The resulting merged bounding rectangle</returns>
    public static BoundingRectangle CreateMerged(
      BoundingRectangle original, BoundingRectangle additional
    ) {
      BoundingRectangle result;
      CreateMerged(ref original, ref additional, out result);
      return result;
    }

    /// <summary>
    ///   Builds the smallest bounding rectangle that contains the two
    ///   specified bounding rectangle.
    /// </summary>
    /// <param name="original">One of the bounding rectangles to contain</param>
    /// <param name="additional">One of the bounding rectangles to contain</param>
    /// <param name="result">The resulting merged bounding rectangle</param>
    public static void CreateMerged(
      ref BoundingRectangle original, ref BoundingRectangle additional,
      out BoundingRectangle result
    ) {
      result = new BoundingRectangle();
      result.Min = Vector2.Min(original.Min, additional.Min);
      result.Max = Vector2.Max(original.Max, additional.Max);
    }

    /// <summary>Coordinates of the lesser side of the bounding rectangle</summary>
    public Vector2 Min;

    /// <summary>Coordinates of the greater side of the bounding rectangle</summary>
    public Vector2 Max;

  }

} // namespace Nuclex.Support.SpatialPartitioning
