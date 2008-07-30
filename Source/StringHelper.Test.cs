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
using System.IO;

#if UNITTEST

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Nuclex.Support {

  /// <summary>Unit Test for the string helper class</summary>
  [TestFixture]
  public class StringHelperTest {

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method works identical to the framework's
    ///   implementation of the IndexOfAny() method, only inverted.
    /// </summary>
    [Test]
    public void TestIndexNotOfAny() {
      string positive = "xxxxxOOOOO";
      string negative = "OOOOOxxxxx";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { 'O' }),
        StringHelper.IndexNotOfAny(negative, new char[] { 'O' })
      );
    }

    /// <summary>
    ///   Verifies that the LastIndexNotOfAny() method works identical to the framework's
    ///   implementation of the LastIndexOfAny() method, only inverted.
    /// </summary>
    [Test]
    public void TestLastIndexNotOfAny() {
      string positive = "xxxxxOOOOO";
      string negative = "OOOOOxxxxx";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'x' }),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'x' })
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method works with multiple characters
    /// </summary>
    [Test]
    public void TestMultipleCharIndexNotOfAny() {
      string haystack = "0123456789";

      Assert.AreEqual(
        5, StringHelper.IndexNotOfAny(haystack, new char[] { '4', '3', '2', '1', '0' })
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method works with multiple characters
    /// </summary>
    [Test]
    public void TestMultipleCharLastIndexNotOfAny() {
      string haystack = "0123456789";

      Assert.AreEqual(
        4, StringHelper.LastIndexNotOfAny(haystack, new char[] { '9', '8', '7', '6', '5' })
      );
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
