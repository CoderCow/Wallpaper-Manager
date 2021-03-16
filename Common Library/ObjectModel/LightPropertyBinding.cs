using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Windows;

namespace Common.ObjectModel {
  public partial class LightPropertyBindingManager {
    /// <summary>
    ///   Binds data of one or more properties between two objects.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    protected partial class LightPropertyBinding: IWeakEventListener {
      #region Properties: Source, Target
      /// <summary>
      ///   <inheritdoc cref="Source" select='../value/node()' />
      /// </summary>
      private readonly INotifyPropertyChanged source;

      /// <summary>
      ///   Gets the first (or source) object to be synchronized.
      /// </summary>
      /// <value>
      ///   The first (or source) object to be synchronized.
      /// </value>
      public INotifyPropertyChanged Source {
        get { return this.source; }
      }

      /// <summary>
      ///   <inheritdoc cref="Target" select='../value/node()' />
      /// </summary>
      private readonly Object target;

      /// <summary>
      ///   Gets the first (or source) object to be synchronized.
      /// </summary>
      /// <value>
      ///   The first (or source) object to be synchronized.
      /// </value>
      public Object Target {
        get { return this.target; }
      }
      #endregion

      #region Property: PropertyData
      /// <summary>
      ///   <inheritdoc cref="PropertyData" select='../value/node()' />
      /// </summary>
      private readonly List<BoundPropertyData> propertyData;

      /// <summary>
      ///   Gets the collection containing the <see cref="BoundPropertyData" /> instances holding the property data needed for
      ///   the reflection and binding process.
      /// </summary>
      /// <value>
      ///   The collection containing the <see cref="BoundPropertyData" /> instances holding the property data needed for
      ///   the reflection and binding process.
      /// </value>
      /// <seealso cref="LightBoundProperty">LightBoundProperty Structure</seealso>
      protected List<BoundPropertyData> PropertyData {
        get { return this.propertyData; }
      }
      #endregion

      #region Property: IsRegistered
      /// <summary>
      ///   <inheritdoc cref="IsRegistered" select='../value/node()' />
      /// </summary>
      private Boolean isRegistered;

      /// <summary>
      ///   Gets a <see cref="Boolean" /> indicating whether the binding is registered or not.
      /// </summary>
      /// <value>
      ///   Indicates whether the binding is registered or not.
      /// </value>
      protected Boolean IsRegistered {
        get { return this.isRegistered; }
      }
      #endregion


      #region Methods: Constructor, IsPropertyBindable, UpdateTargetNow, Deregister
      /// <summary>
      ///   Initializes a new instance of the <see cref="LightPropertyBinding">PropertySynchronizer Class</see>.
      /// </summary>
      /// <param name="source">
      ///   <inheritdoc cref="Source" select='../value/node()' />
      /// </param>
      /// <param name="target">
      ///   <inheritdoc cref="Target" select='../value/node()' />
      /// </param>
      /// <param name="properties">
      ///   The set of <see cref="LightBoundProperty" /> instances describing the properties to be bound. Set to <c>null</c> to
      ///   bind all properties with the same name.
      /// </param>
      /// <exception cref="ArgumentException">
      ///   A given property in the <paramref name="properties" /> is not bindable on the <paramref name="source" /> or 
      ///   <paramref name="target" /> object.
      /// </exception>
      /// <seealso cref="LightBoundProperty">LightBoundProperty Structure</seealso>
      public LightPropertyBinding(INotifyPropertyChanged source, Object target, ICollection<LightBoundProperty> properties) {
        if (source == null) throw new ArgumentNullException();
        if (target == null) throw new ArgumentNullException();

        this.source = source;
        this.target = target;
        this.propertyData = new List<BoundPropertyData>();

        // No properties given? Then we have to reflect them by searching for bindable properties with the same name on the source
        // and target type.
        if (properties == null || properties.Count == 0) {
          PropertyInfo[] targetPropertyInfos = target.GetType().GetProperties();

          foreach (PropertyInfo propertyInfo in source.GetType().GetProperties()) {
            // Check if this property is get-bindable.
            if (this.IsPropertyBindable(propertyInfo, true, false)) {
              // Check if there is a set-bindable property with the same name on the target object.
              foreach (PropertyInfo targetPropertyInfo in targetPropertyInfos) {
                if (targetPropertyInfo.Name == propertyInfo.Name && this.IsPropertyBindable(targetPropertyInfo, false, true)) {
                  this.PropertyData.Add(new BoundPropertyData(propertyInfo, targetPropertyInfo));
                  break;
                }
              }
            }
          }

          // No bindable properties found?
          if (this.PropertyData.Count == 0)
            throw new ArgumentException(
              String.Concat("No bindable properties on the given type found.\nType: ", source.GetType().Name));
        } else {
          // Properties are given, so validate and reflect them.
          PropertyInfo[] sourcePropertyInfos = source.GetType().GetProperties();
          PropertyInfo[] targetPropertyInfos = target.GetType().GetProperties();

          foreach (LightBoundProperty property in properties) {
            PropertyInfo sourceInfo = null;
            PropertyInfo targetInfo = null;

            // Check if get-bindable on the source object.
            foreach (PropertyInfo propertyInfo in sourcePropertyInfos) {
              if (propertyInfo.Name == property.SourceName)
                sourceInfo = propertyInfo;
            }

            // Property not found or not get-bindable on the source object.
            if ((sourceInfo == null) || !(this.IsPropertyBindable(sourceInfo, true, false)))
              throw new ArgumentException(String.Concat(
                "The property is not bindable on the source object. Make sure it exists and has an accessible get accessor.", 
                "\nSource Type: ", source.GetType().Name, "\nProperty Name: ", property.SourceName));

            // Check if set-bindable on the target object.
            foreach (PropertyInfo propertyInfo in targetPropertyInfos) {
              if (propertyInfo.Name == property.TargetName)
                targetInfo = propertyInfo;
            }

            // Property not found or not set-bindable on the target object.
            if ((targetInfo == null) || !(this.IsPropertyBindable(targetInfo, false, true))) {
              throw new ArgumentException(String.Concat(
                "The property is not bindable on the target object. Make sure it exists and has an accessible set accessor.",
                "\nTarget Type: ", target.GetType().Name, "\nProperty Name: ", property.TargetName));
            }

            this.PropertyData.Add(new BoundPropertyData(sourceInfo, targetInfo));
          }
        }

        if (this.PropertyData.Count > 1)
          PropertyChangedEventManager.AddListener(source, this, String.Empty);
        else
          PropertyChangedEventManager.AddListener(source, this, this.PropertyData[0].SourceInfo.Name);

        this.isRegistered = true;
        this.UpdateTargetNow();
      }

      /// <summary>
      ///   Checks whether a properties value can be get or set by reflection.
      /// </summary>
      /// <param name="propertyInfo">
      ///   The reflected <see cref="PropertyInfo" /> instance used to check the property.
      /// </param>
      /// <param name="checkGet">
      ///   A <see cref="Boolean" /> indicating whether the properties Get-Accessor should be checked.
      /// </param>
      /// <param name="checkSet">
      ///   A <see cref="Boolean" /> indicating whether the properties Set-Accessor should be checked.
      /// </param>
      /// <returns>
      ///   A <see cref="Boolean" /> indicating whether the properies value can be get or set by reflection.
      /// </returns>
      /// <seealso cref="PropertyInfo">PropertyInfo Class</seealso>
      protected Boolean IsPropertyBindable(PropertyInfo propertyInfo, Boolean checkGet, Boolean checkSet) {
        if (propertyInfo == null) throw new ArgumentNullException();

        Boolean bindable = false;

        if (checkGet) {
          MethodInfo getAccessor = propertyInfo.GetGetMethod();

          bindable = (
            (propertyInfo.CanRead) && (getAccessor != null) &&
            (getAccessor.IsPublic) && (!getAccessor.IsStatic) && (!getAccessor.IsAbstract) && (!getAccessor.IsGenericMethod));
        }

        if (checkSet) {
          MethodInfo setAccessor = propertyInfo.GetSetMethod();

          bindable = (
            (propertyInfo.CanWrite) && (setAccessor != null) &&
            (setAccessor.IsPublic) && (!setAccessor.IsStatic) && (!setAccessor.IsAbstract) && (!setAccessor.IsGenericMethod));
        }

        return bindable;
      }

      /// <summary>
      ///   Immediately sets all property values for the <see cref="Target" /> object equal to the respective property values of 
      ///   the <see cref="Source" /> object.
      /// </summary>
      /// <inheritdoc select='exception[@cref="InvalidOperationException"]' />
      public void UpdateTargetNow() {
        if (!this.IsRegistered) {
          throw new InvalidOperationException(String.Concat(
            "The property binding has already been deregistered.\nSource Type: ", this.Source.GetType().Name, 
            "\nTarget Type: ", this.Target.GetType().Name
          ));
        }

        // Update with all property bindings.
        foreach (BoundPropertyData propertyBinding in this.PropertyData)
          this.UpdateTargetNow(propertyBinding);
      }

      /// <summary>
      ///   Immediately sets the value of the given property for the <see cref="Target" /> object equal to the respective property 
      ///   value of the <see cref="Source" /> object.
      /// </summary>
      /// <param name="propertyBinding">
      ///   The property binding to synchronize.
      /// </param>
      /// <exception cref="InvalidOperationException">
      ///   This binding is not registered or an error occurred while synchronizing a value.
      /// </exception>
      /// 
      /// <overloads>
      ///   <summary>
      ///     Immediately sets property values for the <see cref="Target" /> object equal to the respective property values of 
      ///     the <see cref="Source" /> object.
      ///   </summary>
      /// </overloads>
      protected void UpdateTargetNow(BoundPropertyData propertyBinding) {
        if (!this.IsRegistered)
          return;

        Object value;
        try {
          value = propertyBinding.SourceInfo.GetValue(this.Source, null);
        } catch (Exception exception) {
          throw new InvalidOperationException(
            String.Concat("Binding of property failed. See inner exception for details.", 
            "\nRelated Type: ", this.Source.GetType().Name, "\nProperty Name: ", propertyBinding.SourceInfo.Name), exception);
        }

        try {
          propertyBinding.TargetInfo.SetValue(this.Target, value, null);
        } catch (Exception exception) {
          throw new InvalidOperationException(
            String.Concat("Binding of property failed. See inner exception for details.", 
            "\nRelated Type: ", this.Target.GetType().Name, "\nProperty Name: ", propertyBinding.TargetInfo.Name), exception);
        }
      }

      /// <inheritdoc />
      /// <param name="sourcePropertyName">
      ///   The name of the bound property on the <see cref="Source" /> object.
      /// </param>
      /// <exception cref="ArgumentNullException">
      ///   <paramref name="sourcePropertyName" /> is <c>null</c>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   There is no property with the name <paramref name="sourcePropertyName" /> bound on the <see cref="Source" /> object.
      /// </exception>
      public void UpdateTargetNow(String sourcePropertyName) {
        if (!this.IsRegistered) {
          throw new InvalidOperationException(String.Concat(
            "The property binding has already been deregistered.",
            "\nSource Type: ", this.Source.GetType().Name, "\nTarget Type: ", this.Target.GetType().Name));
        }
        if (sourcePropertyName == null)
          throw new ArgumentNullException("sourcePropertyName");

        // Is this property bound at all?
        foreach (BoundPropertyData propertyBinding in this.PropertyData) {
          if (propertyBinding.SourceInfo.Name == sourcePropertyName) {
            this.UpdateTargetNow(propertyBinding);
            return;
          }
        }

        throw new ArgumentException(String.Concat(
          "The property is not bound with the source object.",
          "\nSource Type: ", this.Source.GetType().Name, "Property Name: ", sourcePropertyName));
      }

      /// <summary>
      ///   Deregisters the property binding.
      /// </summary>
      public void Deregister() {
        this.isRegistered = false;
     
        if (this.PropertyData.Count > 1)
          PropertyChangedEventManager.RemoveListener(this.Source, this, String.Empty);
        else
          PropertyChangedEventManager.RemoveListener(this.Source, this, this.PropertyData[0].SourceInfo.Name);
      }
      #endregion

      #region IWeakEventListener Implementation
      /// <inheritdoc />
      public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
        if (managerType == typeof(PropertyChangedEventManager)) {
          if (e is PropertyChangedEventArgs) {
            String propertyName = ((PropertyChangedEventArgs)e).PropertyName;

            // Is this property bound at all?
            foreach (BoundPropertyData propertyBinding in this.PropertyData) {
              if (propertyBinding.SourceInfo.Name == propertyName) {
                this.UpdateTargetNow(propertyBinding);
                break;
              }
            }

            return true;
          }
        }

        return false;
      }
      #endregion
    }
  }
}