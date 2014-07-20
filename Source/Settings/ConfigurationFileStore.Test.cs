#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2014 Nuclex Development Labs

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
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support.Settings {

  /// <summary>Unit tests for the configuration file store</summary>
  [TestFixture]
  internal class ConfigurationFileStoreTest {

    /// <summary>Loads a configuration file from a string</summary>
    /// <param name="fileContents">Contents of the configuration file</param>
    /// <returns>The configuration file loaded from the string</returns>
    private static ConfigurationFileStore load(string fileContents) {
      using(var reader = new StringReader(fileContents)) {
        return ConfigurationFileStore.Parse(reader);
      }
    }

    /// <summary>
    ///   Verifies that loading an empty file doesn't lead to an exception
    /// </summary>
    [Test]
    public void CanParseEmptyFile() {
      Assert.That(() => load(string.Empty), Throws.Nothing);
    }

    /// <summary>
    ///   Verifies that categories can be parsed from a configuration file
    /// </summary>
    [Test]
    public void CanParseCategories() {
      string[] categoryNames = new string[] { "Category1", "Category 2" };
      string fileContents =
        "[" + categoryNames[0] + "]\r\n" +
        "  [  " + categoryNames[1] + "  ]  \r\n";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(configurationFile.EnumerateCategories(), Is.EquivalentTo(categoryNames));
    }

    /// <summary>
    ///   Verifies that malformed categories can be handled by the parser
    /// </summary>
    [Test]
    public void MalformedCategoriesAreIgnored() {
      string fileContents =
        "[ Not a category\r\n" +
        "  [";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(configurationFile.EnumerateCategories(), Is.Empty);
    }

    /// <summary>
    ///   Verifies that empty lines in the configuration file have no meaning
    /// </summary>
    [Test]
    public void EmptyLinesAreSkipped() {
      string fileContents =
        "\r\n" +
        "  ";
      ConfigurationFileStore configurationFile = load(fileContents);
      Assert.That(configurationFile.EnumerateCategories(), Is.Empty);
    }

    /// <summary>
    ///   Verifies that category definitions after a comment sign are ignored
    /// </summary>
    [Test]
    public void CommentedOutCategoriesAreIgnored() {
      string fileContents =
        "#[NotACategory]\r\n" +
        "; [ Also Not A Category ]\r\n";
      ConfigurationFileStore configurationFile = load(fileContents);
      Assert.That(configurationFile.EnumerateCategories(), Is.Empty);
    }

    /// <summary>
    ///   Verifies that assignments without an option name are ignored by the parser
    /// </summary>
    [Test]
    public void NamelessAssignmentsAreIgnored() {
      string fileContents =
        "=\r\n" +
        " = \r\n" +
        " = hello";
      ConfigurationFileStore configurationFile = load(fileContents);
      Assert.That(configurationFile.EnumerateCategories(), Is.Empty);
      Assert.That(configurationFile.EnumerateOptions(), Is.Empty);
    }

    /// <summary>
    ///   Verifies that assignments without an option name are ignored by the parser
    /// </summary>
    [Test]
    public void OptionsCanHaveEmptyValues() {
      string fileContents =
        "a =\r\n" +
        "b =   \r\n" +
        "c =       ; hello";
      ConfigurationFileStore configurationFile = load(fileContents);
      Assert.That(configurationFile.EnumerateCategories(), Is.Empty);

      var options = new List<OptionInfo>(configurationFile.EnumerateOptions());
      Assert.That(options.Count, Is.EqualTo(3));

      for(int index = 0; index < options.Count; ++index) {
        Assert.That(
          configurationFile.Get<string>(null, options[index].Name), Is.Null.Or.Empty
        );
      }
    }

    /// <summary>
    ///   Verifies that values assigned to options can contain space charcters
    /// </summary>
    [Test]
    public void OptionValuesCanContainSpaces() {
      string fileContents =
        "test = hello world";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(configurationFile.Get<string>(null, "test"), Is.EqualTo("hello world"));
    }

    /// <summary>
    ///   Verifies that values enclosed in quotes can embed comment characters
    /// </summary>
    [Test]
    public void OptionValuesWithQuotesCanEmbedComments() {
      string fileContents =
        "test = \"This ; is # not a comment\" # but this is";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(
        configurationFile.Get<string>(null, "test"),
        Is.EqualTo("\"This ; is # not a comment\"")
      );
    }

    /// <summary>
    ///   Verifies that values can end on a quote without causing trouble
    /// </summary>
    [Test]
    public void CommentsCanEndWithAQuote() {
      string fileContents =
        "test = \"This value ends with a quote\"";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(
        configurationFile.Get<string>(null, "test"),
        Is.EqualTo("\"This value ends with a quote\"")
      );
    }

    /// <summary>
    ///   Verifies that values can forget the closing quote without causing trouble
    /// </summary>
    [Test]
    public void ClosingQuoteCanBeOmmitted() {
      string fileContents =
        "test = \"No closing quote";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(
        configurationFile.Get<string>(null, "test"),
        Is.EqualTo("\"No closing quote")
      );
    }

    /// <summary>
    ///   Verifies that text placed after the closing quote will also be part of
    ///   an option's value
    /// </summary>
    [Test]
    public void TextAfterClosingQuoteBecomesPartOfValue() {
      string fileContents =
        "test = \"Begins here\" end ends here";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(
        configurationFile.Get<string>(null, "test"),
        Is.EqualTo("\"Begins here\" end ends here")
      );
    }

  }

} // namespace Nuclex.Support.Settings

#endif // UNITTEST
