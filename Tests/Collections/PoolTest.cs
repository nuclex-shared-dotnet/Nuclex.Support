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

  /// <summary>Unit tests for the Pool class</summary>
  [TestFixture]
  internal class PoolTest {

    #region class TestClass

    /// <summary>Used to test the pool</summary>
    private class TestClass : IRecyclable {

      /// <summary>Returns the object to its initial state</summary>
      public void Recycle() {
        this.Recycled = true;
      }

      /// <summary>Whether the instance has been recycled</summary>
      public bool Recycled;

    }

    #endregion // class TestClass

    #region class NoDefaultConstructor

    /// <summary>Used to test the pool</summary>
    private class NoDefaultConstructor {

      /// <summary>Private constructor so no instances can be created</summary>
      private NoDefaultConstructor() { }

    }

    #endregion // class NoDefaultConstructor

    /// <summary>
    ///   Verifies that the pool can return newly constructed objects
    /// </summary>
    [Test]
    public void NewInstancesCanBeObtained() {
      Pool<TestClass> pool = new Pool<TestClass>();
      Assert.IsNotNull(pool.Get());
    }

    /// <summary>
    ///   Verifies that an exception is thrown if the pool's default instance creator is used
    ///   on a type that doesn't have a default constructor
    /// </summary>
    [Test]
    public void UsingDefaultInstanceCreatorRequiresDefaultConstructor() {
      Assert.Throws<ArgumentException>(
        delegate() { new Pool<NoDefaultConstructor>(); }
      );
    }

    /// <summary>
    ///   Tests whether the pool can redeem objects that are no longer used
    /// </summary>
    [Test]
    public void InstancesCanBeRedeemed() {
      Pool<TestClass> pool = new Pool<TestClass>();
      pool.Redeem(new TestClass());
    }

    /// <summary>
    ///   Tests whether the Recycle() method is called at the appropriate time
    /// </summary>
    [Test]
    public void RedeemedItemsWillBeRecycled() {
      Pool<TestClass> pool = new Pool<TestClass>();
      TestClass x = new TestClass();

      Assert.IsFalse(x.Recycled);
      pool.Redeem(x);
      Assert.IsTrue(x.Recycled);
    }

    /// <summary>Verifies that the pool's Capacity is applied correctly</summary>
    [Test]
    public void PoolCapacityCanBeAdjusted() {
      Pool<TestClass> pool = new Pool<TestClass>(123);
      Assert.AreEqual(123, pool.Capacity);
      pool.Capacity = 321;
      Assert.AreEqual(321, pool.Capacity);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
