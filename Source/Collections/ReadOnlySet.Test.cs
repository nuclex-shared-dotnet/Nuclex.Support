#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2017 Nuclex Development Labs

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

#if !NO_SETS

#if UNITTEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;
using NMock;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable set wrapper</summary>
  [TestFixture]
  internal class ReadOnlySetTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.set = new HashSet<int>();
      this.readOnlySet = new ReadOnlySet<int>(this.set);
    }

    /// <summary>
    ///   Verifies that the observable set has a default constructor
    /// </summary>
    [Test]
    public void HasDefaultConstructor() {
      Assert.IsNotNull(new ReadOnlySet<int>(new HashSet<int>()));
    }

    /// <summary>
    ///   Verifies that an exception is thrown upon any attempt to add items
    ///   to a read-only set
    /// </summary>
    [Test]
    public void AddingThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).Add(123); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown upon any attempt to remove items
    ///   from a read-only set
    /// </summary>
    [Test]
    public void RemovingThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).Remove(123); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown upon any attempt to except
    ///   the set with another set
    /// </summary>
    [Test]
    public void ExceptingThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).ExceptWith(null); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown upon any attempt to intersect
    ///   the set with another set
    /// </summary>
    [Test]
    public void InsersectThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).IntersectWith(null); }
      );
    }

    /// <summary>
    ///   Verifies that it's possible to determine whether a set is a proper subset
    ///   or superset of another set
    /// </summary>
    [Test]
    public void CanDetermineProperSubsetAndSuperset() {
      this.set.Add(1);
      this.set.Add(2);
      this.set.Add(3);

      var set2 = new HashSet<int>() { 1, 3 };

      Assert.IsTrue(this.readOnlySet.IsProperSupersetOf(set2));
      Assert.IsTrue(set2.IsProperSubsetOf(this.readOnlySet));

      set2.Add(2);

      Assert.IsFalse(this.readOnlySet.IsProperSupersetOf(set2));
      Assert.IsFalse(set2.IsProperSubsetOf(this.readOnlySet));
    }

    /// <summary>
    ///   Verifies that it's possible to determine whether a set is a subset
    ///   or a superset of another set
    /// </summary>
    [Test]
    public void CanDetermineSubsetAndSuperset() {
      this.set.Add(1);
      this.set.Add(2);
      this.set.Add(3);

      var set2 = new HashSet<int>() { 1, 2, 3 };

      Assert.IsTrue(this.readOnlySet.IsSupersetOf(set2));
      Assert.IsTrue(set2.IsSubsetOf(this.readOnlySet));

      set2.Add(4);

      Assert.IsFalse(this.readOnlySet.IsSupersetOf(set2));
      Assert.IsFalse(set2.IsSubsetOf(this.readOnlySet));
    }

    /// <summary>
    ///   Verifies that a set can determine if another set overlaps with it
    /// </summary>
    [Test]
    public void CanDetermineOverlap() {
      this.set.Add(1);
      this.set.Add(3);
      this.set.Add(5);

      var set2 = new HashSet<int>() { 3 };

      Assert.IsTrue(this.readOnlySet.Overlaps(set2));
      Assert.IsTrue(set2.Overlaps(this.readOnlySet));
    }

    /// <summary>
    ///   Verifies that a set can determine if another set contains the same elements
    /// </summary>
    [Test]
    public void CanDetermineSetEquality() {
      this.set.Add(1);
      this.set.Add(3);
      this.set.Add(5);

      var set2 = new HashSet<int>() { 3, 1, 5 };

      Assert.IsTrue(this.readOnlySet.SetEquals(set2));
      Assert.IsTrue(set2.SetEquals(this.readOnlySet));

      this.set.Add(7);

      Assert.IsFalse(this.readOnlySet.SetEquals(set2));
      Assert.IsFalse(set2.SetEquals(this.readOnlySet));
    }

    /// <summary>
    ///   Verifies that any attempt to symmetrically except a read-only set
    ///   causes an exception
    /// </summary>
    [Test]
    public void SymmetricallyExceptingThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).SymmetricExceptWith(null); }
      );
    }

    /// <summary>
    ///   Verifies that any attempt to union a read-only set causes an exception
    /// </summary>
    [Test]
    public void UnioningThrowsException() {
      Assert.Throws<NotSupportedException>(
        delegate() { ((ISet<int>)this.readOnlySet).UnionWith(null); }
      );
    }

    /// <summary>Set being wrapped in a read-only set</summary>
    private ISet<int> set;
    /// <summary>Read-only wrapper around the set</summary>
    private ReadOnlySet<int> readOnlySet;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST

#endif // !NO_SETS
