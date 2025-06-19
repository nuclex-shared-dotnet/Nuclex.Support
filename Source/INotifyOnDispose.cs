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
using System.Diagnostics;

namespace Nuclex.Support {

  /// <summary>
  ///   Can be implemented by objects that wish to notify others when they
  ///   are being disposed, allowing for further cleanup work to be attached
  /// </summary>
  /// <remarks>
  ///   This can be useful if an instance has implicit ownership of certain
  ///   resources. For example, a view model may have been provided with
  ///   scoped services via constructor injection. If the view model implements
  ///   this interface, the scope disposal can be attached to the view model
  ///   without requiring intrusive boilerplate code inside the view model.
  /// </remarks>
  public interface INotifyOnDispose {

    /// <summary>Signals that the object is about to be disposed</summary>
    /// <remarks>
    ///   This should be called as the last thing in a Dispose() method and when
    ///   all objects of which the instance has explicit ownership are already
    ///   disposed and gone.
    /// </remarks>
    event EventHandler Disposing;

  }

} // namespace Nuclex.Support
