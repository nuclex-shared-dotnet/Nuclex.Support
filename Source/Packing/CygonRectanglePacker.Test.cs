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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Packing {

  /// <summary>Unit test for the arevalo rectangle packer class</summary>
  [TestFixture]
  public class CygonRectanglePackerTest : RectanglePackerTest {

    /// <summary>Tests the packer's efficiency using a deterministic benchmark</summary>
    [Test]
    public void TestSpaceEfficiency() {
      float efficiency = calculateEfficiency(new CygonRectanglePacker(70, 70));
      
      Assert.GreaterOrEqual(efficiency, 0.75, "Packer achieves 75% efficiency");
    }

  }

} // namespace Nuclex.Support.Packing

#endif // UNITTEST
