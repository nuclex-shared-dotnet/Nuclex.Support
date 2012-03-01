#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2012 Nuclex Development Labs

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
      var collectionOfCollections = (ICollection<KeyValuePair<int, ICollection<string>>>)dictionary;
      Assert.AreEqual(1, collectionOfCollections.Count);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
