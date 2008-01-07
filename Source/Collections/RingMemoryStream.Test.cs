#region CPL License
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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the ring buffer class</summary>
  [TestFixture]
  public class RingMemoryStreamTest {

    /// <summary>Prepares some test data for the units test methods</summary>
    [TestFixtureSetUp]
    public void Setup() {
      this.testBytes = new byte[20];
      for(int i = 0; i < 20; ++i)
        this.testBytes[i] = (byte)i;
    }

    /// <summary>
    ///   Ensures that the ring buffer blocks write attempts that would exceed its capacity
    /// </summary>
    [Test, ExpectedException(typeof(OverflowException))]
    public void TestTooLargeChunk() {
      new RingMemoryStream(10).Write(this.testBytes, 0, 11);
    }

    /// <summary>
    ///   Ensures that the ring buffer still accepts write attempts that would fill the
    ///   entire buffer in one go.
    /// </summary>
    [Test]
    public void TestBarelyFittingChunk() {
      new RingMemoryStream(10).Write(this.testBytes, 0, 10);
    }

    /// <summary>Tests whether the ring buffer correctly handles fragmentation</summary>
    [Test]
    public void TestSplitBlockRead() {
      RingMemoryStream rms = new RingMemoryStream(10);
      rms.Write(this.testBytes, 0, 10);
      rms.Read(this.testBytes, 0, 5);
      rms.Write(this.testBytes, 0, 5);

      byte[] actual = new byte[10];
      rms.Read(actual, 0, 10);
      Assert.AreEqual(new byte[] { 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 }, actual);
    }

    /// <summary>
    ///   Tests whether the ring buffer correctly returns partial data if more
    ///   data is requested than is contained in it.
    /// </summary>
    [Test]
    public void TestEndOfStream() {
      byte[] temp = new byte[10];

      RingMemoryStream rms = new RingMemoryStream(10);
      Assert.AreEqual(0, rms.Read(temp, 0, 5));

      rms.Write(this.testBytes, 0, 5);
      Assert.AreEqual(5, rms.Read(temp, 0, 10));

      rms.Write(this.testBytes, 0, 6);
      rms.Read(temp, 0, 5);
      rms.Write(this.testBytes, 0, 9);
      Assert.AreEqual(10, rms.Read(temp, 0, 20));
    }

    /// <summary>
    ///   Validates that the ring buffer can extend its capacity without loosing data
    /// </summary>
    [Test]
    public void TestCapacityIncrease() {
      RingMemoryStream rms = new RingMemoryStream(10);
      rms.Write(this.testBytes, 0, 10);

      rms.Capacity = 20;
      byte[] actual = new byte[10];
      rms.Read(actual, 0, 10);

      Assert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, actual);
    }

    /// <summary>
    ///   Validates that the ring buffer can reduce its capacity without loosing data
    /// </summary>
    [Test]
    public void TestCapacityDecrease() {
      RingMemoryStream rms = new RingMemoryStream(20);
      rms.Write(this.testBytes, 0, 10);

      rms.Capacity = 10;
      byte[] actual = new byte[10];
      rms.Read(actual, 0, 10);

      Assert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, actual);
    }

    /// <summary>
    ///   Checks that an exception is thrown when the ring buffer's capacity is
    ///   reduced so much it would have to give up some of its contained data
    /// </summary>
    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestCapacityDecreaseException() {
      RingMemoryStream rms = new RingMemoryStream(20);
      rms.Write(this.testBytes, 0, 20);

      rms.Capacity = 10;
    }

    /// <summary>
    ///   Tests whether the auto reset feature works (resets the buffer pointer to the
    ///   left end of the buffer when it gets empty; mainly a performance feature).
    /// </summary>
    [Test]
    public void TestAutoReset() {
      RingMemoryStream rms = new RingMemoryStream(10);

      byte[] temp = new byte[10];

      rms.Write(this.testBytes, 0, 8);
      rms.Read(temp, 0, 2);
      rms.Read(temp, 0, 2);
      rms.Read(temp, 0, 1);
      rms.Read(temp, 0, 1);

      Assert.AreEqual(2, rms.Length);
    }

    /// <summary>Test data for the ring buffer unit tests</summary>
    private byte[] testBytes;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
