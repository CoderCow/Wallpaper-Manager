using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Common.ObjectModel {
  public partial class LightPropertyBindingManager: IDisposable {
    #region Property: Bindings
    /// <summary>
    ///   <inheritdoc cref="Bindings" select='../value/node()' />
    /// </summary>
    private readonly Collection<LightPropertyBinding> bindings;

    /// <summary>
    ///   Gets the collection of registered <see cref="LightPropertyBinding" /> instances.
    /// </summary>
    /// <value>
    ///   The collection of registered <see cref="LightPropertyBinding" /> instances.
    /// </value>
    protected Collection<LightPropertyBinding> Bindings {
      get { return this.bindings; }
    }
    #endregion


    #region Method: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="LightPropertyBinding" /> class used to manage 
    ///   <see cref="LightPropertyBinding" /> objects.
    /// </summary>
    public LightPropertyBindingManager() {
      this.bindings = new Collection<LightPropertyBinding>();
    }
    #endregion

    #region Method: Register
    /// <summary>
    ///   Registeres a new light property binding for the given <see cref="LightBoundProperty" /> objects.
    /// </summary>
    /// <inheritdoc cref="LightPropertyBinding(INotifyPropertyChanged, Object, ICollection{LightBoundProperty})" />
    /// 
    /// <overloads>
    ///   <summary>
    ///     Registers a light property binding.
    ///   </summary>
    /// </overloads>
    public void Register(INotifyPropertyChanged source, Object target, ICollection<LightBoundProperty> properties) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null) throw new ArgumentNullException();
      if (target == null) throw new ArgumentNullException();

      this.bindings.Add(new LightPropertyBinding(source, target, properties));
    }

    /// <summary>
    ///   Registeres a new light property binding for the given property.
    /// </summary>
    /// <inheritdoc />
    /// <param name="property">
    ///   The property to be bound.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The given property is not bindable on the <paramref name="source" /> or <paramref name="target" /> object.
    /// </exception>
    public void Register(INotifyPropertyChanged source, Object target, LightBoundProperty property) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null) throw new ArgumentNullException();
      if (target == null) throw new ArgumentNullException();

      this.Register(source, target, new[] { property });
    }

    /// <summary>
    ///   Registeres a new light property binding for the given property.
    /// </summary>
    /// <inheritdoc />
    /// <param name="propertyName">
    ///   The property name to be bound. Has to be equal on the <paramref name="source" /> and <param name="target" /> object.
    /// </param>
    /// <inheritdoc cref="Register(INotifyPropertyChanged, Object, LightBoundProperty)" select='exception' />
    public void Register(INotifyPropertyChanged source, Object target, String propertyName) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null) throw new ArgumentNullException();
      if (target == null) throw new ArgumentNullException();
      if (propertyName == null) throw new ArgumentNullException();
      if (propertyName.Length == 0) throw new ArgumentOutOfRangeException();

      this.Register(source, target, new[] { new LightBoundProperty(propertyName) });
    }

    /// <summary>
    ///   Registeres a new light property binding for all bindable properties with the same name on a source and target type.
    /// </summary>
    /// <inheritdoc />
    public void Register(INotifyPropertyChanged source, Object target) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null) throw new ArgumentNullException();
      if (target == null) throw new ArgumentNullException();

      this.Register(source, target, (ICollection<LightBoundProperty>)null);
    }
    #endregion

    #region Method: DeregisterAll
    /// <summary>
    ///   Deregisters all properties bound to a specific source and target.
    /// </summary>
    /// <param name="source">
    ///   The source object of the binding. Set to <c>null</c> to unbind all properties bound to <paramref name="target" />.
    /// </param>
    /// <param name="target">
    ///   The destination object of the binding. Set to <c>null</c> to unbind all properties bound to <paramref name="source" />.
    /// </param>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Deregisters all properties bound to a specific source, target or both.
    ///   </summary>
    /// </overloads>
    public void DeregisterAll(INotifyPropertyChanged source, Object target) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null && target == null) throw new ArgumentNullException();

      for (Int32 i = 0; i < this.bindings.Count; i++) {
        LightPropertyBinding propertyBinding = this.bindings[i];

        if (
          (source == null || propertyBinding.Source == source) &&
            (target == null || propertyBinding.Target == target)
          ) {
          this.bindings.RemoveAt(i);
          i--;
        }
      }
    }

    /// <inheritdoc />
    public void DeregisterAll(INotifyPropertyChanged source) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (source == null) throw new ArgumentNullException();
      
      this.DeregisterAll(source, null);
    }

    /// <inheritdoc />
    public void DeregisterAll(Object target) {
      if (this.isDisposed) throw new ObjectDisposedException("this");
      if (target == null) throw new ArgumentNullException();

      this.DeregisterAll(null, target);
    }
    #endregion

    #region IDisposable Implementation
    [ContractPublicPropertyName("IsDisposed")]
    private Boolean isDisposed;

    public Boolean IsDisposed {
      get { return this.isDisposed; }
    }

    protected virtual void Dispose(Boolean disposing) {
      if (!this.isDisposed) {
        if (disposing) {
          this.bindings.Clear();
        }
      }

      this.isDisposed = true;
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~LightPropertyBindingManager() {
      this.Dispose(false);
    }
    #endregion
  }
}