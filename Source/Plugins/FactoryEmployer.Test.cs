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
using System.IO;

#if UNITTEST

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Nuclex.Support.Plugins {

  /// <summary>Unit Test for the factory employer class</summary>
  [TestFixture]
  public class FactoryEmployerTest {

    #region class Base

    /// <summary>
    ///   Abstract base class to serve as abstract product for testing the factory employer
    /// </summary>
    private abstract class Base { }

    #endregion // class Base

    #region class Derived

    /// <summary>
    ///   Class derived from the abstract base to serve as concrete product for
    ///   testing the factory employer
    /// </summary>
    private class Derived : Base { }

    #endregion // class Derived

    #region class Unrelated

    /// <summary>Unrelated class used to test the factory employer</summary>
    private class Unrelated { }

    #endregion // class Unrelated

    /// <summary>
    ///   Tests whether the factory employer can detect employable types
    /// </summary>
    [Test]
    public void TestCanEmploy() {
      FactoryEmployer<Base> testEmployer = new FactoryEmployer<Base>();

      Assert.IsFalse(testEmployer.CanEmploy(typeof(Base)));
      Assert.IsTrue(testEmployer.CanEmploy(typeof(Derived)));
      Assert.IsFalse(testEmployer.CanEmploy(typeof(Unrelated)));
    }

    /// <summary>
    ///   Tests whether the factory employer throws an exception when it is asked to
    ///   employ an abstract class
    /// </summary>
    [Test, ExpectedException(typeof(MissingMethodException))]
    public void TestThrowOnEmployAbstractClass() {
      FactoryEmployer<Base> testEmployer = new FactoryEmployer<Base>();

      testEmployer.Employ(typeof(Base));
    }

    /// <summary>
    ///   Tests whether the factory employer throws an exception when it is asked to
    ///   employ a class that is not the product type or a derivative thereof
    /// </summary>
    [Test, ExpectedException(typeof(InvalidCastException))]
    public void TestThrowOnEmployUnrelatedClass() {
      FactoryEmployer<Base> testEmployer = new FactoryEmployer<Base>();

      testEmployer.Employ(typeof(Unrelated));
    }

    /// <summary>
    ///   Tests whether the factory employer can employ a class derived from the product
    /// </summary>
    [Test]
    public void TestEmployClassDerivedFromProduct() {
      FactoryEmployer<Base> testEmployer = new FactoryEmployer<Base>();

      testEmployer.Employ(typeof(Derived));

      Assert.AreEqual(1, testEmployer.Factories.Count);
      Assert.AreEqual(typeof(Derived), testEmployer.Factories[0].ConcreteType);
      Assert.IsInstanceOfType(
        typeof(Derived), testEmployer.Factories[0].CreateInstance()
      );
    }

    /// <summary>
    ///   Tests whether the factory employer can employ the product class itself if it
    ///   isn't abstract
    /// </summary>
    [Test]
    public void TestEmployProduct() {
      FactoryEmployer<Unrelated> testEmployer = new FactoryEmployer<Unrelated>();

      testEmployer.Employ(typeof(Unrelated));

      Assert.AreEqual(1, testEmployer.Factories.Count);
      Assert.AreEqual(typeof(Unrelated), testEmployer.Factories[0].ConcreteType);
      Assert.IsInstanceOfType(
        typeof(Unrelated), testEmployer.Factories[0].CreateInstance()
      );
    }

  }

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
