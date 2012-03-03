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
using System.ComponentModel;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit tests for the property change event argument helper</summary>
  [TestFixture]
  internal class PropertyChangedEventArgsHelperTest {

    #region class TestViewModel

    /// <summary>Example class on which unit test generates change notifications</summary>
    public class TestViewModel {

      /// <summary>Example property that will be reported to have changed</summary>
      public int SomeProperty { get; set; }

    }

    #endregion // class TestViewModel

    /// <summary>
    ///   Verifies that a property change notification matching the property
    ///   passed to the AreAffecting() method is recognized
    /// </summary>
    [Test]
    public void MatchingPropertyChangeNotificationIsRecognized() {
      var arguments = new PropertyChangedEventArgs("SomeProperty");
      Assert.IsTrue(arguments.AreAffecting(() => ViewModel.SomeProperty));
      Assert.IsTrue(arguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Ensures that a mismatching property change notification will
    ///   not report the property as being affected.
    /// </summary>
    [Test]
    public void MismatchingPropertyIsReportedAsUnaffected() {
      var arguments = new PropertyChangedEventArgs("AnotherProperty");
      Assert.IsFalse(arguments.AreAffecting(() => ViewModel.SomeProperty));
      Assert.IsFalse(arguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Verifies that any specific property is reported as being affected
    ///   when the property change notification is a null wildcard 
    /// </summary>
    [Test]
    public void SpecificPropertyIsAffectedByNullWildcard() {
      var nullArguments = new PropertyChangedEventArgs(null);
      Assert.IsTrue(nullArguments.AreAffecting(() => ViewModel.SomeProperty));
      Assert.IsTrue(nullArguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Verifies that any specific property is reported as being affected
    ///   when the property change notification is an empty wildcard 
    /// </summary>
    [Test]
    public void SpecificPropertyIsAffectedByEmptyWildcard() {
      var emptyArguments = new PropertyChangedEventArgs(string.Empty);
      Assert.IsTrue(emptyArguments.AreAffecting(() => ViewModel.SomeProperty));
      Assert.IsTrue(emptyArguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Tests whether the helper can recognize a wildcard property change
    ///   notification using null as the wildcard.
    /// </summary>
    [Test]
    public void NullWildcardIsRecognized() {
      var nullArguments = new PropertyChangedEventArgs(null);
      Assert.IsTrue(nullArguments.AffectAllProperties());
    }

    /// <summary>
    ///   Tests whether the helper can recognize a wildcard property change
    ///   notification using an empty string as the wildcard.
    /// </summary>
    [Test]
    public void EmptyWildcardIsRecognized() {
      var emptyArguments = new PropertyChangedEventArgs(string.Empty);
      Assert.IsTrue(emptyArguments.AffectAllProperties());
    }

    /// <summary>Helper used to construct lambda expressions</summary>
    protected static TestViewModel ViewModel { get; set; }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
