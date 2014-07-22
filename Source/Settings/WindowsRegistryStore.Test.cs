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
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Collections.Generic;

namespace Nuclex.Support.Settings {

  /// <summary>Unit tests for the windows registry settings store</summary>
  [TestFixture]
  internal class WindowsRegistryStoreTest {

    #region class TestContext

    /// <summary>Sets up a temporary registry key for the unit test</summary>
    private class TestContext : IDisposable {

      /// <summary>Initializes a new test context</summary>
      public TestContext() {
        this.keyName = Guid.NewGuid().ToString();
        this.registryKey = Registry.CurrentUser.CreateSubKey(this.keyName);
        this.store = new WindowsRegistryStore(this.registryKey, writable: true);
      }

      /// <summary>Immediately frees all resources owned by the test context</summary>
      public void Dispose() {
        if(this.store != null) {
          this.store.Dispose();
          this.store = null;
          this.registryKey = null;
        } else if(this.registryKey != null) {
          this.registryKey.Dispose();
          this.registryKey = null;
        }
        if(this.keyName != null) {
          Registry.CurrentUser.DeleteSubKeyTree(this.keyName);
          this.keyName = null;
        }
      }

      /// <summary>Store created on a temporary registry key</summary>
      public WindowsRegistryStore Store {
        get { return this.store; }
      }

      /// <summary>Name of the temporary registry key</summary>
      private string keyName;
      /// <summary>Registry key (ownership transfered to the store)</summary>
      private RegistryKey registryKey;
      /// <summary>Store that is accessing the registry key</summary>
      private WindowsRegistryStore store;

    }

    #endregion // class TestContext

    /// <summary>Verifies that new instances of the registry store can be created</summary>
    [Test]
    public void CanBeCreated() {
      Assert.That(
        () => { using(var context = new TestContext()) { } }, Throws.Nothing
      );
    }

    /// <summary>Verifies that booleans can be stored in the registry</summary>
    [Test]
    public void BooleansCanBeStored() {
      using(var context = new TestContext()) {
        context.Store.Set(null, "test", true);
        Assert.That(context.Store.Get<bool>(null, "test"), Is.True);

        context.Store.Set(null, "test", false);
        Assert.That(context.Store.Get<bool>(null, "test"), Is.False);
      }
    }

    /// <summary>Verifies that integers can be stored in the registry</summary>
    [Test]
    public void IntegersCanBeStored() {
      using(var context = new TestContext()) {
        context.Store.Set(null, "test", 123);
        Assert.That(context.Store.Get<int>(null, "test"), Is.EqualTo(123));

        context.Store.Set(null, "test", 456);
        Assert.That(context.Store.Get<int>(null, "test"), Is.EqualTo(456));
      }
    }

    /// <summary>Verifies that floats can be stored in the registry</summary>
    [Test]
    public void FloatsCanBeStored() {
      float testValue = float.Parse("123.456", CultureInfo.InvariantCulture);

      using(var context = new TestContext()) {
        context.Store.Set(null, "test", testValue);
        Assert.That(context.Store.Get<float>(null, "test"), Is.EqualTo(testValue));

        testValue = float.Parse("654.321", CultureInfo.InvariantCulture);

        context.Store.Set(null, "test", testValue);
        Assert.That(context.Store.Get<float>(null, "test"), Is.EqualTo(testValue));
      }
    }

    /// <summary>Verifies that strings can be stored in the registry</summary>
    [Test]
    public void StringsCanBeStored() {
      using(var context = new TestContext()) {
        context.Store.Set(null, "test", "hello world");
        Assert.That(context.Store.Get<string>(null, "test"), Is.EqualTo("hello world"));

        context.Store.Set(null, "test", "world hello");
        Assert.That(context.Store.Get<string>(null, "test"), Is.EqualTo("world hello"));
      }
    }

    /// <summary>Verifies that the subkeys of a registry key can be enumerated</summary>
    [Test]
    public void CategoriesCanBeEnumerated() {
      string[] names = new string[] { "one", "two", "three" };

      using(var context = new TestContext()) {
        context.Store.Set(names[0], "sol", 21);
        context.Store.Set(names[1], "sol", 42);
        context.Store.Set(names[2], "sol", 84);

        Assert.That(context.Store.EnumerateCategories(), Is.EquivalentTo(names));
      }
    }

    /// <summary>Verifies that the values under a registry subkey can be enumerated</summary>
    [Test]
    public void OptionsInCategoryCanBeEnumerated() {
      string[] names = new string[] { "one", "two", "three" };

      using(var context = new TestContext()) {
        context.Store.Set("test", names[0], 1);
        context.Store.Set("test", names[1], 2);
        context.Store.Set("test", names[2], 3);

        var optionInfos = new List<OptionInfo>(context.Store.EnumerateOptions("test"));
        Assert.That(optionInfos.Count, Is.EqualTo(3));
      }
    }

    /// <summary>Verifies that the values under a registry key can be enumerated</summary>
    [Test]
    public void RootOptionsCanBeEnumerated() {
      string[] names = new string[] { "one", "two", "three" };

      using(var context = new TestContext()) {
        context.Store.Set(null, names[0], 1);
        context.Store.Set(null, names[1], 2);
        context.Store.Set(null, names[2], 3);

        var optionInfos = new List<OptionInfo>(context.Store.EnumerateOptions(null));
        Assert.That(optionInfos.Count, Is.EqualTo(3));
      }
    }

  }

} // namespace Nuclex.Support.Settings

#endif // UNITTEST
