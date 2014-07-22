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
using System.Text;

using NUnit.Framework;

namespace Nuclex.Support.Settings {

  /// <summary>Unit tests for the configuration file store</summary>
  [TestFixture]
  internal class ConfigurationFileStoreTest {

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

    /// <summary>
    ///   Verifies that text placed after the closing quote will also be part of
    ///   an option's value
    /// </summary>
    [Test]
    public void OptionValuesCanBeChanged() {
      string fileContents = "test = 123 ; comment";
      ConfigurationFileStore configurationFile = load(fileContents);

      configurationFile.Set(null, "test", "hello world");

      Assert.That(
        save(configurationFile),
        Contains.Substring("hello world").And.ContainsSubstring("comment")
      );
    }

    /// <summary>
    ///   Verifies that options can be added to the configuration file
    /// </summary>
    [Test]
    public void OptionsCanBeAdded() {
      var configurationFile = new ConfigurationFileStore();

      configurationFile.Set<string>(null, "test", "123");
      Assert.That(configurationFile.Get<string>(null, "test"), Is.EqualTo("123"));
    }

    /// <summary>
    ///   Verifies that options can be added to the configuration file
    /// </summary>
    [Test]
    public void CategoriesCanBeAdded() {
      var configurationFile = new ConfigurationFileStore();

      configurationFile.Set<string>("general", "sol", "42");

      Assert.That(
        configurationFile.EnumerateCategories(), Is.EquivalentTo(new string[] { "general" })
      );
      Assert.That(save(configurationFile), Contains.Substring("[general]"));
    }

    /// <summary>
    ///   Verifies that accessing an option that doesn't exist throws an exception
    /// </summary>
    [Test]
    public void AccessingNonExistingOptionThrowsException() {
      var configurationFile = new ConfigurationFileStore();

      Assert.That(
        () => configurationFile.Get<string>(null, "doesn't exist"),
        Throws.Exception.AssignableTo<KeyNotFoundException>()
      );
    }

    /// <summary>
    ///   Verifies that accessing a category that doesn't exist throws an exception
    /// </summary>
    [Test]
    public void AccessingNonExistingCategoryThrowsException() {
      var configurationFile = new ConfigurationFileStore();
      configurationFile.Set<string>(null, "test", "123");

      Assert.That(
        () => configurationFile.Get<string>("doesn't exist", "test"),
        Throws.Exception.AssignableTo<KeyNotFoundException>()
      );
    }

    /// <summary>
    ///   Verifies that it's possible to enumerate a category that doesn't exist
    /// </summary>
    [Test]
    public void NonExistingCategoryCanBeEnumerated() {
      var configurationFile = new ConfigurationFileStore();

      Assert.That(configurationFile.EnumerateOptions("doesn't exist"), Is.Empty);
    }

    /// <summary>
    ///   Verifies that it's possible to create an option without a value
    /// </summary>
    [Test]
    public void ValuelessOptionsCanBeCreated() {
      var configurationFile = new ConfigurationFileStore();

      configurationFile.Set<string>(null, "test", null);
      Assert.That(configurationFile.Get<string>(null, "test"), Is.Null.Or.Empty);
    }

    /// <summary>
    ///   Verifies that it's possible to assign an empty value to an option
    /// </summary>
    [Test]
    public void OptionValueCanBeCleared() {
      string fileContents = "test = 123 ; comment";
      ConfigurationFileStore configurationFile = load(fileContents);

      configurationFile.Set<string>(null, "test", null);
      Assert.That(configurationFile.Get<string>(null, "test"), Is.Null.Or.Empty);
    }

    /// <summary>
    ///   Verifies that it's possible to remove options from the configuration file
    /// </summary>
    [Test]
    public void OptionsCanBeRemoved() {
      var configurationFile = new ConfigurationFileStore();
      configurationFile.Set<string>(null, "test", null);

      Assert.That(configurationFile.Remove(null, "test"), Is.True);
      
      string value;
      Assert.That(configurationFile.TryGet<string>(null, "test", out value), Is.False);
    }

    /// <summary>
    ///   Verifies that it's possible to assign an empty value to an option
    /// </summary>
    [Test]
    public void RemovingOptionShiftsFollowingOptionsUp() {
      string fileContents =
        "first = 1\r\n" +
        "second = 2";
      ConfigurationFileStore configurationFile = load(fileContents);

      Assert.That(configurationFile.Remove(null, "first"), Is.True);
      configurationFile.Set<string>(null, "second", "yay! first!");

      Assert.That(save(configurationFile), Has.No.ContainsSubstring("1"));
      Assert.That(save(configurationFile), Contains.Substring("second"));
      Assert.That(save(configurationFile), Contains.Substring("yay! first!"));
    }

    /// <summary>
    ///   Verifies that it's not an error to remove an option from a non-existing category
    /// </summary>
    [Test]
    public void CanRemoveOptionFromNonExistingCategory() {
      var configurationFile = new ConfigurationFileStore();
      Assert.That(configurationFile.Remove("nothing", "first"), Is.False);
    }

    /// <summary>
    ///   Verifies that it's not an error to remove a non-existing option
    /// </summary>
    [Test]
    public void CanRemoveNonExistingOption() {
      var configurationFile = new ConfigurationFileStore();
      Assert.That(configurationFile.Remove(null, "first"), Is.False);
    }

    /// <summary>
    ///   Verifies that it's not an error to remove a non-existing option
    /// </summary>
    [
      Test,
      TestCase("nothing=", typeof(string)),
      TestCase("text = world", typeof(string)),
      TestCase("short=9", typeof(int)),
      TestCase("integer = 123", typeof(int)),
      TestCase("integer = 123  ", typeof(int)),
      TestCase("string=x", typeof(string)),
      TestCase("string = 123s", typeof(string)),
      TestCase("float = 123.45", typeof(float)),
      TestCase("float = 123.45  ", typeof(float)),
      TestCase("boolean = true", typeof(bool)),
      TestCase("boolean = false", typeof(bool)),
      TestCase("boolean = yes", typeof(bool)),
      TestCase("boolean = no", typeof(bool))
    ]
    public void OptionTypeCanBeIdentified(string assignment, Type expectedType) {
      ConfigurationFileStore configurationFile = load(assignment);

      OptionInfo info;
      using(
        IEnumerator<OptionInfo> enumerator = configurationFile.EnumerateOptions().GetEnumerator()
      ) {
        Assert.That(enumerator.MoveNext(), Is.True);
        info = enumerator.Current;
        Assert.That(enumerator.MoveNext(), Is.False);
      }

      Assert.That(info.OptionType, Is.EqualTo(expectedType));
    }

    /// <summary>
    ///   Verifies that configuration files containing duplicate option names can not
    ///   be used with the configuration file store
    /// </summary>
    [Test]
    public void FilesWithDuplicateOptionNamesCannotBeProcessed() {
      string fileContents =
        "duplicate name = 1\r\n" +
        "duplicate name = 2";

      Assert.That(() => load(fileContents), Throws.Exception);
    }

    /// <summary>
    ///   Verifies that configuration files containing duplicate option names can not
    ///   be used with the configuration file store
    /// </summary>
    [
      Test,
      TestCase("value = yes", true),
      TestCase("value = true", true),
      TestCase("value = no", false),
      TestCase("value = false", false)
    ]
    public void BooleanLiteralsAreUnderstood(string fileContents, bool expectedValue) {
      ConfigurationFileStore configurationFile = load(fileContents);

      if(expectedValue) { 
        Assert.That(configurationFile.Get<bool>(null, "value"), Is.True);
      } else {
        Assert.That(configurationFile.Get<bool>(null, "value"), Is.False);
      }
    }

    /// <summary>Loads a configuration file from a string</summary>
    /// <param name="fileContents">Contents of the configuration file</param>
    /// <returns>The configuration file loaded from the string</returns>
    private static ConfigurationFileStore load(string fileContents) {
      using(var reader = new StringReader(fileContents)) {
        return ConfigurationFileStore.Parse(reader);
      }
    }

    /// <summary>Saves a configuration file into a string</summary>
    /// <param name="configurationFile">Configuration file that will be saved</param>
    /// <returns>Contents of the configuration file</returns>
    private static string save(ConfigurationFileStore configurationFile) {
      var builder = new StringBuilder();

      using(var writer = new StringWriter(builder)) {
        configurationFile.Save(writer);
        writer.Flush();
      }

      return builder.ToString();
    }

  }

} // namespace Nuclex.Support.Settings

#endif // UNITTEST
