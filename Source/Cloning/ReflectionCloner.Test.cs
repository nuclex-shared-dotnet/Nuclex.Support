#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

namespace Nuclex.Support.Cloning {

  /// <summary>Unit Test for the reflection-based cloner</summary>
  [TestFixture]
  public class ReflectionClonerTest {

    #region class TestReferenceType

    /// <summary>A reference type being used for testing</summary>
    private class TestReferenceType {

      /// <summary>Field holding an integer value for testing</summary>
      public int TestField;
      /// <summary>Property holding an integer value for testing</summary>
      public int TestProperty { get; set; }

    }

    #endregion // class TestReferenceType

    #region struct TestValueType

    /// <summary>A value type being used for testing</summary>
    private struct TestValueType {

      /// <summary>Field holding an integer value for testing</summary>
      public int TestField;
      /// <summary>Property holding an integer value for testing</summary>
      public int TestProperty { get; set; }

    }

    #endregion // struct TestValueType

    #region struct HierarchicalValueType

    /// <summary>A value type containiner other complex types used for testing</summary>
    private struct HierarchicalValueType {

      /// <summary>Field holding an integer value for testing</summary>
      public int TestField;
      /// <summary>Property holding an integer value for testing</summary>
      public int TestProperty { get; set; }
      /// <summary>Value type field for testing</summary>
      public TestValueType ValueTypeField;
      /// <summary>Value type property for testing</summary>
      public TestValueType ValueTypeProperty { get; set; }
      /// <summary>Reference type field for testing</summary>
      public TestReferenceType ReferenceTypeField;
      /// <summary>Reference type property for testing</summary>
      public TestReferenceType ReferenceTypeProperty { get; set; }
      /// <summary>An array field of reference types</summary>
      public TestReferenceType[,][] ReferenceTypeArrayField;
      /// <summary>An array property of reference types</summary>
      public TestReferenceType[,][] ReferenceTypeArrayProperty { get; set; }

    }

    #endregion // struct HierarchicalValueType

    #region struct HierarchicalReferenceType

    /// <summary>A value type containiner other complex types used for testing</summary>
    private class HierarchicalReferenceType {

      /// <summary>Field holding an integer value for testing</summary>
      public int TestField;
      /// <summary>Property holding an integer value for testing</summary>
      public int TestProperty { get; set; }
      /// <summary>Value type field for testing</summary>
      public TestValueType ValueTypeField;
      /// <summary>Value type property for testing</summary>
      public TestValueType ValueTypeProperty { get; set; }
      /// <summary>Reference type field for testing</summary>
      public TestReferenceType ReferenceTypeField;
      /// <summary>Reference type property for testing</summary>
      public TestReferenceType ReferenceTypeProperty { get; set; }
      /// <summary>An array field of reference types</summary>
      public TestReferenceType[,][] ReferenceTypeArrayField;
      /// <summary>An array property of reference types</summary>
      public TestReferenceType[,][] ReferenceTypeArrayProperty { get; set; }

    }

    #endregion // struct HierarchicalReferenceType

    /// <summary>
    ///   Verifies that a field-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).ShallowClone(original, false);
      verifyShallowFieldBasedValueTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).ShallowClone(original, false);
      verifyShallowFieldBasedReferenceTypeCopy(original, clone);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).DeepClone(original, false);
      verifyDeepFieldBasedValueTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).DeepClone(original, false);
      verifyDeepFieldBasedReferenceTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).ShallowClone(original, true);
      verifyShallowPropertyBasedValueTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).ShallowClone(original, true);
      verifyShallowPropertyBasedReferenceTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).DeepClone(original, true);
      verifyDeepPropertyBasedValueTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).DeepClone(original, true);
      verifyDeepPropertyBasedReferenceTypeCopy(ref original, ref clone);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a value type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyShallowFieldBasedValueTypeCopy(
      ref HierarchicalValueType original, ref HierarchicalValueType clone
    ) {
      Assert.AreEqual(original.TestField, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
      Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreSame(original.ReferenceTypeField, clone.ReferenceTypeField);
      Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a reference type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyShallowFieldBasedReferenceTypeCopy(
      HierarchicalReferenceType original, HierarchicalReferenceType clone
    ) {
      Assert.AreEqual(original.TestField, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
      Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreSame(original.ReferenceTypeField, clone.ReferenceTypeField);
      Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a value type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyDeepFieldBasedValueTypeCopy(
      ref HierarchicalValueType original, ref HierarchicalValueType clone
    ) {
      Assert.AreEqual(original.TestField, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
      Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeField.TestField, clone.ReferenceTypeField.TestField
      );
      Assert.AreEqual(
        original.ReferenceTypeField.TestProperty, clone.ReferenceTypeField.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestField, clone.ReferenceTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.AreNotSame(original.ReferenceTypeField, clone.ReferenceTypeField);
      Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a reference type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyDeepFieldBasedReferenceTypeCopy(
      ref HierarchicalReferenceType original, ref HierarchicalReferenceType clone
    ) {
      Assert.AreEqual(original.TestField, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
      Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeField.TestField, clone.ReferenceTypeField.TestField
      );
      Assert.AreEqual(
        original.ReferenceTypeField.TestProperty, clone.ReferenceTypeField.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestField, clone.ReferenceTypeProperty.TestField
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.AreNotSame(original.ReferenceTypeField, clone.ReferenceTypeField);
      Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a value type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyShallowPropertyBasedValueTypeCopy(
      ref HierarchicalValueType original, ref HierarchicalValueType clone
    ) {
      Assert.AreEqual(0, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeField.TestField);
      Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a reference type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyShallowPropertyBasedReferenceTypeCopy(
      ref HierarchicalReferenceType original, ref HierarchicalReferenceType clone
    ) {
      Assert.AreEqual(0, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeField.TestField);
      Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a value type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyDeepPropertyBasedValueTypeCopy(
      ref HierarchicalValueType original, ref HierarchicalValueType clone
    ) {
      Assert.AreEqual(0, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeField.TestField);
      Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreEqual(0, clone.ReferenceTypeProperty.TestField);
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a reference type matches
    ///   the expected outcome for this type of clone
    /// </summary>
    /// <param name="original">Original instance that has been cloned</param>
    /// <param name="clone">Cloned instance that will be verified</param>
    private static void verifyDeepPropertyBasedReferenceTypeCopy(
      ref HierarchicalReferenceType original, ref HierarchicalReferenceType clone
    ) {
      Assert.AreEqual(0, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeField.TestField);
      Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
      Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreEqual(0, clone.ReferenceTypeProperty.TestField);
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.IsNull(clone.ReferenceTypeField);
      Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
    }

    /// <summary>Creates a value type with random data for testing</summary>
    /// <returns>A new value type with random data</returns>
    private static HierarchicalValueType createValueType() {
      return new HierarchicalValueType() {
        TestField = 123,
        TestProperty = 321,
        ReferenceTypeArrayField = new TestReferenceType[2, 4][] {
          {
            null, null, null, null
          },
          {
            null, null, null,
            new TestReferenceType[3] {
              new TestReferenceType() { TestField = 101, TestProperty = 202 },
              null,
              new TestReferenceType() { TestField = 909, TestProperty = 808 }
            }
          },
        },
        ReferenceTypeArrayProperty = new TestReferenceType[2, 4][] {
          {
            null, null, null, null
          },
          {
            null, null, null,
            new TestReferenceType[3] {
              new TestReferenceType() { TestField = 303, TestProperty = 404 },
              null,
              new TestReferenceType() { TestField = 707, TestProperty = 606 }
            }
          },
        },
        ValueTypeField = new TestValueType() {
          TestField = 456,
          TestProperty = 654
        },
        ValueTypeProperty = new TestValueType() {
          TestField = 789,
          TestProperty = 987,
        },
        ReferenceTypeField = new TestReferenceType() {
          TestField = 135,
          TestProperty = 531
        },
        ReferenceTypeProperty = new TestReferenceType() {
          TestField = 246,
          TestProperty = 642,
        }
      };
    }

    /// <summary>Creates a reference type with random data for testing</summary>
    /// <returns>A new reference type with random data</returns>
    private static HierarchicalReferenceType createReferenceType() {
      return new HierarchicalReferenceType() {
        TestField = 123,
        TestProperty = 321,
        ReferenceTypeArrayField = new TestReferenceType[2, 4][] {
          {
            null, null, null, null
          },
          {
            null, null, null,
            new TestReferenceType[3] {
              new TestReferenceType() { TestField = 101, TestProperty = 202 },
              null,
              new TestReferenceType() { TestField = 909, TestProperty = 808 }
            }
          },
        },
        ReferenceTypeArrayProperty = new TestReferenceType[2, 4][] {
          {
            null, null, null, null
          },
          {
            null, null, null,
            new TestReferenceType[3] {
              new TestReferenceType() { TestField = 303, TestProperty = 404 },
              null,
              new TestReferenceType() { TestField = 707, TestProperty = 606 }
            }
          },
        },
        ValueTypeField = new TestValueType() {
          TestField = 456,
          TestProperty = 654
        },
        ValueTypeProperty = new TestValueType() {
          TestField = 789,
          TestProperty = 987,
        },
        ReferenceTypeField = new TestReferenceType() {
          TestField = 135,
          TestProperty = 531
        },
        ReferenceTypeProperty = new TestReferenceType() {
          TestField = 246,
          TestProperty = 642,
        }
      };
    }

  }

} // namespace Nuclex.Support.Cloning

#endif // UNITTEST
