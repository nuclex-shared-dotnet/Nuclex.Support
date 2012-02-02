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

#if UNITTEST

using System;
using System.Collections.Generic;

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

    /// <summary>Verifies that clones of primitive types can be created</summary>
    [Test]
    public void PrimitiveTypesCanBeCloned() {
      int original = 12345;
      int clone = (new ReflectionCloner()).ShallowClone(original, false);
      Assert.AreEqual(original, clone);
    }

    /// <summary>Verifies that shallow clones of arrays can be made</summary>
    [Test]
    public void ShallowClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[] {
        new TestReferenceType() { TestField = 123, TestProperty = 456 }
      };
      TestReferenceType[] clone = (new ReflectionCloner()).ShallowClone(original, false);

      Assert.AreSame(original[0], clone[0]);
    }

    /// <summary>Verifies that deep clones of arrays can be made</summary>
    [Test]
    public void DeepClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[] {
        new TestReferenceType() { TestField = 123, TestProperty = 456 }
      };
      TestReferenceType[] clone = (new ReflectionCloner()).DeepClone(original, false);

      Assert.AreNotSame(original[0], clone[0]);
      Assert.AreEqual(original[0].TestField, clone[0].TestField);
      Assert.AreEqual(original[0].TestProperty, clone[0].TestProperty);
    }

    /// <summary>Verifies that deep clones of a generic list can be made</summary>
    [Test]
    public void GenericListsCanBeCloned() {
      var original = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      List<int> clone = (new ReflectionCloner()).DeepClone(original, false);

      CollectionAssert.AreEqual(original, clone);
    }

    /// <summary>Verifies that deep clones of a generic dictionary can be made</summary>
    [Test]
    public void GenericDictionariesCanBeCloned() {
      var original = new Dictionary<int, string>();
      original.Add(1, "one");
      Dictionary<int, string> clone = (new ReflectionCloner()).DeepClone(original, false);

      Assert.AreEqual("one", clone[1]);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).ShallowClone(original, false);
      verifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).ShallowClone(original, false);
      verifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).DeepClone(original, false);
      verifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).DeepClone(original, false);
      verifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).ShallowClone(original, true);
      verifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).ShallowClone(original, true);
      verifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = createValueType();
      HierarchicalValueType clone = (new ReflectionCloner()).DeepClone(original, true);
      verifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = createReferenceType();
      HierarchicalReferenceType clone = (new ReflectionCloner()).DeepClone(original, true);
      verifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a cloned object exhibits the expected state for the type of
    ///   clone that has been performed
    /// </summary>
    /// <param name="original">Original instance the clone was created from</param>
    /// <param name="clone">Cloned instance that will be checked for correctness</param>
    /// <param name="isDeepClone">Whether the cloned instance is a deep clone</param>
    /// <param name="isPropertyBasedClone">
    ///   Whether a property-based clone was performed
    /// </param>
    private static void verifyClone(
      HierarchicalReferenceType original, HierarchicalReferenceType clone,
      bool isDeepClone, bool isPropertyBasedClone
    ) {
      if(isPropertyBasedClone) {
        Assert.AreEqual(0, clone.TestField);
        Assert.AreEqual(0, clone.ValueTypeField.TestField);
        Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
        Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
        Assert.IsNull(clone.ReferenceTypeField);

        if(isDeepClone) {
          Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
          Assert.AreEqual(0, clone.ReferenceTypeProperty.TestField);
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][0],
            clone.ReferenceTypeArrayProperty[1, 3][0]
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][2],
            clone.ReferenceTypeArrayProperty[1, 3][2]
          );
          Assert.AreEqual(0, clone.ReferenceTypeArrayProperty[1, 3][0].TestField);
          Assert.AreEqual(0, clone.ReferenceTypeArrayProperty[1, 3][2].TestField);
        } else {
          Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
        }
      } else {
        Assert.AreEqual(original.TestField, clone.TestField);
        Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
        Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
        Assert.AreEqual(
          original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
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

        if(isDeepClone) {
          Assert.AreNotSame(original.ReferenceTypeField, clone.ReferenceTypeField);
          Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreNotSame(
            original.ReferenceTypeArrayField, clone.ReferenceTypeArrayField
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][0],
            clone.ReferenceTypeArrayProperty[1, 3][0]
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][2],
            clone.ReferenceTypeArrayProperty[1, 3][2]
          );
          Assert.AreEqual(
            original.ReferenceTypeArrayProperty[1, 3][0].TestField,
            clone.ReferenceTypeArrayProperty[1, 3][0].TestField
          );
          Assert.AreEqual(
            original.ReferenceTypeArrayProperty[1, 3][2].TestField,
            clone.ReferenceTypeArrayProperty[1, 3][2].TestField
          );
        } else {
          Assert.AreSame(original.ReferenceTypeField, clone.ReferenceTypeField);
          Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreSame(
            original.ReferenceTypeArrayField, clone.ReferenceTypeArrayField
          );
          Assert.AreSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
        }
      }

      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeArrayProperty[1, 3][0].TestProperty,
        clone.ReferenceTypeArrayProperty[1, 3][0].TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeArrayProperty[1, 3][2].TestProperty,
        clone.ReferenceTypeArrayProperty[1, 3][2].TestProperty
      );
    }

    /// <summary>
    ///   Verifies that a cloned object exhibits the expected state for the type of
    ///   clone that has been performed
    /// </summary>
    /// <param name="original">Original instance the clone was created from</param>
    /// <param name="clone">Cloned instance that will be checked for correctness</param>
    /// <param name="isDeepClone">Whether the cloned instance is a deep clone</param>
    /// <param name="isPropertyBasedClone">
    ///   Whether a property-based clone was performed
    /// </param>
    private static void verifyClone(
      ref HierarchicalValueType original, ref HierarchicalValueType clone,
      bool isDeepClone, bool isPropertyBasedClone
    ) {
      if(isPropertyBasedClone) {
        Assert.AreEqual(0, clone.TestField);
        Assert.AreEqual(0, clone.ValueTypeField.TestField);
        Assert.AreEqual(0, clone.ValueTypeField.TestProperty);
        Assert.AreEqual(0, clone.ValueTypeProperty.TestField);
        Assert.IsNull(clone.ReferenceTypeField);

        if(isDeepClone) {
          Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
          Assert.AreEqual(0, clone.ReferenceTypeProperty.TestField);
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][0],
            clone.ReferenceTypeArrayProperty[1, 3][0]
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][2],
            clone.ReferenceTypeArrayProperty[1, 3][2]
          );
          Assert.AreEqual(0, clone.ReferenceTypeArrayProperty[1, 3][0].TestField);
          Assert.AreEqual(0, clone.ReferenceTypeArrayProperty[1, 3][2].TestField);
        } else {
          Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
        }
      } else {
        Assert.AreEqual(original.TestField, clone.TestField);
        Assert.AreEqual(original.ValueTypeField.TestField, clone.ValueTypeField.TestField);
        Assert.AreEqual(original.ValueTypeField.TestProperty, clone.ValueTypeField.TestProperty);
        Assert.AreEqual(
          original.ValueTypeProperty.TestField, clone.ValueTypeProperty.TestField
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

        if(isDeepClone) {
          Assert.AreNotSame(original.ReferenceTypeField, clone.ReferenceTypeField);
          Assert.AreNotSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreNotSame(
            original.ReferenceTypeArrayField, clone.ReferenceTypeArrayField
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][0],
            clone.ReferenceTypeArrayProperty[1, 3][0]
          );
          Assert.AreNotSame(
            original.ReferenceTypeArrayProperty[1, 3][2],
            clone.ReferenceTypeArrayProperty[1, 3][2]
          );
          Assert.AreEqual(
            original.ReferenceTypeArrayProperty[1, 3][0].TestField,
            clone.ReferenceTypeArrayProperty[1, 3][0].TestField
          );
          Assert.AreEqual(
            original.ReferenceTypeArrayProperty[1, 3][2].TestField,
            clone.ReferenceTypeArrayProperty[1, 3][2].TestField
          );
        } else {
          Assert.AreSame(original.ReferenceTypeField, clone.ReferenceTypeField);
          Assert.AreSame(original.ReferenceTypeProperty, clone.ReferenceTypeProperty);
          Assert.AreSame(
            original.ReferenceTypeArrayField, clone.ReferenceTypeArrayField
          );
          Assert.AreSame(
            original.ReferenceTypeArrayProperty, clone.ReferenceTypeArrayProperty
          );
        }
      }

      Assert.AreEqual(original.TestProperty, clone.TestProperty);
      Assert.AreEqual(
        original.ValueTypeProperty.TestProperty, clone.ValueTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeProperty.TestProperty, clone.ReferenceTypeProperty.TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeArrayProperty[1, 3][0].TestProperty,
        clone.ReferenceTypeArrayProperty[1, 3][0].TestProperty
      );
      Assert.AreEqual(
        original.ReferenceTypeArrayProperty[1, 3][2].TestProperty,
        clone.ReferenceTypeArrayProperty[1, 3][2].TestProperty
      );
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
