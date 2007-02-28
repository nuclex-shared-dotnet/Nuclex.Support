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

    /// <summary>Prepares some test data for the units test methods</summary>
    [TestFixtureSetUp]
    public void Setup() {
    }

    /// <summary>
    ///   Ensures that the ring buffer blocks write attempts that would exceed its capacity
    /// </summary>
    [Test]
    public void TestTooLargeChunk() {
      MemoryStream buffer = new MemoryStream();

      // Fill and save
      {
        List<TestSerializable> serializables = new List<TestSerializable>();
        
        serializables.Add(new TestSerializable());
        serializables.Add(new TestSerializable());
        serializables[0].Dummy = 123;
        serializables[1].Dummy = 456;

        BinarySerializer<TestSerializable>.SaveCollection(
          serializables, new BinaryWriter(buffer)
        );
        buffer.Position = 0;
      }

      // Load and validate
      {
        List<TestSerializable> serializables = new List<TestSerializable>();

        BinarySerializer<TestSerializable>.LoadCollection(
          serializables, new BinaryReader(buffer)
        );
        
        Assert.AreEqual(2, serializables.Count);
        Assert.AreEqual(123, serializables[0].Dummy);
        Assert.AreEqual(456, serializables[1].Dummy);
      }
    }
    
  }

} // namespace Nuclex.Support.Serialization

#endif // UNITTEST