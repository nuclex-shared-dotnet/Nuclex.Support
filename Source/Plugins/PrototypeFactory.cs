using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Plugins {

#if !NO_CLONING

  /// <summary>Factory that creates instances by cloning a prototype</summary>
  /// <typeparam name="TProduct">Type of product created by the factory</typeparam>
  /// <typeparam name="TConcreteType">Type of the prototype that will be cloned</typeparam>
  public class PrototypeFactory<TProduct, TConcreteType> :
    IAbstractFactory<TProduct>, IAbstractFactory, IDisposable
    where TProduct : class
    where TConcreteType : class, ICloneable {

    /// <summary>Initializes a new prototype based factory</summary>
    /// <param name="prototype">Prototype instance that will be cloned</param>
    public PrototypeFactory(TConcreteType prototype) {
      this.prototype = prototype;
    }

    /// <summary>
    ///   Creates a new instance of the type to which the factory is specialized
    /// </summary>
    /// <returns>The newly created instance</returns>
    public TProduct CreateInstance() {
      return (TProduct)this.prototype.Clone();
    }

    /// <summary>
    ///   Creates a new instance of the type to which the factory is specialized
    /// </summary>
    /// <returns>The newly created instance</returns>
    object IAbstractFactory.CreateInstance() {
      return this.prototype.Clone();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.prototype != null) {
        IDisposable disposablePrototype = this.prototype as IDisposable;
        if(disposablePrototype != null) {
          disposablePrototype.Dispose();
        }

        this.prototype = null;
      }
    }

    /// <summary>The prototype object</summary>
    private TConcreteType prototype;

  }

#endif // !NO_CLONING

} // namespace Nuclex.Support.Plugins
