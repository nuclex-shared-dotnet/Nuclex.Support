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
using System.Collections.Generic;
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Serialization {

  /// <summary>Ensures that the binary serializer is working correctly</summary>
  [TestFixture]
  public class BinarySerializerTest {

    private class TestSerializable : IBinarySerializable {
      public void Load(BinaryReader reader) { this.Dummy = reader.ReadInt32(); }
      public void Save(BinaryWriter writer) { writer.Write(this.Dummy); }
      public int Dummy;
    }

    /// <summary>Prepares some test data for the unit test methods</summary>
    [TestFixtureSetUp]
    public void Setup() {
    }

    /// <summary>
    ///   Tests wether a simple collection can be successfully saved and loaded again
    /// </summary>
    [Test]
    public void TestSimpleCollection() {
      MemoryStream buffer = new MemoryStream();

      // Fill and save
      {
        List<TestSerializable> serializables = new List<TestSerializable>();
        
        serializables.Add(new TestSerializable());
        serializables.Add(new TestSerializable());
        serializables[0].Dummy = 123;
        serializables[1].Dummy = 456;

        BinarySerializer.Save(new BinaryWriter(buffer), serializables);
        buffer.Position = 0;
      }

      // Load and validate
      {
        List<TestSerializable> serializables = new List<TestSerializable>();

        BinarySerializer.Load(new BinaryReader(buffer), serializables);
        
        Assert.AreEqual(2, serializables.Count);
        Assert.AreEqual(123, serializables[0].Dummy);
        Assert.AreEqual(456, serializables[1].Dummy);
      }
    }
    
  }

} // namespace Nuclex.Support.Serialization

#endif // UNITTEST