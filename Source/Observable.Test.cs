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
using NMock;

namespace Nuclex.Support {

  /// <summary>Unit tests for observable class</summary>
  [TestFixture]
  internal class ObservableTest {

    #region class TestObservable

    /// <summary>Example class on which unit test generates change notifications</summary>
    public class TestObservable : Observable {

      /// <summary>Triggers the property changed event for the specified property</summary>
      /// <param name="propertyName">
      ///   Name of the property that will be reported as changed
      /// </param>
      public void FirePropertyChanged(string propertyName) {
        OnPropertyChanged(propertyName);
      }

      /// <summary>Fires the property changed event for the 'SomePropety' property</summary>
      public void FireSomePropertyChanged() {
        OnPropertyChanged(() => SomeProperty);
      }

      /// <summary>Example property that will be reported to have changed</summary>
      public int SomeProperty { get; set; }

    }

    #endregion // class TestObservable

    #region class MockedSubscriber

    /// <summary>Mocked change notification subscriber</summary>
    public class MockedSubscriber {

      /// <summary>Called when the value of a property has changed</summary>
      /// <param name="sender">Object of which a property has changed</param>
      /// <param name="arguments">Contains the name of the changed property</param>
      public void PropertyChanged(object sender, PropertyChangedEventArgs arguments) {
        this.wasNotified = true;
        this.changedPropertyName = arguments.PropertyName;
      }

      /// <summary>Whether the subscriber was notified of a property change</summary>
      public bool WasNotified {
        get { return this.wasNotified; }
      }

      /// <summary>
      ///   Checks whether a change notification for the specified property was received
      /// </summary>
      /// <param name="propertyName">Name of the property that will be checked for</param>
      /// <returns>
      ///   True if a change notification for the specified property was received
      /// </returns>
      public bool WasNotifiedOfChangeTo(string propertyName) {
        if(!this.wasNotified) {
          return false;
        }

        if(string.IsNullOrEmpty(propertyName)) {
          return string.IsNullOrEmpty(this.changedPropertyName);
        }

        return (propertyName == this.changedPropertyName);
      }

      /// <summary>Whether a change notification was received</summary>
      private bool wasNotified;
      /// <summary>Name of the property for which a change notification was received</summary>
      private string changedPropertyName;

    }

    #endregion // class MockedSubscriber

    /// <summary>Called before each unit test is run</summary>
    [SetUp]
    public void Setup() {
      this.testObservable = new TestObservable();
      this.subscriber = new MockedSubscriber();

      this.testObservable.PropertyChanged += this.subscriber.PropertyChanged;
    }

    /// <summary>
    ///   Verifies that the name of the changed property can be specified manually
    ///   when triggering the PropertyChanged event
    /// </summary>
    [Test]
    public void PropertyNameCanBeSpecifiedManually() {
      this.testObservable.FirePropertyChanged("SomeProperty");
      Assert.IsTrue(this.subscriber.WasNotifiedOfChangeTo("SomeProperty"));
    }

#if DEBUG // The check is conditionally performed only in debug mode
    /// <summary>
    ///   Verifies that specifying the name of a property that doesn't exist
    ///   causes an ArgumentException to be thrown
    /// </summary>
    [Test]
    public void SpecifyingInvalidPropertyNameThrowsArgumentException() {
      Assert.Throws<ArgumentException>(
        delegate() { this.testObservable.FirePropertyChanged("DoesntExist"); }
      );
    }
#endif

    /// <summary>
    ///   Verifies that the observable is capable of deducing the name of the property
    ///   from a lambda expression
    /// </summary>
    [Test]
    public void PropertyNameCanBeDeducedFromLambdaExpression() {
      this.testObservable.FireSomePropertyChanged();
      Assert.IsTrue(this.subscriber.WasNotifiedOfChangeTo("SomeProperty"));
    }

    /// <summary>
    ///   Verifies that change notifications for all properties of a type can
    ///   be generated
    /// </summary>
    [Test]
    public void WildcardChangeNotificationsCanBeSent() {
      this.testObservable.FirePropertyChanged(string.Empty);
      Assert.IsTrue(this.subscriber.WasNotifiedOfChangeTo(null));

      this.testObservable.FirePropertyChanged(null);
      Assert.IsTrue(this.subscriber.WasNotifiedOfChangeTo(string.Empty));
    }

    /// <summary>Observable object being tested</summary>
    private TestObservable testObservable;
    /// <summary>Subscriber to the observable object being tested</summary>
    private MockedSubscriber subscriber;

  }

} // namespace Nuclex.Support

#endif // UNITTEST
