using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Common.Presentation {
  public abstract class LocalizedStringExtensionBase: MarkupExtension {
    #region Attached Property: LocalizationContext
    public static readonly DependencyProperty LocalizationContext = DependencyProperty.RegisterAttached(
      "LocalizationContext", typeof(String), typeof(LocalizedStringExtensionBase), new PropertyMetadata(null)
    );

    public static String GetLocalizationContext(UIElement element) {
      return (String)element.GetValue(LocalizedStringExtensionBase.LocalizationContext);
    }

    public static void SetLocalizationContext(UIElement element, String context) {
      element.SetValue(LocalizedStringExtensionBase.LocalizationContext, context);
    }
    #endregion

    #region Property: EntryName
    private readonly String entryName;

    public String EntryName {
      get { return this.entryName; }
    }
    #endregion

    #region Property: Context
    private String context;

    public String Context {
      get { return this.context; }
      set { this.context = value; }
    }
    #endregion


    protected LocalizedStringExtensionBase(String entryName) {
      this.entryName = entryName;
    }

    public override Object ProvideValue(IServiceProvider serviceProvider) {
      // We prefer using the given Context property.
      String locContext = this.Context;
      if (locContext == null) {
        UIElement currentObject = (serviceProvider.GetService(typeof(IProvideValueTarget)) as UIElement);

        // Try to find the attached property on the closest parent to this object.
        while (currentObject != null) {
          locContext = LocalizedStringExtensionBase.GetLocalizationContext(currentObject);
          if (locContext != null) {
            break;
          }

          currentObject = (LogicalTreeHelper.GetParent(currentObject) as UIElement);
        }

        if (locContext == null) {
          throw new InvalidOperationException("No localization context given at this point.");
        }
      }
      
      return this.ProvideString(locContext, this.entryName);
    }

    protected abstract String ProvideString(String context, String entryName);
  }
}