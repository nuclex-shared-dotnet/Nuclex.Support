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
using System.Reflection;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the strign segment class</summary>
  [TestFixture]
  internal class TypeHelperTest {

    #region class NoDefaultConstructor

    /// <summary>Test class that doesn't have a default constructor</summary>
    private class NoDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      /// <param name="dummy">Dummy argument so this is no default constructor</param>
      public NoDefaultConstructor(int dummy) { }
    }

    #endregion // class NoDefaultConstructor

    #region class NonPublicDefaultConstructor

    /// <summary>Test class that has a non-public default constructor</summary>
    private class NonPublicDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      protected NonPublicDefaultConstructor() { }
    }

    #endregion // class NonPublicDefaultConstructor

    #region class PublicDefaultConstructor

    /// <summary>Test class that has a public default constructor</summary>
    private class PublicDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      public PublicDefaultConstructor() { }
    }

    #endregion // class PublicDefaultConstructor

    #region class Base

    /// <summary>Base class used to test the helper methods</summary>
    private class Base {
      /// <summary>A simple public field</summary>
      public int PublicBaseField;
      /// <summary>An automatic property with a hidden backing field</summary>
      public int PublicBaseProperty { get; set; }
    }

    #endregion // class Base

    #region class Derived

    /// <summary>Derived class used to test the helper methods</summary>
    private class Derived : Base {
      /// <summary>A simple public field</summary>
      public int PublicDerivedField;
      /// <summary>An automatic property with a hidden backing field</summary>
      public int PublicDerivedProperty { get; set; }
    }

    #endregion // class Derived

    #region class HasIgnoreAttribute

    /// <summary>Class that carries an IgnoreAttribute</summary>
    [Ignore("Just misusing NUnit's IgnoreAttribute to have an attribute to test against")]
    private class HasIgnoreAttribute { }

    #endregion // class HasIgnoreAttribute

    /// <summary>
    ///   Verifies that the type helper can determine whether a class is carrying an attribute
    /// </summary>
    [Test]
    public void CanDetermineIfTypeHasAttribute() {
      Assert.IsTrue(typeof(HasIgnoreAttribute).HasAttribute<IgnoreAttribute>());
      Assert.IsFalse(typeof(HasIgnoreAttribute).HasAttribute<TestAttribute>());
    }

    /// <summary>
    ///   Verifies that the GetFieldInfosIncludingBaseClasses() will include the backing
    ///   fields of automatically implemented properties in base classes
    /// </summary>
    [Test]
    public void CanGetBackingFieldsForPropertiesInBaseClasses() {
      FieldInfo[] fieldInfos = typeof(Derived).GetFieldInfosIncludingBaseClasses(
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
      );
      Assert.AreEqual(4, fieldInfos.Length);
    }

    /// <summary>
    ///   Useless test that avoids a compile warning about unused fields
    /// </summary>
    [Test]
    public void AvoidCompilerWarnings() {
      var derived = new Derived() {
        PublicBaseField = 123,
        PublicBaseProperty = 321,
        PublicDerivedField = 456
      };
    }

    /// <summary>Tests whether the default constructor detection works as expected</summary>
    [Test]
    public void TestDefaultConstructorDetection() {
      Assert.IsFalse(typeof(NoDefaultConstructor).HasDefaultConstructor());
      Assert.IsFalse(typeof(NonPublicDefaultConstructor).HasDefaultConstructor());
      Assert.IsTrue(typeof(PublicDefaultConstructor).HasDefaultConstructor());
    }

  }

} // namespace Nuclex.Support
