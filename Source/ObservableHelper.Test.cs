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

#endif // UNITTEST
