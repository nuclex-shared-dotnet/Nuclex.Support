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
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the Parentable class</summary>
  [TestFixture]
  internal class ParentableTest {

    #region class TestParentable

    /// <summary>Parentable object that can be the child of an int</summary>
    private class TestParentable : Parentable<int> {

      /// <summary>Initializes a new instance of the parentable test class</summary>
      public TestParentable() { }

      /// <summary>The parent object that owns this instance</summary>
      public int GetParent() {
        return base.Parent;
      }

      /// <summary>Invoked whenever the instance's owner changes</summary>
      /// <remarks>
      ///   When items are parented for the first time, the oldParent argument will
      ///   be null. Also, if the element is removed from the collection, the
      ///   current parent will be null.
      /// </remarks>
      /// <param name="oldParent">Previous owner of the instance</param>
      protected override void OnParentChanged(int oldParent) {
        this.parentChangedCalled = true;

        base.OnParentChanged(oldParent); // to satisfy NCover :-/
      }

      /// <summary>Whether the OnParentChanged method has been called</summary>
      public bool ParentChangedCalled {
        get { return this.parentChangedCalled; }
      }

      /// <summary>Whether the OnParentChanged method has been called</summary>
      private bool parentChangedCalled;

    }

    #endregion // class TestParentable

    /// <summary>
    ///   Tests whether a parent can be assigned and then retrieved from
    ///   the parentable object
    /// </summary>
    [Test]
    public void TestParentAssignment() {
      TestParentable testParentable = new TestParentable();

      testParentable.SetParent(12345);
      Assert.AreEqual(12345, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether a parent can be assigned and then retrieved from
    ///   the parentable object
    /// </summary>
    [Test]
    public void TestParentChangedNotification() {
      TestParentable testParentable = new TestParentable();

      testParentable.SetParent(12345);

      Assert.IsTrue(testParentable.ParentChangedCalled);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
