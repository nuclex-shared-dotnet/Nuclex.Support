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
      #pragma warning disable 0618
      Assert.IsTrue(arguments.AreAffecting(() => ViewModel.SomeProperty));
      #pragma warning restore 0618
      Assert.IsTrue(arguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Ensures that a mismatching property change notification will
    ///   not report the property as being affected.
    /// </summary>
    [Test]
    public void MismatchingPropertyIsReportedAsUnaffected() {
      var arguments = new PropertyChangedEventArgs("AnotherProperty");
      #pragma warning disable 0618
      Assert.IsFalse(arguments.AreAffecting(() => ViewModel.SomeProperty));
      #pragma warning restore 0618
      Assert.IsFalse(arguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Verifies that any specific property is reported as being affected
    ///   when the property change notification is a null wildcard 
    /// </summary>
    [Test]
    public void SpecificPropertyIsAffectedByNullWildcard() {
      var nullArguments = new PropertyChangedEventArgs(null);
      #pragma warning disable 0618
      Assert.IsTrue(nullArguments.AreAffecting(() => ViewModel.SomeProperty));
      #pragma warning restore 0618
      Assert.IsTrue(nullArguments.AreAffecting("SomeProperty"));
    }

    /// <summary>
    ///   Verifies that any specific property is reported as being affected
    ///   when the property change notification is an empty wildcard 
    /// </summary>
    [Test]
    public void SpecificPropertyIsAffectedByEmptyWildcard() {
      var emptyArguments = new PropertyChangedEventArgs(string.Empty);
      #pragma warning disable 0618
      Assert.IsTrue(emptyArguments.AreAffecting(() => ViewModel.SomeProperty));
      #pragma warning disable 0618
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
