#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

#if ENABLE_BROKEN_COMMAND_LINE_PARSER // Too bugged. 100% test coverage not possible.

namespace Nuclex.Support.Parsing {

  /// <summary>Ensures that the command line parser is working properly</summary>
  [TestFixture]
  public class CommandLineParserTest {

    /// <summary>Validates that normal arguments can be parsed</summary>
    [Test]
    public void TestArrayConstructorWithPlainArguments() {
      Assert.IsTrue(
        new CommandLineParser(new string[] { "-hello" }).HasArgument("hello"),
        "Argument with minus sign is recognized"
      );
      Assert.IsTrue(
        new CommandLineParser(new string[] { "--hello" }).HasArgument("hello"),
        "Argument with double minus sign is recognized"
      );
      Assert.IsTrue(
        new CommandLineParser(new string[] { "/hello" }).HasArgument("hello"),
        "Argument with slash is recognized"
      );
    }

    /// <summary>Validates that argument assignments are working</summary>
    [Test]
    public void TestArrayConstructorWithAssignments() {
      Assert.AreEqual(
        "world",
        new CommandLineParser(new string[] { "-hello:world" })["hello"],
        "Argument can be assigned with a double colon"
      );
      Assert.AreEqual(
        "world",
        new CommandLineParser(new string[] { "-hello=world" })["hello"],
        "Argument can be assigned with a equality sign"
      );
      Assert.AreEqual(
        "world",
        new CommandLineParser(new string[] { "-hello", "world" })["hello"],
        "Argument can be assigned with a space"
      );
    }

    /// <summary>
    ///   Validates that loosely specified values are recognized by the parser
    /// </summary>
    [Test]
    public void TestArrayConstructorWithLooseValues() {
      Assert.IsTrue(
        new CommandLineParser(new string[] { "hello" }).Values.Contains("hello"),
        "Plain loose value is recognized"
      );
      Assert.IsTrue(
        new CommandLineParser(new string[] { "-hello:world", "foo" }).Values.Contains("foo"),
        "Loose value following an assignment is recognized"
      );
    }

    /// <summary>
    ///   Tests whether the parser can parse the processes current command line if
    ///   the default constructor is used
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      new CommandLineParser();
    }

    /// <summary>
    ///   Tests whether the string constructor works for simple arguments being
    ///   specified on the command line
    /// </summary>
    [Test]
    public void TestStringConstructorWithSimpleArguments() {
      CommandLineParser parser = new CommandLineParser("argument1 argument2");
      Assert.AreEqual("argument1", parser.Values[0]);
      Assert.AreEqual("argument2", parser.Values[1]);
    }

    // TODO: This test fails!!
#if FAILED_TEST
    /// <summary>
    ///   Bullshit
    /// </summary>
    [Test]
    public void TestStringConstructorWithQuotedArguments() {
      CommandLineParser parser = new CommandLineParser("\"this is a single argument\"");
      Assert.AreEqual("this is a single argument", parser.Values[0]);
    }
#endif

    /// <summary>
    ///   Tests whether the string constructor recognizes an unfinished argument
    ///   (that is, an argument that gets 'nothing' assigned)
    /// </summary>
    [Test]
    public void TestStringConstructorWithUnfinishedAssignment() {
      CommandLineParser parser = new CommandLineParser("--hello= --world=");
      Assert.AreEqual(0, parser.Values.Count);
    }

    /// <summary>
    ///   Tests whether the string constructor recognizes an argument with a space before
    ///   its assigned value
    /// </summary>
    [Test]
    public void TestStringConstructorWithSpacedAssignment() {
      CommandLineParser parser = new CommandLineParser("--hello= world");
      Assert.AreEqual(1, parser.Values.Count);
    }

  }

} // namespace Nuclex.Support.Parsing

#endif // ENABLE_BROKEN_COMMAND_LINE_PARSER

#endif // UNITTEST