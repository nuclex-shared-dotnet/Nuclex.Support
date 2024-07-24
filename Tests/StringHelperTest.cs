#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

using System;
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the string helper class</summary>
  [TestFixture]
  internal class StringHelperTest {

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
    ///   Verifies that the IndexNotOfAny() method works identical to the framework's
    ///   implementation of the IndexOfAny() method, only inverted, using a start index.
    /// </summary>
    [Test]
    public void TestIndexNotOfAnyWithStartIndex() {
      string positive = "OOOOOxxxxxOOOOO";
      string negative = "xxxxxOOOOOxxxxx";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { 'O' }, 5),
        StringHelper.IndexNotOfAny(negative, new char[] { 'O' }, 5)
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
    ///   Verifies that the LastIndexNotOfAny() method works identical to the framework's
    ///   implementation of the LastIndexOfAny() method, only inverted, using a start index.
    /// </summary>
    [Test]
    public void TestLastIndexNotOfAnyWithStartIndex() {
      string positive = "OOOOOxxxxxOOOOO";
      string negative = "xxxxxOOOOOxxxxx";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'x' }, 5),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'x' }, 5)
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method works with multiple characters
    /// </summary>
    [Test]
    public void TestMultipleCharIndexNotOfAny() {
      string positive = "abcde12345";
      string negative = "12345abcde";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { '1', '2', '3', '4', '5' }),
        StringHelper.IndexNotOfAny(negative, new char[] { '1', '2', '3', '4', '5' })
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method works with multiple characters,
    ///   using a start index
    /// </summary>
    [Test]
    public void TestMultipleCharIndexNotOfAnyWithStartIndex() {
      string positive = "12345abcde12345";
      string negative = "abcde12345abcde";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { '1', '2', '3', '4', '5' }, 5),
        StringHelper.IndexNotOfAny(negative, new char[] { '1', '2', '3', '4', '5' }, 5)
      );
    }

    /// <summary>
    ///   Verifies that the LastIndexNotOfAny() method works with multiple characters
    /// </summary>
    [Test]
    public void TestMultipleCharLastIndexNotOfAny() {
      string positive = "abcde12345";
      string negative = "12345abcde";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' })
      );
    }

    /// <summary>
    ///   Verifies that the LastIndexNotOfAny() method works with multiple characters,
    ///   using a start index
    /// </summary>
    [Test]
    public void TestMultipleCharLastIndexNotOfAnyWithStartIndex() {
      string positive = "12345abcde12345";
      string negative = "abcde12345abcde";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }, 5),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' }, 5)
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method fails when only matches are found
    /// </summary>
    [Test]
    public void TestIndexNotOfAnyMatchesOnly() {
      string positive = "1234512345";
      string negative = "abcdeabcde";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }),
        StringHelper.IndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' })
      );
    }

    /// <summary>
    ///   Verifies that the IndexNotOfAny() method fails when only matches are found,
    ///   using a start index
    /// </summary>
    [Test]
    public void TestIndexNotOfAnyMatchesOnlyWithStartIndex() {
      string positive = "abcde1234512345";
      string negative = "12345abcdeabcde";

      Assert.AreEqual(
        positive.IndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }, 5),
        StringHelper.IndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' }, 5)
      );
    }

    /// <summary>
    ///   Verifies that the LastIndexNotOfAny() method fails when only matches are found
    /// </summary>
    [Test]
    public void TestLastIndexNotOfAnyMatchesOnly() {
      string positive = "1234512345";
      string negative = "abcdeabcde";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' })
      );
    }

    /// <summary>
    ///   Verifies that the LastIndexNotOfAny() method fails when only matches are found,
    ///   using a start index
    /// </summary>
    [Test]
    public void TestLastIndexNotOfAnyMatchesOnlyWithStartIndex() {
      string positive = "abcde1234512345";
      string negative = "12345abcdeabcde";

      Assert.AreEqual(
        positive.LastIndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e' }, 5),
        StringHelper.LastIndexNotOfAny(negative, new char[] { 'a', 'b', 'c', 'd', 'e' }, 5)
      );
    }

    // TODO: Also need unit tests for the 'length' argument
    //       to guarantee the methods stop searching at the exact character

  }

} // namespace Nuclex.Support

#endif // UNITTEST
