using System;
using System.Collections.Generic;

using Nuclex.Support.Plugins;

namespace Nuclex.Support.Services {

  /// <summary>Modes in which services can be instantiated</summary>
  public enum Instancing {
    /// <summary>There will only be one service in the whole process</summary>
    Singleton,
    /// <summary>Each thread will be assigned its own service</summary>
    InstancePerThread,
    /// <summary>A new service will be created each time it is queried for</summary>
    Factory
  }

} // namespace Nuclex.Support.DependencyInjection
