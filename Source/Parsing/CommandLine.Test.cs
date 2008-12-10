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

namespace Nuclex.Support.Parsing {

  /// <summary>Ensures that the command line parser is working properly</summary>
  [TestFixture]
  public class CommandLineTest {
  
  /*
    struct CommandLine {
      [Option]
      bool? Option;
      [Option]
      int? Width;
      [Option]
      TypeCode Code;
      [Values]
      string[] Values;
    }
*/
    /// <summary>Validates that a single argument without quotes can be parsed</summary>
    [Test]
    public void TestParseSingleNakedArgument() {
      CommandLine.Parse("Hello");
    }

    /// <summary>
    ///   Validates that the parser can handle a single argument initator without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseLoneArgumentInitiator() {
      CommandLine.Parse("/");
      CommandLine.Parse("-");
      CommandLine.Parse("--");
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseMultipleLoneArgumentInitiators() {
      CommandLine.Parse("/ // /");
      CommandLine.Parse("- -- -");
      CommandLine.Parse("-- --- --");
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseArgumentInitiatorsWithInvalidNames() {
      CommandLine.Parse("/=:");
      CommandLine.Parse("-/=");
      CommandLine.Parse("--:/");
    }

    /// <summary>
    ///   Validates that the parser can handle an command line consisting of only spaces
    /// </summary>
    [Test]
    public void TestParseSpacesOnly() {
      CommandLine.Parse(" \t ");
    }

    /// <summary>
    ///   Validates that the parser can handle a quoted argument that's missing
    ///   the closing quote
    /// </summary>
    [Test]
    public void TestParseQuoteArgumentWithoutClosingQuote() {
      CommandLine.Parse("\"Quoted argument");
    }

    /// <summary>Validates that normal arguments can be parsed</summary>
    [Test]
    public void TestParseOptions() {
      CommandLine.Parse("Hello -World /This --Is \"a test\"");
    }

    /// <summary>Validates that null can be parsed</summary>
    [Test]
    public void TestParseNull() {
      Assert.IsNotNull(CommandLine.Parse(null));
    }

  }

} // namespace Nuclex.Support.Parsing

#endif // UNITTEST