#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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
using System.Collections;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Licensing {

  /// <summary>Unit test for the license key class</summary>
  [TestFixture]
  public class LicenseKeyTest {

    /// <summary>Validates the correct translation of keys to GUIDs and back</summary>
    [Test]
    public void TestGuidKeyConversion() {

      for(int i = 0; i < 128; ++i) {

        // Create a new BitArray with the n.th bit set
        BitArray guidBits = new BitArray(128);
        guidBits[i] = true;

        // Create a GUID from this Bitarray
        byte[] guidBytes = new byte[16];
        guidBits.CopyTo(guidBytes, 0);
        Guid originalGuid = new Guid(guidBytes);

        // Convert the GUID into a license key and back to a GUID
        string licenseKey = new LicenseKey(originalGuid).ToString();
        Guid rebuiltGuid = LicenseKey.Parse(licenseKey).ToGuid();

        // Verify that the original GUID matches the fore-and-back converted one
        Assert.AreEqual(originalGuid, rebuiltGuid, "Test for GUID bit " + i);

      }

    }

    /// <summary>Tests whether license keys can be modified without destroying them</summary>
    [Test]
    public void TestKeyModification() {

      for(int i = 0; i < 4; ++i) {
        for(int j = 0; j < 8; ++j) {

          LicenseKey testKey = new LicenseKey(
            new Guid(-1, -1, -1, 255, 255, 255, 255, 255, 255, 255, 255)
          );

          string originalString = testKey.ToString();
          testKey[i] &= ~(1 << j);
          string modifiedString = testKey.ToString();

          Assert.IsTrue(
            originalString != modifiedString, "Modified string differs from original"
          );

          testKey[i] |= (1 << j);
          string revertedString = testKey.ToString();

          Assert.AreEqual(
            originalString, revertedString, "Original state restorable"
          );

        } // for j
      } // for i

    }

  }

} // namespace Nuclex.Support.Licensing

#endif // UNITTEST
