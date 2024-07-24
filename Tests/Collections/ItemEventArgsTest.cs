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

#if UNITTEST

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the item event argument container</summary>
  [TestFixture]
  internal class ItemEventArgsTest {

    /// <summary>
    ///   Tests whether an integer argument can be stored in the argument container
    /// </summary>
    [Test]
    public void IntegersCanBeCarried() {
      var test = new ItemEventArgs<int>(12345);
      Assert.AreEqual(12345, test.Item);
    }

    /// <summary>
    ///   Tests whether a string argument can be stored in the argument container
    /// </summary>
    [Test]
    public void StringsCanBeCarried() {
      var test = new ItemEventArgs<string>("hello world");
      Assert.AreEqual("hello world", test.Item);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
