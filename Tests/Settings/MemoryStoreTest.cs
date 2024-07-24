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

namespace Nuclex.Support.Settings {

  /// <summary>Unit tests for the memory settings store</summary>
  [TestFixture]
  internal class MemoryStoreTest {

    /// <summary>Verifies that constructed a memory store throws an exception</summary>
    [Test]
    public void CanBeCreated() {
      Assert.That(() => new MemoryStore(), Throws.Nothing);
    }

    /// <summary>
    ///   Verifies that it's possible to enumerate the options in a non-existing category
    /// </summary>
    [Test]
    public void NonExistingCategoriesCanBeEnumerated() {
      var memoryStore = new MemoryStore();
      Assert.That(memoryStore.EnumerateOptions("doesn't exist"), Is.Empty);
    }

    /// <summary>
    ///   Verifies that accessing an option that doesn't exist throws an exception
    /// </summary>
    [Test]
    public void AccessingNonExistingOptionThrowsException() {
      var memoryStore = new MemoryStore();

      Assert.That(
        () => memoryStore.Get<string>(null, "doesn't exist"),
        Throws.Exception.AssignableTo<KeyNotFoundException>()
      );
    }

    /// <summary>
    ///   Verifies that accessing a category that doesn't exist throws an exception
    /// </summary>
    [Test]
    public void AccessingNonExistingCategoryThrowsException() {
      var memoryStore = new MemoryStore();
      memoryStore.Set<string>(null, "test", "123");

      Assert.That(
        () => memoryStore.Get<string>("doesn't exist", "test"),
        Throws.Exception.AssignableTo<KeyNotFoundException>()
      );
    }

    /// <summary>
    ///   Verifies that settings can be stored in the memory store
    /// </summary>
    [Test]
    public void SettingsCanBeAssignedAndRetrieved() {
      var memoryStore = new MemoryStore();
      memoryStore.Set<string>("general", "sol", "42");

      Assert.That(memoryStore.Get<string>("general", "sol"), Is.EqualTo("42"));
    }

    /// <summary>
    ///   Verifies that it's possible to remove options from the memory store
    /// </summary>
    [Test]
    public void OptionsCanBeRemoved() {
      var memoryStore = new MemoryStore();
      memoryStore.Set<string>(null, "test", null);

      Assert.That(memoryStore.Remove(null, "test"), Is.True);
      
      string value;
      Assert.That(memoryStore.TryGet<string>(null, "test", out value), Is.False);
    }

    /// <summary>
    ///   Verifies that it's not an error to remove an option from a non-existing category
    /// </summary>
    [Test]
    public void CanRemoveOptionFromNonExistingCategory() {
      var memoryStore = new MemoryStore();
      Assert.That(memoryStore.Remove("nothing", "first"), Is.False);
    }

    /// <summary>
    ///   Verifies that it's not an error to remove a non-existing option
    /// </summary>
    [Test]
    public void CanRemoveNonExistingOption() {
      var memoryStore = new MemoryStore();
      Assert.That(memoryStore.Remove(null, "first"), Is.False);
    }

    /// <summary>
    ///   Verifies that the root category is not part of the enumerated categories
    /// </summary>
    [Test]
    public void RootCategoryIsNotEnumerated() {
      var memoryStore = new MemoryStore();
      Assert.That(memoryStore.EnumerateCategories(), Is.Empty);
    }

    /// <summary>
    ///   Verifies that the root category is not part of the enumerated categories
    /// </summary>
    [Test]
    public void OptionsInRootCategoryCanBeEnumerated() {
      var memoryStore = new MemoryStore();

      string[] optionNames = new string[] { "first", "second" };
      memoryStore.Set<int>(null, optionNames[0], 1);
      memoryStore.Set<int>(null, optionNames[1], 2);

      var optionInfos = new List<OptionInfo>(memoryStore.EnumerateOptions());
      Assert.That(optionInfos.Count, Is.EqualTo(2));

      var enumeratedOptionNames = new List<string>() {
        optionInfos[0].Name, optionInfos[1].Name
      };
      Assert.That(enumeratedOptionNames, Is.EquivalentTo(optionNames));
    }

    /// <summary>
    ///   Verifies that the root category is not part of the enumerated categories
    /// </summary>
    [Test]
    public void CategoriesCanBeCreated() {
      var memoryStore = new MemoryStore();

      memoryStore.Set<string>(null, "not", "used");
      memoryStore.Set<string>("test", "message", "hello world");

      Assert.That(memoryStore.EnumerateCategories(), Is.EquivalentTo(new string[] { "test" }));
    }

  }

} // namespace Nuclex.Support.Settings

#endif // UNITTEST
