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

  /// <summary>Base class for unit testing the rectangle packers</summary>
  public abstract class RectanglePackerTest {

    /// <summary>Determines the efficiency of a packer with a packing area of 70x70</summary>
    /// <param name="packer">Packer with a packing area of 70x70 units</param>
    /// <returns>The efficiency factor of the packer</returns>
    /// <remarks>
    ///   A perfect packer would achieve an efficiency rating of 1.0. This is
    ///   impossible however since the 24 squares cannot all be packed into
    ///   the 70x70 square with no overlap (Bitner & Reingold 1975). The closer
    ///   the efficiency rating is to 1.0, the better, with 0.99 being the
    ///   mathematically best rating achievable.
    /// </remarks>
    public float calculateEfficiency(RectanglePacker packer) {
      // If we take a 1x1 square, a 2x2 square, etc. up to a 24x24 square,
      // the sum of the areas of these squares is 4900, which is 70². This
      // is the only nontrivial sum of consecutive squares starting with
      // one which is a perfect square (Watson 1918).
      int areaCovered = 0;

      for(int size = 24; size >= 1; --size) {
        Point placement;

        if(packer.TryPack(size, size, out placement))
          areaCovered += size * size;
      }

      return (float)areaCovered / 4900.0f;
    }

  }

} // namespace Nuclex.Support.Packing
