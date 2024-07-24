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

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit tests for the observable helper</summary>
  [TestFixture]
  internal class ObservableHelperTest {

    #region class TestReferenceType

    /// <summary>Example class on which unit test generates change notifications</summary>
    public class TestReferenceType {

      /// <summary>Example property that will be reported to have changed</summary>
      public int SomeProperty { get; set; }

    }

    #endregion // class TestReferenceType

    #region struct TestValueType

    /// <summary>Example class on which unit test generates change notifications</summary>
    public struct TestValueType {

      /// <summary>Example property that will be reported to have changed</summary>
      public int SomeProperty { get; set; }

    }

    #endregion // struct TestValueType

    /// <summary>
    ///   Verifies that the name of a property accessed in a lambda expression
    ///   can be obtained.
    /// </summary>
    [Test]
    public void CanObtainPropertyNameFromLambdaExpression() {
      string propertyName = ObservableHelper.GetPropertyName(
        () => SomeReferenceType.SomeProperty
      );
      Assert.AreEqual("SomeProperty", propertyName);
    }

    /// <summary>
    ///   Verifies that the name of a property assigned in a lambda expression
    ///   can be obtained.
    /// </summary>
    [Test]
    public void CanObtainPropertyNameFromBoxedLambdaExpression() {
      string propertyName = ObservableHelper.GetPropertyName(
        () => (object)(SomeValueType.SomeProperty)
      );
      Assert.AreEqual("SomeProperty", propertyName);
    }

    /// <summary>Helper used to construct lambda expressions</summary>
    protected static TestReferenceType SomeReferenceType { get; set; }

    /// <summary>Helper used to construct lambda expressions</summary>
    protected static TestValueType SomeValueType { get; set; }

  }

}
