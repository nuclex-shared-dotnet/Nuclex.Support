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
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the path helper class</summary>
  [TestFixture]
  internal class PathHelperTest {

    /// <summary>
    ///   Tests whether the relative path creator keeps the absolute path if
    ///   the location being passed is not relative to the base path.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathOfNonRelativePath() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2"),
          platformify("D:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("D:/Folder1/Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("D:/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("D:/Folder1/Folder2/"))
      );

    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to the parent folder of the base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToParentFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2"),
          platformify("C:/Folder1")
        ),
        Is.EqualTo(platformify(".."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("C:/Folder1/")
        ),
        Is.EqualTo(platformify("../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative path to
    ///   the parent folder of the base path for windows paths with more than one level.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToParentFolderTwoLevels() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/Folder3"),
          platformify("C:/Folder1")
        ),
        Is.EqualTo(platformify("../.."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/Folder3/"),
          platformify("C:/Folder1/")
        ),
        Is.EqualTo(platformify("../../"))
      );
    }


    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to the parent folder of the base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToParentFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2"),
          platformify("/Folder1")
        ),
        Is.EqualTo(platformify(".."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/"),
          platformify("/Folder1/")
        ),
        Is.EqualTo(platformify("../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative path to
    ///   the parent folder of the base path for unix paths with more than one level.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToParentFolderTwoLevels() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/Folder3"),
          platformify("/Folder1")
        ),
        Is.EqualTo(platformify("../.."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/Folder3/"),
          platformify("/Folder1/")
        ),
        Is.EqualTo(platformify("../../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to a nested folder in the base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToNestedFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1"),
          platformify("C:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/"),
          platformify("C:/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("Folder2/"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to a nested folder in the base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToNestedFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1"),
          platformify("/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/"),
          platformify("/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("Folder2/"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to another folder on the same level as base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToSiblingFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("C:/Folder1/Folder2345")
        ),
        Is.EqualTo(platformify("../Folder2345"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2345/"),
          platformify("C:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("../Folder2"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to another folder on the same level as base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToSiblingFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/"),
          platformify("/Folder1/Folder2345")
        ),
        Is.EqualTo(platformify("../Folder2345"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2345/"),
          platformify("/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("../Folder2"))
      );
    }

    /// <summary>
    ///   Converts unix-style directory separators into the format used by the current platform
    /// </summary>
    /// <param name="path">Path to converts into the platform-dependent format</param>
    /// <returns>Platform-specific version of the provided unix-style path</returns>
    private string platformify(string path) {
      return path.Replace('/', Path.DirectorySeparatorChar);
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
