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
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Nuclex.Support.Parsing {

  /// <summary>Verifies that the parser helper methods are correct</summary>
  [TestFixture]
  internal class ParserHelperTest {

    /// <summary>Ensures that the SkipSpaces() method can handle null strings</summary>
    [Test]
    public void CanSkipSpacesInNullString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipSpaces((string)null, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipSpaces() method can handle empty strings</summary>
    [Test]
    public void CanSkipSpacesInEmptyString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipSpaces(string.Empty, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipSpaces() method can skip spaces</summary>
    [Test]
    public void SpacesCanBeSkipped() {
      int index = 7;
      ParserHelper.SkipSpaces("   Test   Test   ", ref index);
      Assert.AreEqual(10, index);
    }

    /// <summary>Ensures that the SkipNonSpaces() method can handle null strings</summary>
    [Test]
    public void CanSkipNonSpacesInNullString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipNonSpaces((string)null, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNonSpaces() method can handle empty strings</summary>
    [Test]
    public void CanSkipNonSpacesInEmptyString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipNonSpaces(string.Empty, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNonSpaces() method can skip non-space characters</summary>
    [Test]
    public void NonSpacesCanBeSkipped() {
      int index = 7;
      ParserHelper.SkipNonSpaces("Test   Test   Test", ref index);
      Assert.AreEqual(11, index);
    }

    /// <summary>Ensures that the SkipNumbers() method can handle null strings</summary>
    [Test]
    public void CanSkipNumbersInNullString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipNumericals((string)null, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNumbers() method can handle empty strings</summary>
    [Test]
    public void CanSkipNumbersInEmptyString() {
      int index = 0;
      Assert.DoesNotThrow(
        delegate() { ParserHelper.SkipNumericals(string.Empty, ref index); }
      );
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNumbers() method can skip numbers</summary>
    [Test]
    public void NumbersCanBeSkipped() {
      int index = 6;
      ParserHelper.SkipNumericals("123abc456def789", ref index);
      Assert.AreEqual(9, index);
    }

    /// <summary>Ensures that the SkipIntegers() method can handle null strings</summary>
    [Test]
    public void CanSkipIntegersInNullString() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipInteger((string)null, ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNumbers() method can handle empty strings</summary>
    [Test]
    public void CanSkipIntegersInEmptyString() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipInteger(string.Empty, ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Verifies that a prefix alone can not be skipped as an integer</summary>
    [Test]
    public void PrefixAloneIsNotAnInteger() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipInteger("+Test", ref index));
      Assert.AreEqual(0, index);
      Assert.IsFalse(ParserHelper.SkipInteger("-", ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Verifies that a prefixed integer can be skipped</summary>
    [Test]
    public void PrefixedIntegersCanBeSkipped() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipInteger("+123", ref index));
      Assert.AreEqual(4, index);
    }

    /// <summary>Verifies that an integer without a prefix can be skipped</summary>
    [Test]
    public void PlainIntegersCanBeSkipped() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipInteger("12345", ref index));
      Assert.AreEqual(5, index);
    }

    /// <summary>
    ///   Verifies that trying to skip text as if it was an integer skips nothing
    /// </summary>
    [Test]
    public void SkippingTextAsIntegerReturnsFalse() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipInteger("hello", ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipIntegers() method can handle null strings</summary>
    [Test]
    public void CanSkipStringInNullString() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipString((string)null, ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Ensures that the SkipNumbers() method can handle empty strings</summary>
    [Test]
    public void CanSkipStringInEmptyString() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipString(string.Empty, ref index));
      Assert.AreEqual(0, index);
    }

    /// <summary>Verifies that a string consisting of a single word can be skipped</summary>
    [Test]
    public void SingleWordStringsCanBeSkipped() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipString("hello", ref index));
      Assert.AreEqual(5, index);
    }

    /// <summary>
    ///   Verifies that a space character is not skipped over when skipping a string
    /// </summary>
    [Test]
    public void SpaceTerminatesUnquotedStrings() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipString("hello world", ref index));
      Assert.AreEqual(5, index);
    }

    /// <summary>Verifies that a string in quotes continues until the closing quote</summary>
    [Test]
    public void QuotedStringsCanBeSkipped() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipString("\"This is a test\"", ref index));
      Assert.AreEqual(16, index);
    }

    /// <summary>Verifies that a string in quotes continues until the closing quote</summary>
    [Test]
    public void QuotedStringsStopAtClosingQuote() {
      int index = 0;
      Assert.IsTrue(ParserHelper.SkipString("\"This is a test\" but this not.", ref index));
      Assert.AreEqual(16, index);
    }

    /// <summary>Verifies that a string in quotes continues until the closing quote</summary>
    [Test]
    public void QuotedStringRequiresClosingQuote() {
      int index = 0;
      Assert.IsFalse(ParserHelper.SkipString("\"This is missing the closing quote", ref index));
      Assert.AreEqual(0, index);
    }

  }

} // namespace Nuclex.Support.Parsing
