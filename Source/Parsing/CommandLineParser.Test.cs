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
      Assert.AreEqual(
        true.ToString(),
        new CommandLineParser(new string[] { "-hello" })["hello"],
        "Argument with minus sign is recognized"
      );
      Assert.AreEqual(
        true.ToString(),
        new CommandLineParser(new string[] { "--hello" })["hello"],
        "Argument with double minus sign is recognized"
      );
      Assert.AreEqual(
        true.ToString(),
        new CommandLineParser(new string[] { "/hello" })["hello"],
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

  }

} // namespace Nuclex.Support.Parsing

#endif // UNITTEST