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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.Xna.Framework;

namespace Nuclex.Support.Serialization {

  /// <summary>Utility class for serializating objects into binary data</summary>
  public static class BinarySerializer {

    #region System.Collections.Generic.ICollection

    /// <summary>Loads a collection from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the collection</param>
    /// <param name="collection">Collection to be deserialized into</param>
    public static void Load<BinarySerializableType>(
      BinaryReader reader, ICollection<BinarySerializableType> collection
    ) where BinarySerializableType : IBinarySerializable {

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

    /// <summary>Serializes a collection of binary serializable objects</summary>
    /// <param name="writer">BinaryWriter to serialize the collection into</param>
    /// <param name="collection">Collection to be serialized</param>
    public static void Save<BinarySerializableType>(
      BinaryWriter writer, ICollection<BinarySerializableType> collection
    ) where BinarySerializableType : IBinarySerializable {

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

    #endregion // System.Collections.Generic.ICollection

    #region Microsoft.Xna.Framework.Matrix

    /// <summary>Loads a matrix from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the matrix</param>
    /// <param name="matrix">Matrix to be deserialized</param>
    public static void Load(BinaryReader reader, out Matrix matrix) {
      matrix.M11 = reader.ReadSingle();
      matrix.M12 = reader.ReadSingle();
      matrix.M13 = reader.ReadSingle();
      matrix.M14 = reader.ReadSingle();

      matrix.M21 = reader.ReadSingle();
      matrix.M22 = reader.ReadSingle();
      matrix.M23 = reader.ReadSingle();
      matrix.M24 = reader.ReadSingle();

      matrix.M31 = reader.ReadSingle();
      matrix.M32 = reader.ReadSingle();
      matrix.M33 = reader.ReadSingle();
      matrix.M34 = reader.ReadSingle();

      matrix.M41 = reader.ReadSingle();
      matrix.M42 = reader.ReadSingle();
      matrix.M43 = reader.ReadSingle();
      matrix.M44 = reader.ReadSingle();
    }

    /// <summary>Serializes a matrix into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the matrix into</param>
    /// <param name="matrix">Matrix to be serialized</param>
    public static void Save(BinaryWriter writer, ref Matrix matrix) {
      writer.Write(matrix.M11);
      writer.Write(matrix.M12);
      writer.Write(matrix.M13);
      writer.Write(matrix.M14);

      writer.Write(matrix.M21);
      writer.Write(matrix.M22);
      writer.Write(matrix.M23);
      writer.Write(matrix.M24);

      writer.Write(matrix.M31);
      writer.Write(matrix.M32);
      writer.Write(matrix.M33);
      writer.Write(matrix.M34);

      writer.Write(matrix.M41);
      writer.Write(matrix.M42);
      writer.Write(matrix.M43);
      writer.Write(matrix.M44);
    }

    #endregion // Microsoft.Xna.Framework.Matrix

    #region Microsoft.Xna.Framework.Vector2

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector2 vector) {
      vector.X = reader.ReadSingle();
      vector.Y = reader.ReadSingle();
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector2 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
    }

    #endregion // Microsoft.Xna.Framework.Vector2

    #region Microsoft.Xna.Framework.Vector3

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector3 vector) {
      vector.X = reader.ReadSingle();
      vector.Y = reader.ReadSingle();
      vector.Z = reader.ReadSingle();
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector3 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
      writer.Write(vector.Z);
    }

    #endregion // Microsoft.Xna.Framework.Vector3

    #region Microsoft.Xna.Framework.Vector4

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector4 vector) {
      vector.X = reader.ReadSingle();
      vector.Y = reader.ReadSingle();
      vector.Z = reader.ReadSingle();
      vector.W = reader.ReadSingle();
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector4 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
      writer.Write(vector.Z);
      writer.Write(vector.W);
    }

    #endregion // Microsoft.Xna.Framework.Quaternion

    #region Microsoft.Xna.Framework.Quaternion

    /// <summary>Loads a quaternion from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the quaternion</param>
    /// <param name="quaternion">Quaternion to be deserialized</param>
    public static void Load(BinaryReader reader, out Quaternion quaternion) {
      quaternion.X = reader.ReadSingle();
      quaternion.Y = reader.ReadSingle();
      quaternion.Z = reader.ReadSingle();
      quaternion.W = reader.ReadSingle();
    }

    /// <summary>Serializes a quaternion into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the quaternion into</param>
    /// <param name="quaternion">Quaternion to be serialized</param>
    public static void Save(BinaryWriter writer, ref Quaternion quaternion) {
      writer.Write(quaternion.X);
      writer.Write(quaternion.Y);
      writer.Write(quaternion.Z);
      writer.Write(quaternion.W);
    }

    #endregion // Microsoft.Xna.Framework.Quaternion

  } // class BinarySerializer

} // namespace Nuclex.Support.Serialization
