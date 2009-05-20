using System;
using System.Collections.Generic;
using System.Reflection;

using Nuclex.Support.Plugins;

namespace Nuclex.Support.Services {

#if false
  /// <summary>
  ///   Inversion of Control container that manages the services of an application
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is a very lightweight and simple inversion of control container that
  ///     relieves components of their duty to track down implementations for the services
  ///     they require to function. It will help with lazy initialization and prevent
  ///     components from becoming cross-linked balls of spaghetti references.
  ///   </para>
  ///   <para>
  ///     Here's a short list of the terms used throughout this container and their
  ///     specific meaning in this context.
  ///   </para>
  ///   <para>
  ///     <list type="bullet">
  ///       <item>
  ///         <term>Service</term>
  ///         <description>
  ///           Defined by an interface (service contract) and provided by a component
  ///           that implements the service contract. A service provides some kind of
  ///           utility to the application, for example it could provide access to
  ///           a data base or allow other components to control certain aspects of
  ///           the application.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Contract</term>
  ///         <description>
  ///           Interface defining the behavior that a service implementation has to
  ///           follow. In order for a component to become a suitable candidate for
  ///           providing a specific service, it has to implement the service contract
  ///           interface and should rigorously follow its specifications.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Component</term>
  ///         <description>
  ///           A component is simply a class that implements one or more service
  ///           contracts. The service manager will created instances of these classes
  ///           when all their dependencies can be provided for and an implementation
  ///           of their service contract is requested.
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  /// </remarks>
  public partial class ServiceManager : IServiceProvider {

    /// <summary>Initializes a new service manager</summary>
    public ServiceManager() {
      this.pluginRepository = new PluginRepository();
    }

    /// <summary>
    ///   Returns all available implementations for the specified contract
    /// </summary>
    /// <param name="completeOnly">
    ///   If true, only services whose dependencies can be completely
    ///   satisfied by the container are returned.
    /// </param>
    /// <returns>
    ///   A new enumerator for the available contract implementations
    /// </returns>
    IEnumerable<ContractType> GetImplementations<ContractType>(bool completeOnly)
      where ContractType : class {
      Type contractType = typeof(ContractType);

      Assembly[] loadedAssemblies = this.pluginRepository.LoadedAssemblies.ToArray();
      for(int index = 0; index < loadedAssemblies.Length; ++index) {
        Type[] assemblyTypes = loadedAssemblies[index].GetTypes();

        for(int typeIndex = 0; typeIndex < assemblyTypes.Length; ++typeIndex) {
          Type checkedType = assemblyTypes[typeIndex];
          if(contractType.IsAssignableFrom(checkedType)) {
            
          }
        }
      }

      yield return null;
    }
    
    private struct CachedContractLookUp {
      public Type[] ValidComponents;
      public int Version;
    }
    private Dictionary<Type, CachedContractLookUp> cachedContracts;
    private int version;

    /// <summary>
    ///   Allows the adjustment of the container's behavior in regard to
    ///   the specified contract
    /// </summary>
    /// <typeparam name="ContractType">
    ///   Contract for which the behavior will be adjusted
    /// </typeparam>
    /// <returns>
    ///   A context object through which the behavior of the container can be
    ///   adjusted for the specified type
    /// </returns>
    public ForContext<ContractType> For<ContractType>() where ContractType : class {
      return new ForContext<ContractType>(this);
    }

    /// <summary>
    ///   Allows the adjustment of the container's behavior in regard to
    ///   the specified contract
    /// </summary>
    /// <param name="contractType">
    ///   Contract for which the behavior will be adjusted
    /// </param>
    /// <returns>
    ///   A context object through which the behavior of the container can be
    ///   adjusted for the specified type
    /// </returns>
    public ForContext For(Type contractType) {
      return new ForContext(this, contractType);
    }

    // Allow Dependency on Container
    //   public Foo(IServiceProvider serviceProvider)
    //   public Foo(IserviceLocator serviceLocator)
    //   public Foo(Container container)
    
    public ContractType GetService<ContractType>() where ContractType : class {
      throw new NotImplementedException();
    }

    /// <summary>Retrieves the service of the specified type</summary>
    /// <param name="contractType">
    ///   Contract for which the service will be retrieved
    /// </param>
    /// <returns>The service for the specified contract</returns>
    object IServiceProvider.GetService(Type contractType) {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Contains all assemblies partaking in the dependency injection scheme
    /// </summary>
    private PluginRepository pluginRepository;

  }
#endif

} // namespace Nuclex.Support.DependencyInjection
