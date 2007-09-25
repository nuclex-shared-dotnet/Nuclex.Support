using System;
using System.Collections.Generic;
using System.Text;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Parsing {

  /// <summary>Ensures that the command line parser is working properly</summary>
  [TestFixture]
  public class CommandLineParserTest {

    /// <summary>Validates that normal arguments can be parsed</summary>
    [Test]
    public void TestPlainArguments() {
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
    public void TestAssignments() {
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
    public void TestLooseValues() {
      Assert.IsTrue(
        new CommandLineParser(new string[] { "hello" }).Values.Contains("hello"),
        "Plain loose value is recognized"
      );
      Assert.IsTrue(
        new CommandLineParser(new string[] { "-hello:world", "foo" }).Values.Contains("foo"),
        "Loose value following an assignment is recognized"
      );
    }

  }

} // namespace Nuclex.Support.Parsing

#endif // UNITTEST