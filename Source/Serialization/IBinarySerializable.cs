using System;
using System.Collections.Generic;
using System.IO;

namespace Nuclex.Support.Serialization {

  /// <summary>Interface for objects able to serialize themselfes into a binary format</summary>
  /// <remarks>
  ///   Sometimes, the limitations of XML serialization are too strict, especially
  ///   in the context of a game where you might need to serialize larger chunks of
  ///   binary data or in cases where you do not wish to expose a default constructor
  ///   in your classes. This interface defines two simple methods that can be
  ///   used to load and save an object's state in a simple manner.
  /// </remarks>
  public interface IBinarySerializable {

    /// <summary>Loads the object's state from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the object's state</param>
    void Load(BinaryReader reader);
    
    /// <summary>Save the object's state into a serialized representation</summary>
    /// <param name="writer">Writer to use for writing the object's state</param>
    void Save(BinaryWriter writer);

  }

} // namespace Nuclex.Support.Serialization
