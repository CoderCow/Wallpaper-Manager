using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PropertyChanged;

namespace WallpaperManager.Models {
  [ImplementPropertyChanged]
  public abstract class ValidatableBase: IDataErrorInfo, INotifyPropertyChanged {
    private static readonly ConcurrentDictionary<Type, string[]> cachedPropertyLists = new ConcurrentDictionary<Type, string[]>();
    protected readonly HashSet<string> unvalidatedProperties;

    protected ValidatableBase() {
      this.unvalidatedProperties = new HashSet<string>();

      // First time cache the property list if necessary.
      this.GetPropertyList();
    }

    /// <summary>
    ///   Checks whether the current value of the property exposed by the instance ob this type is causing it to have a valid object status or not.
    /// </summary>
    /// <remarks>
    ///   This method will return <see cref="ValidationResult.ValidResult" /> if the <paramref name="propertyName" /> is not a property exposed by this class.
    /// </remarks>
    /// <param name="propertyName">The name of the property to invalidate.</param>
    /// <returns>A result containing an optional error message.</returns>
    public ValidationResult InvalidateProperty(string propertyName) {
      string errorMessage = this.InvalidatePropertyInternal(propertyName);

      ValidationResult result;
      if (errorMessage == null) {
        this.unvalidatedProperties.Remove(propertyName);
        result = ValidationResult.ValidResult;
      } else {
        result = new ValidationResult(false, errorMessage);
      }

      return result;
    }

    protected virtual string InvalidatePropertyInternal(string propertyName) {
      return null;
    }

    private string[] GetPropertyList() {
      
      Type thisType = this.GetType();
      string[] propertyNames = ValidatableBase.cachedPropertyLists.GetOrAdd(
        thisType, (type) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(prop => prop.Name).ToArray());

      return propertyNames;
    }

    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName) {
      this.unvalidatedProperties.Add(propertyName);

      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Implementation of IDataErrorInfo
    public virtual string Error {
      get {
        string[] propertyList = this.GetPropertyList();
        StringBuilder errorBuilder = new StringBuilder();

        foreach (string propertyName in propertyList) {
          ValidationResult result = this.InvalidateProperty(propertyName);

          if (!result.IsValid) {
            errorBuilder.Append(propertyName);
            errorBuilder.Append(" - ");
            errorBuilder.Append(result.ErrorContent);
            errorBuilder.AppendLine();
          }
        }

        return errorBuilder.ToString();
      }
    }

    public string this[string columnName] {
      get {
        ValidationResult result = this.InvalidateProperty(columnName);

        if (!result.IsValid)
          return result.ErrorContent.ToString();
        else 
          return string.Empty;
      }
    }
    #endregion
  }
}
