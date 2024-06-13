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

  /// <summary>Unit Test for the string segment class</summary>
  [TestFixture]
  internal class StringSegmentTest {

    /// <summary>
    ///   Tests whether the default constructor of the StringSegment class throws the
    ///   right exception when being passed 'null' instead of a string
    /// </summary>
    [Test]
    public void SimpleConstructorThrowsWhenStringIsNull() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new StringSegment(null); }
      );
    }

    /// <summary>
    ///   Tests whether the simple constructor of the StringSegment class accepts
    ///   an empty string
    /// </summary>
    [Test]
    public void SimpleConstructorAcceptsEmptyString() {
      new StringSegment(string.Empty);
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed 'null' instead of a string
    /// </summary>
    [Test]
    public void ConstructorThrowsWhenStringIsNull() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new StringSegment(null, 0, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class accepts
    ///   an empty string
    /// </summary>
    [Test]
    public void ConstructorAcceptsEmptyString() {
      new StringSegment(string.Empty, 0, 0);
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed an invalid start offset
    /// </summary>
    [Test]
    public void ConstructorThrowsOnInvalidOffset() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment(string.Empty, -1, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed an invalid string length
    /// </summary>
    [Test]
    public void ConstructorThrowsOnInvalidLength() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment(string.Empty, 0, -1); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed a string length that's too large
    /// </summary>
    [Test]
    public void ConstructorThrowsOnLengthOverrun() {
      Assert.Throws<ArgumentException>(
        delegate() { new StringSegment("hello", 3, 3); }
      );
    }

    /// <summary>Tests whether the 'Text' property works as expected</summary>
    [Test]
    public void TextPropertyStoresOriginalString() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual("hello", testSegment.Text);
    }

    /// <summary>Tests whether the 'Offset' property works as expected</summary>
    [Test]
    public void OffsetPropertyIsStored() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual(1, testSegment.Offset);
    }

    /// <summary>Tests whether the 'Count' property works as expected</summary>
    [Test]
    public void CountPropertyIsStored() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual(3, testSegment.Count);
    }

    /// <summary>
    ///   Tests whether two differing instances produce different hash codes
    /// </summary>
    [Test]
    public void DifferentInstancesHaveDifferentHashCodes_Usually() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.AreNotEqual(
        helloWorldSegment.GetHashCode(), howAreYouSegment.GetHashCode()
      );
    }

    /// <summary>
    ///   Tests whether two equivalent instances produce an identical hash code
    /// </summary>
    [Test]
    public void EquivalentInstancesHaveSameHashcode() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.AreEqual(
        helloWorld1Segment.GetHashCode(), helloWorld2Segment.GetHashCode()
      );
    }

    /// <summary>Tests the equals method performing a comparison against null</summary>
    [Test]
    public void EqualsAgainstNullIsAlwaysFalse() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);

      Assert.IsFalse(
        helloWorldSegment.Equals(null)
      );
    }

    /// <summary>Tests the equality operator with differing instances</summary>
    [Test]
    public void DifferingInstancesAreNotEqual() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.IsFalse(helloWorldSegment == howAreYouSegment);
    }

    /// <summary>Tests the equality operator with equivalent instances</summary>
    [Test]
    public void EquivalentInstancesAreEqual() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.IsTrue(helloWorld1Segment == helloWorld2Segment);
    }

    /// <summary>Tests the inequality operator with differing instances</summary>
    [Test]
    public void DifferingInstancesAreUnequal() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.IsTrue(helloWorldSegment != howAreYouSegment);
    }

    /// <summary>Tests the inequality operator with equivalent instances</summary>
    [Test]
    public void EquivalentInstancesAreNotUnequal() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.IsFalse(helloWorld1Segment != helloWorld2Segment);
    }

    /// <summary>Tests the ToString() method of the string segment</summary>
    [Test]
    public void TestToString() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 4, 3);

      Assert.AreEqual("o w", helloWorldSegment.ToString());
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
