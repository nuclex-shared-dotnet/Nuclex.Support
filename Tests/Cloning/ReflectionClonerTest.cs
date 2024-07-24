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

using NUnit.Framework;

namespace Nuclex.Support.Cloning {

  /// <summary>Unit Test for the reflection-based cloner</summary>
  [TestFixture]
  internal class ReflectionClonerTest : CloneFactoryTest {

    /// <summary>Initializes a new unit test suite for the reflection cloner</summary>
    public ReflectionClonerTest() {
      this.cloneFactory = new ReflectionCloner();
    }

    /// <summary>Verifies that cloning a null object simply returns null</summary>
    [Test]
    public void CloningNullYieldsNull() {
      Assert.IsNull(this.cloneFactory.DeepFieldClone<object>(null));
      Assert.IsNull(this.cloneFactory.DeepPropertyClone<object>(null));
      Assert.IsNull(this.cloneFactory.ShallowFieldClone<object>(null));
      Assert.IsNull(this.cloneFactory.ShallowPropertyClone<object>(null));
    }

    /// <summary>
    ///   Verifies that clones of objects whose class doesn't possess a default constructor
    ///   can be made
    /// </summary>
    [Test]
    public void ClassWithoutDefaultConstructorCanBeCloned() {
      var original = new ClassWithoutDefaultConstructor(1234);
      ClassWithoutDefaultConstructor clone = this.cloneFactory.DeepFieldClone(original);

      Assert.AreNotSame(original, clone);
      Assert.AreEqual(original.Dummy, clone.Dummy);
    }

    /// <summary>Verifies that clones of primitive types can be created</summary>
    [Test]
    public void PrimitiveTypesCanBeCloned() {
      int original = 12345;
      int clone = this.cloneFactory.ShallowFieldClone(original);
      Assert.AreEqual(original, clone);
    }

    /// <summary>Verifies that shallow clones of arrays can be made</summary>
    [Test]
    public void ShallowClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[] {
        new TestReferenceType() { TestField = 123, TestProperty = 456 }
      };
      TestReferenceType[] clone = this.cloneFactory.ShallowFieldClone(original);

      Assert.AreSame(original[0], clone[0]);
    }

    /// <summary>Verifies that deep clones of arrays can be made</summary>
    [Test]
    public void DeepClonesOfArraysCanBeMade() {
      var original = new TestReferenceType[] {
        new TestReferenceType() { TestField = 123, TestProperty = 456 }
      };
      TestReferenceType[] clone = this.cloneFactory.DeepFieldClone(original);

      Assert.AreNotSame(original[0], clone[0]);
      Assert.AreEqual(original[0].TestField, clone[0].TestField);
      Assert.AreEqual(original[0].TestProperty, clone[0].TestProperty);
    }

    /// <summary>Verifies that deep clones of a generic list can be made</summary>
    [Test]
    public void GenericListsCanBeCloned() {
      var original = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      List<int> clone = this.cloneFactory.DeepFieldClone(original);

      CollectionAssert.AreEqual(original, clone);
    }

    /// <summary>Verifies that deep clones of a generic dictionary can be made</summary>
    [Test]
    public void GenericDictionariesCanBeCloned() {
      var original = new Dictionary<int, string>();
      original.Add(1, "one");
      Dictionary<int, string> clone = this.cloneFactory.DeepFieldClone(original);

      Assert.AreEqual("one", clone[1]);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.ShallowFieldClone(original);
      VerifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.ShallowFieldClone(original);
      VerifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.DeepFieldClone(original);
      VerifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a field-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.DeepFieldClone(original);
      VerifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: false);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a value type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.ShallowPropertyClone(original);
      VerifyClone(ref original, ref clone, isDeepClone: false, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based shallow clone of a reference type can be performed
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.ShallowPropertyClone(original);
      VerifyClone(original, clone, isDeepClone: false, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a value type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfValueTypesCanBeMade() {
      HierarchicalValueType original = CreateValueType();
      HierarchicalValueType clone = this.cloneFactory.DeepPropertyClone(original);
      VerifyClone(ref original, ref clone, isDeepClone: true, isPropertyBasedClone: true);
    }

    /// <summary>
    ///   Verifies that a property-based deep clone of a reference type can be performed
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfReferenceTypesCanBeMade() {
      HierarchicalReferenceType original = CreateReferenceType();
      HierarchicalReferenceType clone = this.cloneFactory.DeepPropertyClone(original);
      VerifyClone(original, clone, isDeepClone: true, isPropertyBasedClone: true);
    }

    /// <summary>Clone factory being tested</summary>
    private ICloneFactory cloneFactory;

  }

} // namespace Nuclex.Support.Cloning
