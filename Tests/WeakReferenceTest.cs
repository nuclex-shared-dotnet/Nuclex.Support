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

#if UNITTEST

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the strongly typed weak reference class</summary>
  [TestFixture]
  internal class WeakReferenceTest {

    #region class Dummy

    /// <summary>Dummy class for testing the shared instance provider</summary>
    [Serializable]
    private class Dummy {
      /// <summary>Initializes a new dummy</summary>
      public Dummy() { }
    }

    #endregion // class Dummy

    /// <summary>Tests whether the simple constructor works</summary>
    [Test]
    public void TestSimpleConstructor() {
      new WeakReference<Dummy>(new Dummy());
    }

    /// <summary>Test whether the full constructor works</summary>
    [Test]
    public void TestFullConstructor() {
      new WeakReference<Dummy>(new Dummy(), false);
    }

    /// <summary>
    ///   Test whether the target object can be retrieved from the weak reference
    /// </summary>
    [Test]
    public void TestTargetRetrieval() {
      Dummy strongReference = new Dummy();
      WeakReference<Dummy> weakReference = new WeakReference<Dummy>(strongReference);

      // We can not just call GC.Collect() and base our test on the assumption that
      // the garbage collector will actually collect the Dummy instance. This is up
      // to the garbage collector to decide. But we can keep a strong reference in
      // parallel and safely assume that the WeakReference will not be invalidated!
      Assert.AreSame(strongReference, weakReference.Target);
    }

    /// <summary>
    ///   Test whether the target object can be reassigned in the weak reference
    /// </summary>
    [Test]
    public void TestTargetReassignment() {
      Dummy strongReference1 = new Dummy();
      Dummy strongReference2 = new Dummy();
      WeakReference<Dummy> weakReference = new WeakReference<Dummy>(strongReference1);

      Assert.AreSame(strongReference1, weakReference.Target);
      weakReference.Target = strongReference2;
      Assert.AreSame(strongReference2, weakReference.Target);
    }

    /// <summary>
    ///   Test whether the target object can be reassigned in the weak reference
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        WeakReference<Dummy> weakReference1 = new WeakReference<Dummy>(new Dummy());

        formatter.Serialize(memory, weakReference1);
        memory.Position = 0;
        object weakReference2 = formatter.Deserialize(memory);

        // We cannot make any more predictions but for the type of the weak reference.
        // The pointee might have been garbage collected just now or the serializer
        // might have decided not to serialize the pointee at all (which is a valid
        // decision if the serializer found no strong reference to the pointee) in
        // another of the object graph.
        Assert.IsNotNull(weakReference2 as WeakReference<Dummy>);
      }
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
