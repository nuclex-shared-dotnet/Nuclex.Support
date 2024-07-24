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

#if UNITTEST

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the Priority/Item pair class</summary>
  [TestFixture]
  internal class PriorityItemPairTest {

    #region class ToStringNullReturner

    /// <summary>Test class in which ToString() can return null</summary>
    private class ToStringNullReturner {

      /// <summary>
      ///   Returns a System.String that represents the current System.Object
      /// </summary>
      /// <returns>A System.String that represents the current System.Object</returns>
      public override string ToString() { return null; }

    }

    #endregion // class ToStringNullReturner

    /// <summary>Tests whether the pair's default constructor works</summary>
    [Test]
    public void TestDefaultConstructor() {
      new PriorityItemPair<int, string>();
    }

    /// <summary>Tests whether the priority can be retrieved from the pair</summary>
    [Test]
    public void TestPriorityRetrieval() {
      PriorityItemPair<int, string> testPair = new PriorityItemPair<int, string>(
        12345, "hello world"
      );

      Assert.AreEqual(12345, testPair.Priority);
    }

    /// <summary>Tests whether the item can be retrieved from the pair</summary>
    [Test]
    public void TestItemRetrieval() {
      PriorityItemPair<int, string> testPair = new PriorityItemPair<int, string>(
        12345, "hello world"
      );

      Assert.AreEqual("hello world", testPair.Item);
    }

    /// <summary>Tests whether the ToString() methods works with valid strings</summary>
    [Test]
    public void TestToStringWithValidStrings() {
      PriorityItemPair<string, string> testPair = new PriorityItemPair<string, string>(
        "hello", "world"
      );

      Assert.AreEqual("[hello, world]", testPair.ToString());
    }

    /// <summary>Tests whether the ToString() methods works with null strings</summary>
    [Test]
    public void TestToStringWithNullStrings() {
      PriorityItemPair<ToStringNullReturner, ToStringNullReturner> testPair =
        new PriorityItemPair<ToStringNullReturner, ToStringNullReturner>(
          new ToStringNullReturner(), new ToStringNullReturner()
        );

      Assert.AreEqual("[, ]", testPair.ToString());
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
