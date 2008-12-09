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