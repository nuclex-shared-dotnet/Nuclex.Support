#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2013 Nuclex Development Labs

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
using NMock;

namespace Nuclex.Support.Collections {

  /// <summary>Unit tests for the multi dictionary</summary>
  [TestFixture]
  internal class MultiDictionaryTest {

    /// <summary>
    ///   Verifies that new instances of the multi dictionary can be created
    /// </summary>
    [Test]
    public void CanConstructNewDictionary() {
      var dictionary = new MultiDictionary<int, string>();
      Assert.IsNotNull(dictionary); // nonsense, prevents compiler warning
    }

    /// <summary>
    ///   Verifies that the count is initialized correctly when building
    ///   a multi dictionary from a dictionary of value collections.
    /// </summary>
    [Test]
    public void CountIsCalculatedIfInitializedFromDictionary() {
      var contents = new Dictionary<int, ICollection<string>>();
      contents.Add(1, new List<string>(new string[] { "one", "eins" }));
      contents.Add(2, new List<string>(new string[] { "two", "zwei" }));

      var multiDictionary = new MultiDictionary<int, string>(contents);
      Assert.AreEqual(4, multiDictionary.Count);
    }

    /// <summary>
    ///   Verifies that a new multi dictionary based on a read-only dictionary is
    ///   also read-only
    /// </summary>
    [Test]
    public void IsReadOnlyWhenBasedOnReadOnlyContainer() {
      var readOnly = new ReadOnlyDictionary<int, ICollection<string>>(
        new Dictionary<int, ICollection<string>>()
      );
      var dictionary = new MultiDictionary<int, string>(readOnly);

      Assert.IsTrue(dictionary.IsReadOnly);
    }

    /// <summary>
    ///   Ensures that the multi dictionary can contain the same key multiple times
    ///   (or in other words, multiple values on the same key)
    /// </summary>
    [Test]
    public void CanContainKeyMultipleTimes() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(123, "one two three");
      dictionary.Add(123, "eins zwei drei");

      Assert.AreEqual(2, dictionary.Count);

      CollectionAssert.AreEquivalent(
        new KeyValuePair<int, string>[] {
          new KeyValuePair<int, string>(123, "one two three"),
          new KeyValuePair<int, string>(123, "eins zwei drei")
        },
        dictionary
      );
    }

    /// <summary>
    ///   Verifies that adding values through the indexer still updates the item count
    /// </summary>
    [Test]
    public void AddingValuesFromIndexerUpdatesCount() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(42, "the answer to everything");
      dictionary[42].Add("21x2");

      Assert.AreEqual(2, dictionary.Count);

      CollectionAssert.AreEquivalent(
        new KeyValuePair<int, string>[] {
          new KeyValuePair<int, string>(42, "the answer to everything"),
          new KeyValuePair<int, string>(42, "21x2")
        },
        dictionary
      );
    }

    /// <summary>
    ///   Tests whether the collection can count the number of values stored
    ///   under a key
    /// </summary>
    [Test]
    public void ValuesWithSameKeyCanBeCounted() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(20, "twenty");
      dictionary.Add(30, "thirty");
      dictionary.Add(10, "zehn");
      dictionary.Add(20, "zwanzig");
      dictionary.Add(10, "dix");

      Assert.AreEqual(6, dictionary.Count);
      Assert.AreEqual(3, dictionary.CountValues(10));
      Assert.AreEqual(2, dictionary.CountValues(20));
      Assert.AreEqual(1, dictionary.CountValues(30));
    }

    /// <summary>
    ///   Verifies that counting the values of a non-existing key returns 0
    /// </summary>
    [Test]
    public void CountingValuesOfNonExistentKeyReturnsNull() {
      var dictionary = new MultiDictionary<int, string>();
      Assert.AreEqual(0, dictionary.CountValues(1));
    }

    /// <summary>
    ///   Ensures that its possible to remove values individually without affecting
    ///   other values stored under the same key
    /// </summary>
    [Test]
    public void ValuesCanBeRemovedIndividually() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      dictionary.Add(10, "dix");

      dictionary.Remove(10, "zehn");

      Assert.AreEqual(2, dictionary.Count);
      CollectionAssert.AreEquivalent(
        new KeyValuePair<int, string>[] {
          new KeyValuePair<int, string>(10, "ten"),
          new KeyValuePair<int, string>(10, "dix")
        },
        dictionary
      );
    }

    /// <summary>
    ///   Verifies that the Count property returns the number of unique keys if it is called
    ///   on the collection-of-collections interface implemented by the multi dictionary
    /// </summary>
    [Test]
    public void CollectionOfCollectionCountIsUniqueKeyCount() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");

      Assert.AreEqual(2, dictionary.Count);
      var collectionOfCollections =
        (ICollection<KeyValuePair<int, ICollection<string>>>)dictionary;
      Assert.AreEqual(1, collectionOfCollections.Count);
    }

    /// <summary>
    ///   Verifies that the multi dictionary can be tested for containment of a specific value
    /// </summary>
    [Test]
    public void ContainmentCanBeTested() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");

      Assert.IsTrue(dictionary.Contains(new KeyValuePair<int, string>(10, "ten")));
      Assert.IsTrue(dictionary.Contains(new KeyValuePair<int, string>(10, "zehn")));
      Assert.IsFalse(dictionary.Contains(new KeyValuePair<int, string>(10, "dix")));
      Assert.IsFalse(dictionary.Contains(new KeyValuePair<int, string>(20, "ten")));
    }

    /// <summary>
    ///   Verifies that the multi dictionary can be tested for containment of a specific key
    /// </summary>
    [Test]
    public void KeyContainmentCanBeTested() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");

      Assert.IsTrue(dictionary.ContainsKey(10));
      Assert.IsFalse(dictionary.ContainsKey(20));
    }

    /// <summary>
    ///   Verifies that the key collection can be retrieved from the dictionary
    /// </summary>
    [Test]
    public void KeyCollectionCanBeRetrieved() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");

      ICollection<int> keys = dictionary.Keys;
      Assert.IsNotNull(keys);
      Assert.AreEqual(1, keys.Count);
    }

    /// <summary>
    ///   Verifies that the key collection can be retrieved from the dictionary
    /// </summary>
    [Test]
    public void ValueCollectionCanBeRetrieved() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      dictionary.Add(20, "twenty");

      ICollection<string> values = dictionary.Values;
      Assert.IsNotNull(values);
      Assert.AreEqual(3, values.Count);
    }

    /// <summary>
    ///   Verifies that TryGetValue() returns false and doesn't throw if a key
    ///   is not found in the collection
    /// </summary>
    [Test]
    public void TryGetValueReturnsFalseOnMissingKey() {
      var dictionary = new MultiDictionary<int, string>();
      ICollection<string> values;
      Assert.IsFalse(dictionary.TryGetValue(123, out values));
    }

    /// <summary>Verifies that keys can be looked up via TryGetValue()</summary>
    [Test]
    public void TryGetValueCanLookUpValues() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      ICollection<string> values;
      Assert.IsTrue(dictionary.TryGetValue(10, out values));
      Assert.AreEqual(2, values.Count);
    }

    /// <summary>
    ///   Verifies that assigning null to a key deletes all the values stored
    ///   under it
    /// </summary>
    [Test]
    public void AssigningNullToKeyRemovesAllValues() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      dictionary.Add(20, "twenty");

      Assert.AreEqual(3, dictionary.Count);
      dictionary[10] = null;
      Assert.AreEqual(1, dictionary.Count);
      Assert.IsFalse(dictionary.ContainsKey(10));
    }

    /// <summary>
    ///   Verifies that assigning null to a key deletes all the values stored
    ///   under it
    /// </summary>
    [Test]
    public void ValueListCanBeAssignedToNewKey() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary[3] = new List<string>() { "three", "drei" };

      Assert.AreEqual(2, dictionary.Count);
      Assert.IsTrue(dictionary.Contains(new KeyValuePair<int, string>(3, "three")));
    }

    /// <summary>
    ///   Verifies that assigning null to a key deletes all the values stored
    ///   under it
    /// </summary>
    [Test]
    public void ValueListCanOverwriteExistingKey() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "dix");

      Assert.AreEqual(1, dictionary.Count);

      dictionary[10] = new List<string>() { "ten", "zehn" };

      Assert.AreEqual(2, dictionary.Count);
      Assert.IsFalse(dictionary.Contains(new KeyValuePair<int, string>(10, "dix")));
      Assert.IsTrue(dictionary.Contains(new KeyValuePair<int, string>(10, "ten")));
    }

    /// <summary>
    ///   Verifies that nothing bad happens when a key is removed from the dictionary
    ///   that it doesn't contain
    /// </summary>
    [Test]
    public void NonExistingKeyCanBeRemoved() {
      var dictionary = new MultiDictionary<int, string>();
      Assert.AreEqual(0, dictionary.RemoveKey(123));
    }

    /// <summary>
    ///   Verifies that the remove method returns the number of values that have
    ///   been removed from the dictionary
    /// </summary>
    [Test]
    public void RemoveReturnsNumberOfValuesRemoved() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      Assert.AreEqual(2, dictionary.RemoveKey(10));
    }

    /// <summary>
    ///   Verifies that the dictionary becomes empty after clearing it
    /// </summary>
    [Test]
    public void DictionaryIsEmptyAfterClear() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(10, "ten");
      dictionary.Add(10, "zehn");
      dictionary.Add(20, "twenty");
      Assert.AreEqual(3, dictionary.Count);
      dictionary.Clear();
      Assert.AreEqual(0, dictionary.Count);
    }

    /// <summary>
    ///   Verifies that non-existing values can be removed from the dictionary
    /// </summary>
    [Test]
    public void NonExistingValueCanBeRemoved() {
      var dictionary = new MultiDictionary<int, string>();
      Assert.IsFalse(dictionary.Remove(123, "test"));
    }

    /// <summary>
    ///   Verifies that nothing bad happens when the last value under a key is removed
    /// </summary>
    [Test]
    public void LastValueOfKeyCanBeRemoved() {
      var dictionary = new MultiDictionary<int, string>();
      dictionary.Add(123, "test");
      dictionary.Remove(123, "test");
      Assert.AreEqual(0, dictionary.CountValues(123));
    }

    /// <summary>
    ///   Verifies that the dictionary can be copied into an array
    /// </summary>
    [Test]
    public void DictionaryCanBeCopiedIntoArray() {
      var expected = new List<KeyValuePair<int, string>>() {
        new KeyValuePair<int, string>(1, "one"),
        new KeyValuePair<int, string>(1, "eins"),
        new KeyValuePair<int, string>(2, "two"),
        new KeyValuePair<int, string>(2, "zwei")
      };

      var dictionary = new MultiDictionary<int, string>();
      foreach(KeyValuePair<int, string> entry in expected) {
        dictionary.Add(entry.Key, entry.Value);
      }

      var actual = new KeyValuePair<int, string>[4];
      dictionary.CopyTo(actual, 0);

      CollectionAssert.AreEquivalent(expected, actual);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
