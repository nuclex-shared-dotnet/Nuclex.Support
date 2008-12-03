﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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

using System;
using System.IO;
using System.Reflection;

#if UNITTEST

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using NMock2;

namespace Nuclex.Support.Plugins {

  /// <summary>Unit Test for the plugin repository class</summary>
  [TestFixture]
  public class PluginRepositoryTest {

    #region interface IAssemblyLoadedSubscriber

    /// <summary>Interface used to test the progress tracker</summary>
    public interface IAssemblyLoadedSubscriber {

      /// <summary>
      ///   Represents the method that handles the System.AppDomain.AssemblyLoad event
      ///   of an System.AppDomain
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="arguments">
      ///   An System.AssemblyLoadEventArgs that contains the event data
      /// </param>
      void AssemblyLoaded(object sender, AssemblyLoadEventArgs arguments);

    }

    #endregion // interface IProgressTrackerSubscriber

    /// <summary>
    ///   Tests whether the default constructor of the plugin repository class works
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      new PluginRepository();
    }

    /// <summary>
    ///   Tests whether the AddFiles() method accepts a file mask to which there are
    ///   no matching files
    /// </summary>
    [Test]
    public void TestAddFilesWithZeroMatches() {
      PluginRepository testRepository = new PluginRepository();
      testRepository.AddFiles(Guid.NewGuid().ToString());
    }

    /// <summary>
    ///   Tests whether the AddFiles() method accepts a file mask to which there is
    ///   exactly one matching file
    /// </summary>
    [Test]
    public void TestAddFilesWithOwnAssembly() {
      PluginRepository testRepository = new PluginRepository();

      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddFiles(self.Location);
    }

    /// <summary>
    ///   Tests whether the AddAssembly() method works by adding the test assembly
    ///   itself to the repository
    /// </summary>
    [Test]
    public void TestAddAssembly() {
      PluginRepository testRepository = new PluginRepository();

      // Might also use Assembly.GetCallingAssembly() here, but this leads to the exe of
      // the unit testing tool
      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      Assert.AreEqual(1, testRepository.LoadedAssemblies.Count);
    }

    /// <summary>
    ///   Tests whether the AddAssembly() method works by adding the test assembly
    ///   itself to the repository
    /// </summary>
    [Test]
    public void TestAssemblyLoadedEvent() {
      Mockery mockery = new Mockery();

      PluginRepository testRepository = new PluginRepository();
      IAssemblyLoadedSubscriber subscriber = mockSubscriber(mockery, testRepository);

      Expect.Once.On(subscriber).Method("AssemblyLoaded").WithAnyArguments();

      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of a plugin repository</summary>
    /// <param name="mockery">Mockery to create an event subscriber in</param>
    /// <param name="repository">Repository to subscribe the mocked subscriber to</param>
    /// <returns>The mocked event subscriber</returns>
    private static IAssemblyLoadedSubscriber mockSubscriber(
      Mockery mockery, PluginRepository repository
    ) {
      IAssemblyLoadedSubscriber mockedSubscriber =
        mockery.NewMock<IAssemblyLoadedSubscriber>();

      repository.AssemblyLoaded += new AssemblyLoadEventHandler(
        mockedSubscriber.AssemblyLoaded
      );

      return mockedSubscriber;
    }

  }

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
