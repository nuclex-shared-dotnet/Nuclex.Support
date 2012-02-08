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

  /// <summary>Unit Test for the expression tree-based cloner</summary>
  [TestFixture]
  public class ExpressionTreeClonerTest : CloneFactoryTest {

    /// <summary>Initializes a new unit test suite for the reflection cloner</summary>
    public ExpressionTreeClonerTest() {
      this.cloneFactory = new ExpressionTreeCloner();
    }

    /// <summary>Verifies that clones of primitive types can be created</summary>
    [Test]
    public void PrimitiveTypesCanBeCloned() {
      int original = 12345;
      int clone = this.cloneFactory.DeepClone(original, false);
      Assert.AreEqual(original, clone);
    }

    /// <summary>Verifies that shallow clones of arrays can be made</summary>
    [Test]
    public void ReferenceTypesCanBeCloned() {
      var original = new TestReferenceType() { TestField = 123, TestProperty = 456 };
      TestReferenceType clone = this.cloneFactory.DeepClone(original, false);

      Assert.AreNotSame(original, clone);
      Assert.AreEqual(original.TestField, clone.TestField);
      Assert.AreEqual(original.TestProperty, clone.TestProperty);
    }

    /// <summary>Verifies that shallow clones of arrays can be made</summary>
    [Test]
    public void PrimitiveArraysCanBeCloned() {
      var original = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      int[] clone = this.cloneFactory.DeepClone(original, false);

      Assert.AreNotSame(original, clone);
      CollectionAssert.AreEqual(original, clone);
    }

#if false
    /// <summary>Verifies that shallow clones of arrays can be made</summary>
    [Test]
    public void ShallowClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[] {
        new TestReferenceType() { TestField = 123, TestProperty = 456 }
      };
      TestReferenceType[] clone = this.cloneFactory.ShallowClone(original, false);

      Assert.AreSame(original[0], clone[0]);
    }
#endif

    /// <summary>Verifies that deep clones of arrays can be made</summary>
    [Test]
    public void DeepClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[,] {
        {
          new TestReferenceType() { TestField = 123, TestProperty = 456 }
        }
      };
      TestReferenceType[,] clone = this.cloneFactory.DeepClone(original, false);

      Assert.AreNotSame(original[0, 0], clone[0, 0]);
      Assert.AreEqual(original[0, 0].TestField, clone[0, 0].TestField);
      Assert.AreEqual(original[0, 0].TestProperty, clone[0, 0].TestProperty);
    }

    /// <summary>Verifies that deep clones of a generic list can be made</summary>
    [Test]
    public void GenericListsCanBeCloned() {
      var original = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      List<int> clone = this.cloneFactory.DeepClone(original, false);

      CollectionAssert.AreEqual(original, clone);
    }

    /// <summary>Verifies that deep clones of a generic dictionary can be made</summary>
    [Test]
    public void GenericDictionariesCanBeCloned() {
      var original = new Dictionary<int, string>();
      original.Add(1, "one");
      Dictionary<int, string> clone = this.cloneFactory.DeepClone(original, false);

      Assert.AreEqual("one", clone[1]);
    }

#if false
    /// <summary>
    ///   Verifies that a field-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.ShallowClone(original, false);
      VerifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.ShallowClone(original, false);
      VerifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: false);
    }
#endif
    /// <summary>
    ///   Verifies that a field-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.DeepClone(original, false);
      VerifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.DeepClone(original, false);
      VerifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: false);
    }

#if false
    /// <summary>
    ///   Verifies that a property-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.ShallowClone(original, true);
      VerifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.ShallowClone(original, true);
      VerifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: true);
    }
#endif

#if false
    /// <summary>
    ///   Verifies that a property-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.DeepClone(original, true);
      VerifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.DeepClone(original, true);
      VerifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: true);
    }
#endif

    /// <summary>Clone factory being tested</summary>
    private ICloneFactory cloneFactory;

  }

} // namespace Nuclex.Support.Cloning

#endif // UNITTEST
