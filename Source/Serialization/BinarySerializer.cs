using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Nuclex.Support.Serialization;

namespace Nuclex.Support.Serialization {

  /// <summary>Utility class for serialization objects into binary data</summary>
  /// <typeparam name="BinarySerializableType">Data type to be serialized</typeparam>
  public static class BinarySerializer<BinarySerializableType>
    where BinarySerializableType : IBinarySerializable {

    /// <summary>Serializes a collection of binary serializable objects</summary>
    /// <param name="collection">Collection to be serialized</param>
    /// <param name="writer">BinaryWriter to serialize the collection into</param>
    public static void SaveCollection(
      ICollection<BinarySerializableType> collection, BinaryWriter writer
    ) {

      // Save the file format version so the loading routine can detect
      // which version of the file format has to be loaded
      writer.Write((int)1);

      // Serialize all the blueprints in the collection
      writer.Write((int)collection.Count);
      foreach(BinarySerializableType item in collection) {

        // Save the type name of the object so we can recreate it later
        writer.Write(item.GetType().AssemblyQualifiedName);

        // Let the object save its own data
        ((IBinarySerializable)item).Save(writer);

      } // foreach

    }

    /// <summary>Loads a collection from its serialized representation</summary>
    /// <param name="collection">Collection to be deserialized into</param>
    /// <param name="reader">Reader to use for reading the collection</param>
    public static void LoadCollection(
      ICollection<BinarySerializableType> collection, BinaryReader reader
    ) {

      // Read and verify the version of the file format this was saved in
      int version = reader.ReadInt32();
      if(version > 1)
        throw new InvalidOperationException("File format is too new");

      // Read all the serialized blueprints
      int count = reader.ReadInt32();
      for(int index = 0; index < count; ++index) {

        // Try to create an instance from the serialized type name
        BinarySerializableType item = (BinarySerializableType)Activator.CreateInstance(
          Type.GetType(reader.ReadString())
        );

        // Let the blueprint load its own data and add it to the collection
        (item as IBinarySerializable).Load(reader);
        collection.Add(item);

      } // for

    }

  } // class BinarySerializer

} // namespace Nuclex.Support.Serialization
