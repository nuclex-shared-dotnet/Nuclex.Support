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
using System.Text;

#if UNITTEST

using NUnit.Framework;

#if ENABLE_BROKEN_COMMAND_LINE_PARSER

namespace Nuclex.Support.Parsing {

  /// <summary>Ensures that the command line parser is working properly</summary>
  [TestFixture]
  public class CommandLineTest {

    #region class OptionTest

    /// <summary>Unit test for the command line option class</summary>
    [TestFixture]
    public class OptionTest {

      /// <summary>
      ///   Verifies that the name of a command line option without a value can be extracted
      /// </summary>
      [Test]
      public void TestNameExtraction() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("--test"), 2, 4
        );

        Assert.AreEqual("--test", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.IsNull(option.Associator);
        Assert.IsNull(option.Value);
      }

      /// <summary>
      ///   Verifies that the name of a command line option without a value can be extracted
      ///   when the option is contained in a substring of a larger string
      /// </summary>
      [Test]
      public void TestNameExtractionFromSubstring() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("||--test||", 2, 6), 4, 4
        );

        Assert.AreEqual("--test", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.IsNull(option.Associator);
        Assert.IsNull(option.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line option can be extracted
      /// </summary>
      [Test]
      public void TestValueExtraction() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("--test=123"), 2, 4, 7, 3
        );

        Assert.AreEqual("--test=123", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.AreEqual("123", option.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line option can be extracted
      ///   when the option is contained in a substring of a larger string
      /// </summary>
      [Test]
      public void TestValueExtractionFromSubstring() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("||--test=123||", 2, 10), 4, 4, 9, 3
        );

        Assert.AreEqual("--test=123", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.AreEqual("123", option.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line option can be extracted
      ///   when the option is assigned a quoted value
      /// </summary>
      [Test]
      public void TestQuotedValueExtraction() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("--test=\"123\"", 0, 12), 2, 4, 8, 3
        );

        Assert.AreEqual("--test=\"123\"", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.AreEqual("123", option.Value);
      }

      /// <summary>
      ///   Varifies that the associator of a command line option with an open ended value
      ///   assignment can be retrieved
      /// </summary>
      [Test]
      public void TestValuelessAssociatorRetrieval() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("--test="), 2, 4
        );

        Assert.AreEqual("--test=", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.IsNull(option.Value);
      }

      /// <summary>
      ///   Varifies that the associator of a command line option with an open ended value
      ///   assignment can be retrieved when the option is contained in a substring of
      ///   a larger string
      /// </summary>
      [Test]
      public void TestValuelessAssociatorRetrievalFromSubstring() {
        CommandLine.Option option = new CommandLine.Option(
          new StringSegment("||--test=||", 2, 7), 4, 4//, 9, -1
        );

        Assert.AreEqual("--test=", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.IsNull(option.Value);
      }

    }

    #endregion // class OptionTest

    /// <summary>
    ///   Validates that the parser can handle an argument initiator without an obvious name
    /// </summary>
    [Test]
    public void TestParseAmbiguousNameResolution() {
      CommandLine commandLine = CommandLine.Parse("--:test");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(1, commandLine.Options.Count);
      Assert.AreEqual("-", commandLine.Options[0].Name);
      Assert.AreEqual("test", commandLine.Options[0].Value);
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseArgumentInitiatorAtEnd() {
      CommandLine commandLine = CommandLine.Parse("-hello:-world -");

      Assert.AreEqual(1, commandLine.Values.Count);
      Assert.AreEqual(1, commandLine.Options.Count);
      Assert.AreEqual("hello", commandLine.Options[0].Name);
      Assert.AreEqual("-world", commandLine.Options[0].Value);
      Assert.AreEqual("-", commandLine.Values[0]);
    }

    /// <summary>Validates that quoted arguments can be parsed</summary>
    [Test]
    public void TestParseQuotedOption() {
      CommandLine commandLine = CommandLine.Parse("hello -world --this -is=\"a test\"");

      Assert.AreEqual(1, commandLine.Values.Count);
      Assert.AreEqual(3, commandLine.Options.Count);
      Assert.AreEqual("hello", commandLine.Values[0]);
      Assert.AreEqual("world", commandLine.Options[0].Name);
      Assert.AreEqual("this", commandLine.Options[1].Name);
      Assert.AreEqual("is", commandLine.Options[2].Name);
      Assert.AreEqual("a test", commandLine.Options[2].Value);
    }

    /// <summary>Validates that null can be parsed</summary>
    [Test]
    public void TestParseNull() {
      CommandLine commandLine = CommandLine.Parse(null);

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(0, commandLine.Options.Count);
    }

    /// <summary>Validates that a single argument without quotes can be parsed</summary>
    [Test]
    public void TestParseSingleNakedValue() {
      CommandLine commandLine = CommandLine.Parse("hello");

      Assert.AreEqual(1, commandLine.Values.Count);
      Assert.AreEqual(0, commandLine.Options.Count);
      Assert.AreEqual("hello", commandLine.Values[0]);
    }

    /// <summary>
    ///   Validates that the parser can handle a quoted argument that's missing
    ///   the closing quote
    /// </summary>
    [Test]
    public void TestParseQuotedArgumentWithoutClosingQuote() {
      CommandLine commandLine = CommandLine.Parse("\"Quoted argument");

      Assert.AreEqual(1, commandLine.Values.Count);
      Assert.AreEqual(0, commandLine.Options.Count);
      Assert.AreEqual("Quoted argument", commandLine.Values[0]);
    }

    /// <summary>
    ///   Validates that the parser can handle an command line consisting of only spaces
    /// </summary>
    [Test]
    public void TestParseSpacesOnly() {
      CommandLine commandLine = CommandLine.Parse(" \t ");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(0, commandLine.Options.Count);
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseMultipleLoneArgumentInitiators() {
      CommandLine commandLine = CommandLine.Parse("--- --");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(2, commandLine.Options.Count);
      Assert.AreEqual("-", commandLine.Options[1].Name);
      Assert.AreEqual("-", commandLine.Options[2].Name);
    }

    /// <summary>
    ///   Verifies that the parser correctly handles options with embedded option initiators
    /// </summary>
    [Test]
    public void TestParseOptionWithEmbeddedInitiator() {
      CommandLine commandLine = CommandLine.Parse("-hello/world=123 -test-case");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(2, commandLine.Options.Count);
      Assert.AreEqual("hello/world", commandLine.Options[0].Name);
      Assert.AreEqual("test-case", commandLine.Options[1].Name);
    }

    /// <summary>
    ///   Validates that arguments and values without spaces inbetween can be parsed
    /// </summary>
    [Test]
    public void TestParseOptionAndValueWithoutSpaces() {
      CommandLine commandLine = CommandLine.Parse("\"value\"-option\"value\"");

      Assert.AreEqual(2, commandLine.Values.Count);
      Assert.AreEqual(1, commandLine.Options.Count);
      Assert.AreEqual("value", commandLine.Values[0]);
      Assert.AreEqual("option", commandLine.Options[0].Name);
      Assert.AreEqual("value", commandLine.Values[1]);
    }

    /// <summary>
    ///   Validates that options with modifiers at the end of the command line
    ///   are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionWithModifierAtEnd() {
      CommandLine commandLine = CommandLine.Parse("--test-value- -test+");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(2, commandLine.Options.Count);
      Assert.AreEqual("test-value", commandLine.Options[0].Name);
      Assert.AreEqual("test", commandLine.Options[1].Name);
    }

    /// <summary>
    ///   Validates that options with values assigned to them are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionWithAssignment() {
      CommandLine commandLine = CommandLine.Parse("-hello:123 -world=321");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(2, commandLine.Options.Count);
      Assert.AreEqual("hello", commandLine.Options[0].Name);
      Assert.AreEqual("123", commandLine.Options[0].Value);
      Assert.AreEqual("world", commandLine.Options[1].Name);
      Assert.AreEqual("321", commandLine.Options[1].Value);
    }

    /// <summary>
    ///   Validates that options with an empty value at the end of the command line
    ///   string are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionAtEndOfString() {
      CommandLine commandLine = CommandLine.Parse("--test:");

      Assert.AreEqual(0, commandLine.Values.Count);
      Assert.AreEqual(1, commandLine.Options.Count);
      Assert.AreEqual("test", commandLine.Options[0].Name);
    }

    /// <summary>
    ///   Verifies that the parser can recognize windows command line options if
    ///   configured to windows mode
    /// </summary>
    [Test]
    public void TestWindowsOptionInitiator() {
      CommandLine commandLine = CommandLine.Parse("/hello //world", true);

      Assert.AreEqual(1, commandLine.Values.Count);
      Assert.AreEqual(2, commandLine.Options.Count);
      Assert.AreEqual("hello", commandLine.Options[0].Name);
      Assert.AreEqual("/", commandLine.Options[0].Value);
      Assert.AreEqual("world", commandLine.Options[1].Name);
    }

    /// <summary>
    ///   Verifies that the parser ignores windows command line options if
    ///   configured to non-windows mode
    /// </summary>
    [Test]
    public void TestNonWindowsOptionValues() {
      CommandLine commandLine = CommandLine.Parse("/hello //world", false);

      Assert.AreEqual(2, commandLine.Values.Count);
      Assert.AreEqual(0, commandLine.Options.Count);
      Assert.AreEqual("/hello", commandLine.Values[0]);
      Assert.AreEqual("//world", commandLine.Values[1]);
    }

  }

} // namespace Nuclex.Support.Parsing

#endif // ENABLE_BROKEN_COMMAND_LINE_PARSER

#endif // UNITTEST