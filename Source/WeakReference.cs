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
using System.Runtime.Serialization;

namespace Nuclex.Support {

  /// <summary>
  ///   Type-safe weak reference, referencing an object while still allowing
  ///   that object to be garbage collected.
  /// </summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class WeakReference<TReferenced> : WeakReference
    where TReferenced : class {

    /// <summary>
    ///   Initializes a new instance of the WeakReference class, referencing
    ///   the specified object.
    /// </summary>
    /// <param name="target">The object to track or null.</param>
    public WeakReference(TReferenced target) :
      base(target) { }

    /// <summary>
    ///   Initializes a new instance of the WeakReference class, referencing
    ///   the specified object optionally using resurrection tracking.
    /// </summary>
    /// <param name="target">An object to track.</param>
    /// <param name="trackResurrection">
    ///   Indicates when to stop tracking the object. If true, the object is tracked
    ///   after finalization; if false, the object is only tracked until finalization.
    /// </param>
    public WeakReference(TReferenced target, bool trackResurrection) :
      base(target, trackResurrection) { }

#if !NO_SERIALIZATION

    /// <summary>
    ///   Initializes a new instance of the WeakReference class, using deserialized
    ///   data from the specified serialization and stream objects.
    /// </summary>
    /// <param name="info">
    ///   An object that holds all the data needed to serialize or deserialize the
    ///   current System.WeakReference object.
    /// </param>
    /// <param name="context">
    ///   (Reserved) Describes the source and destination of the serialized stream
    ///   specified by info.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///   The info parameter is null.
    /// </exception>
    protected WeakReference(SerializationInfo info, StreamingContext context) :
      base(info, context) { }

#endif // !NO_SERIALIZATION

    /// <summary>
    ///   Gets or sets the object (the target) referenced by the current WeakReference
    ///   object.
    /// </summary>
    /// <remarks>
    ///   Is null if the object referenced by the current System.WeakReference object
    ///   has been garbage collected; otherwise, a reference to the object referenced
    ///   by the current System.WeakReference object.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    ///   The reference to the target object is invalid. This can occur if the current
    ///   System.WeakReference object has been finalized
    /// </exception>
    public new TReferenced Target {
      get { return (base.Target as TReferenced); }
      set { base.Target = value; }
    }

  }

} // namespace Nuclex.Support
