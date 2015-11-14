// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Windows;
using Common;
using Common.IO;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Contains wallpaper category related data and represents a collection of <see cref="Wallpaper" /> objects.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This collection observes all containing <see cref="Wallpaper" /> objects for changes of their
  ///     <see cref="IWallpaperBase.IsActivated" /> property and updates its own <see cref="IsActivated" /> property
  ///     related to them.
  ///   </para>
  ///   <para>
  ///     <commondoc select='ObservableCollections/General/remarks/node()' />
  ///   </para>
  /// </remarks>
  /// <seealso cref="IWallpaper">Wallpaper Interface</seealso>
  public class WallpaperCategory : ValidatableBase, IWallpaperCategory {
    /// <summary>
    ///   Represents the version number contained in the serialization info for backward compatibility.
    /// </summary>
    protected const string DataVersion = "2.0";

    /// <summary>
    ///   Represents the lowest length allowed for a category name.
    /// </summary>
    public const int Name_MinLength = 1;

    /// <summary>
    ///   Represents the highest length allowed for a category name.
    /// </summary>
    public const int Name_MaxLength = 30;

    /// <summary>
    ///   <inheritdoc cref="IsActivated" select="../value/node()" />
    /// </summary>
    private bool? isActivated;

    /// <summary>
    ///   Used to listen for property changes on all wallpapers in this category.
    /// </summary>
    private readonly CollectionPropertyChangedListener<IWallpaper> wallpapersPropertyChangedListener;

    private bool ignoreWallpaperChanges;

    /// <summary>
    ///   Gets an array of characters which are not allowed for category names.
    /// </summary>
    /// <value>
    ///   An array of characters which are not allowed for category names.
    /// </value>
    public static ReadOnlyCollection<char> Name_InvalidChars { get; } = new ReadOnlyCollection<char>(new[] {'\r', '\n', '\t', '\b', '\a', '\v', '\f', '\x7F', '[', ']'});

    /// <inheritdoc />
    [DoNotNotify]
    public bool? IsActivated {
      get { return this.isActivated; }
      set {
        Contract.Requires<ArgumentNullException>(value != null);

        try {
          this.ignoreWallpaperChanges = true;
          foreach (IWallpaper wallpaper in this.Wallpapers)
            wallpaper.IsActivated = value.Value;
        } finally {
          this.ignoreWallpaperChanges = false;
        }

        this.isActivated = value;
        this.OnPropertyChanged(nameof(this.IsActivated));
      }
    }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public IWallpaperDefaultSettings WallpaperDefaultSettings { get; set; }

    public ObservableCollection<IWallpaper> Wallpapers { get; }

    public int Count => this.Wallpapers.Count;

    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperCategory" /> class.
    /// </summary>
    /// <param name="name">
    ///   The name of the category.
    /// </param>
    /// <param name="wallpapers">
    ///   A collection of wallpapers which should be added to the category be default.
    ///   Set to <c>null</c> to create an empty category.
    /// </param>
    /// <seealso cref="IWallpaper">Wallpaper Interface</seealso>
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="WallpaperCategory" /> class.
    ///   </summary>
    ///   <seealso cref="IWallpaper">Wallpaper Interface</seealso>
    /// </overloads>
    public WallpaperCategory(string name, WallpaperDefaultSettings defaultSettings, IEnumerable<IWallpaper> wallpapers = null) {
      Contract.Requires<ArgumentNullException>(name != null);
      Contract.Requires<ArgumentNullException>(defaultSettings != null);

      this.Name = name;

      if (wallpapers == null)
        this.Wallpapers = new ObservableCollection<IWallpaper>();
      else
        this.Wallpapers = new ObservableCollection<IWallpaper>(wallpapers);

      this.MeasureActiveStatus();

      this.WallpaperDefaultSettings = defaultSettings;
      this.Wallpapers.CollectionChanged += this.Wallpapers_CollectionChanged;

      this.wallpapersPropertyChangedListener = new CollectionPropertyChangedListener<IWallpaper>(this.Wallpapers);
      this.wallpapersPropertyChangedListener.ItemPropertyChanged += this.Wallpaper_PropertyChanged;
    }

    private void Wallpapers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add: {
          bool? newActivationStatus = this.IsActivated;
          foreach (IWallpaper newWallpaper in e.NewItems) {
            bool collectionWasEmptyBefore = (this.Wallpapers.Count == 1);
            if (collectionWasEmptyBefore)
              newActivationStatus = newWallpaper.IsActivated;
            else if (newActivationStatus != null && newWallpaper.IsActivated != newActivationStatus)
              newActivationStatus = null;
          }

          if (this.IsActivated != newActivationStatus) {
            this.isActivated = newActivationStatus;
            this.OnPropertyChanged(nameof(this.IsActivated));
          }
          
          break;
        }
        case NotifyCollectionChangedAction.Remove: {
          if (this.IsActivated == null || this.Wallpapers.Count == 0)
            this.MeasureActiveStatus();
          
          break;
        }
        case NotifyCollectionChangedAction.Replace: {
          Contract.Assert(e.NewItems.Count == e.OldItems.Count);

          bool isActivatedStatusRefreshRequired = false;
          for (int i = 0; i < e.NewItems.Count; i++) {
            IWallpaper oldWallpaper = (IWallpaper)e.OldItems[i];
            IWallpaper newWallpaper = (IWallpaper)e.NewItems[i];

            if (oldWallpaper.IsActivated != newWallpaper.IsActivated) {
              if (this.IsActivated != null) {
                if (newWallpaper.IsActivated != this.IsActivated) {
                  this.isActivated = null;
                  this.OnPropertyChanged(nameof(this.IsActivated));
                }
              } else {
                isActivatedStatusRefreshRequired = true;
              }
            }
          }
          if (isActivatedStatusRefreshRequired)
            this.MeasureActiveStatus();

          break;
        }
        case NotifyCollectionChangedAction.Reset: {
          this.MeasureActiveStatus();
          
          break;
        }
      }

      if (e.Action != NotifyCollectionChangedAction.Move || e.Action != NotifyCollectionChangedAction.Replace)
        this.OnPropertyChanged(nameof(this.Count));
    }

    private void MeasureActiveStatus() {
      bool? newActivatedStatus;

      if (this.Wallpapers.Count == 0) {
        newActivatedStatus = true;
      } else {
        newActivatedStatus = this.Wallpapers[0].IsActivated;
        foreach (IWallpaper wallpaper in this.Wallpapers.Skip(1)) {
          if (wallpaper.IsActivated != newActivatedStatus.Value) {
            newActivatedStatus = null;
            break;
          }
        }
      }

      if (this.isActivated != newActivatedStatus) {
        this.isActivated = newActivatedStatus;
        this.OnPropertyChanged(nameof(this.IsActivated));
      }
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.Name)) {
        if (this.Name.Length < Name_MinLength)
          return string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooShort"), Name_MinLength);
        else if (this.Name.Length > Name_MaxLength)
          return string.Format(LocalizationManager.GetLocalizedString("Error.Category.NameTooLong"), Name_MaxLength);
        else if (Name_InvalidChars.Any(this.Name.Contains))
          return LocalizationManager.GetLocalizedString("Error.Category.NameContainsInvalidChar");
        else if (this.Name.StartsWith(" ") || this.Name.EndsWith(" "))
          return LocalizationManager.GetLocalizedString("Error.Category.NameStartsOrEndsWithSpace");
      } else if (propertyName == nameof(this.WallpaperDefaultSettings)) {
        if (this.WallpaperDefaultSettings == null)
          return "This field is mandatory.";
      }
      
      return null;
    }
    #endregion

    /// <summary>
    ///   Handles the <see cref="WallpaperBase.PropertyChanged" /> event of a <see cref="IWallpaper" /> object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Wallpaper_PropertyChanged(object sender, CollectionPropertyChangedListener<IWallpaper>.ItemPropertyChangedEventArgs e) {
      if (!this.ignoreWallpaperChanges) {
        IWallpaper wallpaper = e.Item;

        if (e.PropertyName == nameof(wallpaper.IsActivated)) {
          if (this.IsActivated != null) {
            if (this.IsActivated != wallpaper.IsActivated) {
              this.isActivated = null;
              this.OnPropertyChanged("IsActivated");
            }
          } else {
            this.MeasureActiveStatus();
          }
        }
      }
    }

    /// <summary>
    ///   Returns a <see cref="string" /> containing the category's name.
    /// </summary>
    /// <returns>
    ///   A <see cref="string" /> containing the category's name.
    /// </returns>
    public override string ToString() => this.Name;
  }
}