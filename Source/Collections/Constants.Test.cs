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
using System.Collections.Specialized;

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the collection constants</summary>
  [TestFixture]
  internal class ConstantsTest {

#if !NO_SPECIALIZED_COLLECTIONS

    /// <summary>
    ///   Verifies that the collection reset event arguments have 'reset' specified as
    ///   their action
    /// </summary>
    [Test]
    public void CollectionResetEventArgsHaveResetActionSet() {
      Assert.AreEqual(
        NotifyCollectionChangedAction.Reset, Constants.NotifyCollectionResetEventArgs.Action
      );
    }

#endif // !NO_SPECIALIZED_COLLECTIONS

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
