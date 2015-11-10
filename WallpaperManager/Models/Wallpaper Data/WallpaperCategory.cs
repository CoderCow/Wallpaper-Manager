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
  ///     <see cref="WallpaperBase.IsActivated" /> property and updates its own <see cref="IsActivated" /> property
  ///     related to them.
  ///   </para>
  ///   <para>
  ///     <commondoc select='ObservableCollections/General/remarks/node()' />
  ///   </para>
  /// </remarks>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
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
    public const int Name_MaxLength = 100;

    /// <summary>
    ///   <inheritdoc cref="IsActivated" select="../value/node()" />
    /// </summary>
    private bool? isActivated;

    /// <summary>
    ///   <inheritdoc cref="WallpaperDefaultSettings" select='../value/node()' />
    /// </summary>
    private WallpaperDefaultSettings wallpaperDefaultSettings;

    /// <summary>
    ///   Used to listen for property changes on all wallpapers in this category.
    /// </summary>
    private readonly CollectionPropertyChangedListener<Wallpaper> wallpapersPropertyChangedListener;

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
          foreach (Wallpaper wallpaper in this.Wallpapers)
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

    public ObservableCollection<Wallpaper> Wallpapers { get; }

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
    /// <exception cref="ArgumentException">
    ///   <paramref name="name" /> contains invalid characters.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="name" /> is a string which's length is lower than 1 or greater than 100 chars.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="WallpaperCategory" /> class.
    ///   </summary>
    ///   <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// </overloads>
    public WallpaperCategory(string name, WallpaperDefaultSettings defaultSettings, IEnumerable<Wallpaper> wallpapers = null) {
      Contract.Requires<ArgumentOutOfRangeException>(name.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength));
      Contract.Requires<ArgumentException>(!WallpaperCategory.Name_InvalidChars.Any(name.Contains));

      this.Name = name;
      if (wallpapers == null)
        this.Wallpapers = new ObservableCollection<Wallpaper>();
      else
        this.Wallpapers = new ObservableCollection<Wallpaper>(wallpapers);
      
      this.Wallpapers.CollectionChanged += this.Wallpapers_CollectionChanged;
      this.wallpaperDefaultSettings = defaultSettings;

      this.wallpapersPropertyChangedListener = new CollectionPropertyChangedListener<Wallpaper>(this.Wallpapers);
      this.wallpapersPropertyChangedListener.ItemPropertyChanged += this.Wallpaper_PropertyChanged;
    }

    private void Wallpapers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add: {
          foreach (Wallpaper newWallpaper in e.NewItems) {
            // TODO
            /*if (newWallpaper.IsBlank) {
              this.WallpaperDefaultSettings.AssignTo(newWallpaper);

              newWallpaper.SuggestIsMultiscreen = this.WallpaperDefaultSettings.AutoDetermineIsMultiscreen;
              newWallpaper.SuggestPlacement = this.WallpaperDefaultSettings.AutoDeterminePlacement;
            }*/
            if (this.IsActivated != null && newWallpaper.IsActivated != this.IsActivated) {
              this.IsActivated = null;
              this.OnPropertyChanged(nameof(this.IsActivated));
            }
          }
          
          break;
        }
        case NotifyCollectionChangedAction.Remove: {
          if (this.IsActivated == null)
            this.MeasureActiveStatus();
          
          break;
        }
        case NotifyCollectionChangedAction.Replace: {
          Contract.Assert(e.NewItems.Count == e.OldItems.Count);

          bool isActivatedStatusRefreshRequired = false;
          for (int i = 0; i < e.NewItems.Count; i++) {
            Wallpaper oldWallpaper = (Wallpaper)e.OldItems[i];
            Wallpaper newWallpaper = (Wallpaper)e.NewItems[i];

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
      bool? newActivatedStatus = null;
      foreach (Wallpaper wallpaper in this.Wallpapers) {
        if (newActivatedStatus != null && newActivatedStatus != wallpaper.IsActivated)
          newActivatedStatus = null;
        else
          newActivatedStatus = wallpaper.IsActivated;
      }

      this.IsActivated = newActivatedStatus;
    }

    #region Overrides of ValidatableBase
    /// <inheritdoc />
    protected override string InvalidatePropertyInternal(string propertyName) {
      if (propertyName == nameof(this.Name))
        if (this.Name == null)
          return "This field is mandatory.";
        else if (this.Name.Length < WallpaperCategory.Name_MinLength)
          return $"The name is too short (minimum is {WallpaperCategory.Name_MinLength}).";
        else if (this.Name.Length > WallpaperCategory.Name_MaxLength)
          return $"The name is too long (maximum is {WallpaperCategory.Name_MaxLength}).";
        else if (WallpaperCategory.Name_InvalidChars.Any(this.Name.Contains))
          return "The name contains an invalid character.";

      else if (propertyName == nameof(this.WallpaperDefaultSettings))
        if (this.WallpaperDefaultSettings == null)
          return "This field is mandatory.";
      
      return null;
    }
    #endregion

    /*/// <summary>
    ///   Gets the index of the first <see cref="Wallpaper">Wallpaper instance</see> with an
    ///   <see cref="Wallpaper.ImagePath" /> value of <paramref name="imagePath" />.
    /// </summary>
    /// <param name="imagePath">
    ///   The path of the image.
    /// </param>
    /// <returns>
    ///   <c>-1</c> if no <see cref="Wallpaper" /> with an <see cref="Wallpaper.ImagePath" /> value of
    ///   <paramref name="imagePath" /> can be found; otherwise the zero-based index of the first matching
    ///   <see cref="Wallpaper" />.
    /// </returns>
    // TODO: Convert into extension method?
    public int IndexOfByImagePath(Path imagePath) {
      for (int i = 0; i < this.Count; i++) {
        if (this[i].ImagePath == imagePath)
          return i;
      }

      return -1;
    }*/

    /// <summary>
    ///   Handles the <see cref="WallpaperBase.PropertyChanged" /> event of a <see cref="Wallpaper" /> object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Wallpaper_PropertyChanged(object sender, CollectionPropertyChangedListener<Wallpaper>.ItemPropertyChangedEventArgs e) {
      if (!this.ignoreWallpaperChanges) {
        Wallpaper wallpaper = e.Item;

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