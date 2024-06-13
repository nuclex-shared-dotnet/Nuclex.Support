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

  /// <summary>Unit Test for the shared instance provider class</summary>
  [TestFixture]
  internal class SharedTest {

    #region class Dummy

    /// <summary>Dummy class for testing the shared instance provider</summary>
    private class Dummy {
      /// <summary>Initializes a new dummy</summary>
      public Dummy() {}
    }

    #endregion // class Dummy

    /// <summary>
    ///   Verifies that the shared instance provider returns the same instance of a class
    ///   when asked for the same class twice.
    /// </summary>
    [Test]
    public void TestSameInstance() {
      #pragma warning disable 0618
      Dummy dummyInstance = Shared<Dummy>.Instance;
      Dummy otherDummyInstance = Shared<Dummy>.Instance;
      #pragma warning restore 0618
      
      // Make sure they're the same instance. We could have put an instance counter in
      // the dummy class, but this might or might not work well across multiple tests
      // because the order in which tests are executed is undefined and Shared<> changes
      // its global state when the first test is run by remembering the instance.
      //
      // Maybe this really is a defect in Shared<> and the class should be equipped with
      // a method such as Discard() or Dispose() to get rid of the instance?
      Assert.AreSame(dummyInstance, otherDummyInstance);
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
