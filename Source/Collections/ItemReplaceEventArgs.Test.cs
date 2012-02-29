#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2012 Nuclex Development Labs

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

#if UNITTEST

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the item event argument container</summary>
  [TestFixture]
  public class ItemReplaceEventArgsTest {

    /// <summary>
    ///   Tests whether an integer argument can be stored in the argument container
    /// </summary>
    [Test]
    public void IntegersCanBeCarried() {
      var test = new ItemReplaceEventArgs<int>(12345, 54321);
      Assert.AreEqual(12345, test.OldItem);
      Assert.AreEqual(54321, test.NewItem);
    }

    /// <summary>
    ///   Tests whether a string argument can be stored in the argument container
    /// </summary>
    [Test]
    public void StringsCanBeCarried() {
      var test = new ItemReplaceEventArgs<string>("hello", "world");
      Assert.AreEqual("hello", test.OldItem);
      Assert.AreEqual("world", test.NewItem);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
