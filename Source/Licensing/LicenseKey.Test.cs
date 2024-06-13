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
using System.Collections;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Licensing {

  /// <summary>Unit test for the license key class</summary>
  [TestFixture]
  internal class LicenseKeyTest {

    /// <summary>Tests the default constructor of the license key class</summary>
    [Test]
    public void DefaultConstructorCanBeUsed() {
      Assert.IsNotNull(new LicenseKey()); // Nonsense, prevents compiler warning
    }

    /// <summary>Validates the correct translation of keys to GUIDs and back</summary>
    [Test]
    public void LicenseKeysCanBeConvertedToGuidsAndBack() {
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
    public void LicenseKeysCanBeModified() {

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

    /// <summary>Tests whether license keys can be modified without destroying them</summary>
    [Test]
    public void ParsingInvalidLicenseKeyThrowsArgumentException() {
      Assert.Throws<ArgumentException>(
        delegate() { LicenseKey.Parse("hello world"); }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the indexer of a license key is used
    ///   with an invalid index to retrieve a component of the key
    /// </summary>
    [Test]
    public void ReadingInvalidIndexThrowsIndexOutOfRangeException() {
      LicenseKey key = new LicenseKey();
      Assert.Throws<IndexOutOfRangeException>(
        delegate() { Console.WriteLine(key[-1]); }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the indexer of a license key is used
    ///   with an invalid index to set a component of the key
    /// </summary>
    [Test]
    public void WritingInvalidIndexThrowsIndexOutOfRangeException() {
      LicenseKey key = new LicenseKey();
      Assert.Throws<IndexOutOfRangeException>(
        delegate() { key[-1] = 0; }
      );
    }

    /// <summary>
    ///   Verifies that a license key can be converted into a byte array
    /// </summary>
    [Test]
    public void LicenseKeyCanBeConvertedToByteArray() {
      Guid someGuid = Guid.NewGuid();
      LicenseKey someKey = new LicenseKey(someGuid);

      CollectionAssert.AreEqual(someGuid.ToByteArray(), someKey.ToByteArray());
    }

  }

} // namespace Nuclex.Support.Licensing

#endif // UNITTEST
