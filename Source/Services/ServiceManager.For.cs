using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Services {

#if false
  partial class ServiceManager {

    #region class ForContext

    // TODO: Rename to "On" to avoid confusion with concept of for loop?
    /// <summary>Manages the context of the "For" modifier</summary>
    public class ForContext {

      /// <summary>Initializes a new "For" context of the service manager</summary>
      /// <param name="serviceManager">Service manager the context operates on</param>
      /// <param name="contractType">Contract that is being modified</param>
      internal ForContext(ServiceManager serviceManager, Type contractType) {
        this.serviceManager = serviceManager;
        this.contractType = contractType;
      }

      /// <summary>Uses the specified implementation for the contract</summary>
      /// <param name="contractImplementation">
      ///   Implementation that will be used for the contract
      /// </param>
      public void Use(object contractImplementation) { }

      /// <summary>
      ///   Uses the provided object as a prototype for the contract implementation
      /// </summary>
      /// <param name="contractImplementationPrototype">
      ///   Contract implementation that will be used as a prototype
      /// </param>
      public void UsePrototype(object contractImplementationPrototype) { }

      /// <summary>Selects the default implementation to use for the contract</summary>
      /// <param name="implementationType">
      ///   Implementation that will be used as the default for the contract
      /// </param>
      public void UseDefault(Type implementationType) { }

      /// <summary>Service manager the "For" context operates on</summary>
      protected ServiceManager serviceManager;
      /// <summary>Contract that is being modified</summary>
      protected Type contractType;

    }

    #endregion // class ForContext

    #region class ForContext<>

    // TODO: Rename to "On" to avoid confusion with concept of for loop?
    /// <summary>Manages the context of the "For" modifier</summary>
    public class ForContext<ContractType> : ForContext {

      /// <summary>Initializes a new "For" context of the service manager</summary>
      /// <param name="serviceManager">Service manager the context operates on</param>
      internal ForContext(ServiceManager serviceManager) :
        base(serviceManager, typeof(ContractType)) { }

      /// <summary>Uses the specified implementation for the contract</summary>
      /// <param name="implementation">
      ///   Implementation that will be used for the contract
      /// </param>
      public void Use(ContractType implementation) { }

      /// <summary>
      ///   Uses the provided object as a prototype for the contract implementation
      /// </summary>
      /// <typeparam name="PrototypeType">
      ///   Type of the implementation that will be used as a prototype
      /// </typeparam>
      /// <param name="contractImplementationPrototype">
      ///   Contract implementation that will be used as a prototype
      /// </param>
      public void UsePrototype<PrototypeType>(PrototypeType contractImplementationPrototype)
        where PrototypeType : ContractType, ICloneable { }

      /// <summary>Selects the default implementation to use for the contract</summary>
      /// <typeparam name="ImplementationType">
      ///   Implementation that will be used as the default for the contract
      /// </typeparam>
      public void UseDefault<ImplementationType>()
        where ImplementationType : ContractType { }

    }

    #endregion // class ForContext<>

  }
#endif

} // namespace Nuclex.Support.DependencyInjection
