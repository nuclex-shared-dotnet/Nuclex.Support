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
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the ReverseComparer helper class</summary>
  [TestFixture]
  internal class ReverseComparerTest {

    #region class FortyTwoComparer

    /// <summary>Special comparer in which 42 is larger than everything</summary>
    private class FortyTwoComparer : IComparer<int> {

      /// <summary>Compares the left value to the right value</summary>
      /// <param name="left">Value on the left side</param>
      /// <param name="right">Value on the right side</param>
      /// <returns>The relationship of the two values</returns>
      public int Compare(int left, int right) {

        // Is there a 42 in the arguments?
        if(left == 42) {
          if(right == 42) {
            return 0; // both are equal
          } else {
            return +1; // left is larger
          }
        } else if(right == 42) {
          return -1; // right is larger
        }

        // No 42 encountered, proceed as normal
        return Math.Sign(left - right);

      }

    }

    #endregion // class FortyTwoComparer

    /// <summary>
    ///   Tests whether the default constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      new ReverseComparer<int>();
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      new ReverseComparer<int>(new FortyTwoComparer());
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestReversedDefaultComparer() {
      Comparer<int> comparer = Comparer<int>.Default;
      ReverseComparer<int> reverseComparer = new ReverseComparer<int>(comparer);

      Assert.Greater(0, comparer.Compare(10, 20));
      Assert.Less(0, comparer.Compare(20, 10));

      Assert.Less(0, reverseComparer.Compare(10, 20));
      Assert.Greater(0, reverseComparer.Compare(20, 10));
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestReversedCustomComparer() {
      FortyTwoComparer fortyTwoComparer = new FortyTwoComparer();
      ReverseComparer<int> reverseFortyTwoComparer = new ReverseComparer<int>(
        fortyTwoComparer
      );

      Assert.Less(0, fortyTwoComparer.Compare(42, 84));
      Assert.Greater(0, fortyTwoComparer.Compare(84, 42));

      Assert.Greater(0, reverseFortyTwoComparer.Compare(42, 84));
      Assert.Less(0, reverseFortyTwoComparer.Compare(84, 42));
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
