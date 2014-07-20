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

using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Nuclex.Support.Settings {

  /// <summary>Unit tests for the configuration file store</summary>
  [TestFixture]
  internal class ConfigurationFileStoreTest {

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

  }

} // namespace Nuclex.Support.Settings

#endif // UNITTEST
