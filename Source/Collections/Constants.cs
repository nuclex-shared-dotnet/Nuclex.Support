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
#if !NO_SPECIALIZED_COLLECTIONS
using System.Collections.Specialized;
#endif

namespace Nuclex.Support.Collections {

  /// <summary>Contains fixed constants used by some collections</summary>
  public static class Constants {

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fixed event args used to notify that the collection has reset</summary>
    public static readonly NotifyCollectionChangedEventArgs NotifyCollectionResetEventArgs =
      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#endif

  }

} // namespace Nuclex.Support.Collections
