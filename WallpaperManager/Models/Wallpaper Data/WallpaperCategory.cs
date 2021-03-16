// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  ///     <see cref="WallpaperSettingsBase.IsActivated" /> property and updates its own <see cref="IsActivated" /> property
  ///     related to them.
  ///   </para>
  ///   <para>
  ///     <commondoc select='ObservableCollections/General/remarks/node()' />
  ///   </para>
  /// </remarks>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <seealso cref="IWeakEventListener">IWeakEventListener Interface</seealso>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperCategory : ObservableCollection<Wallpaper>, IWeakEventListener {
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
    ///   A <see cref="bool" /> indicating whether the cached <see cref="isActivated" /> value is currently up to date or not.
    /// </summary>
    private bool isActivatedIsUpToDate;

    /// <summary>
    ///   <inheritdoc cref="Name" select="../value/node()" />
    /// </summary>
    private string name;

    /// <summary>
    ///   <inheritdoc cref="WallpaperDefaultSettings" select='../value/node()' />
    /// </summary>
    private WallpaperDefaultSettings wallpaperDefaultSettings;

    /// <summary>
    ///   Gets an array of characters which are not allowed for category names.
    /// </summary>
    /// <value>
    ///   An array of characters which are not allowed for category names.
    /// </value>
    public static ReadOnlyCollection<char> Name_InvalidChars { get; } = new ReadOnlyCollection<char>(new[] {'\r', '\n', '\t', '\b', '\a', '\v', '\f', '\x7F', '[', ']'});

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether this category and the underlying <see cref="Wallpaper" /> objects
    ///   are activated or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indiciating whether this category and the underlying <see cref="Wallpaper" /> objects are
    ///   activated or not. <c>null</c> if the underlying <see cref="Wallpaper" /> objects have a different activated status.
    /// </value>
    /// <remarks>
    ///   <para>
    ///     The activated status of a category usually indicates if its contained <see cref="Wallpaper" /> objects should be
    ///     automatically cycled or not.
    ///   </para>
    ///   <para>
    ///     Setting this property will also update the <see cref="WallpaperSettingsBase.IsActivated" /> property of all
    ///     underlying <see cref="Wallpaper" /> objects of this collection.
    ///   </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperSettingsBase.IsActivated">WallpaperSettingsBase.IsActivated Property</seealso>
    [DoNotNotify]
    public bool? IsActivated {
      get {
        if (this.Count == 0)
          return false;

        // Do we need to refresh the isActivated value?
        if (!this.isActivatedIsUpToDate) {
          this.isActivated = this[0].IsActivated;

          // Loop through all items except the first one.
          for (int i = 1; i < this.Items.Count; i++) {
            if (this.Items[i].IsActivated != this.isActivated) {
              this.isActivated = null;
              break;
            }
          }

          this.isActivatedIsUpToDate = true;
        }

        return this.isActivated;
      }
      set {
        if (value == null) throw new ArgumentNullException();

        foreach (Wallpaper wallpaper in this.Items)
          wallpaper.IsActivated = value.Value;

        this.isActivated = value;
        this.isActivatedIsUpToDate = true;

        this.OnPropertyChanged("IsActivated");
      }
    }

    /// <summary>
    ///   Gets or sets the name of this category.
    /// </summary>
    /// <value>
    ///   The name of this category.
    /// </value>
    /// <exception cref="ArgumentException">
    ///   Attempted to set a <see cref="string" /> which contains invalid characters. Refer to the
    ///   <see cref="Name_InvalidChars" /> property to get a list of invalid characters for a category name.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a <see cref="string" /> of an invalid length. Refer to the <see cref="Name_MinLength" /> and
    ///   <see cref="Name_MaxLength" /> constants for the respective suitable lengths.
    /// </exception>
    /// <seealso cref="Name_InvalidChars">Name_InvalidChars Property</seealso>
    /// <seealso cref="Name_MinLength">Name_MinLength Constant</seealso>
    /// <seealso cref="Name_MaxLength">Name_MaxLength Constant</seealso>
    [DoNotNotify]
    public string Name {
      get { return this.name; }
      set {
        if (!value.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength)) throw new ArgumentOutOfRangeException();
        if (WallpaperCategory.Name_InvalidChars.Any(value.Contains)) throw new ArgumentException();

        this.name = value;
        this.OnPropertyChanged("Name");
      }
    }

    /// <summary>
    ///   Gets or sets the settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </summary>
    /// <value>
    ///   The settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="Models.WallpaperDefaultSettings">WallpaperDefaultSettings Class</seealso>
    public WallpaperDefaultSettings WallpaperDefaultSettings {
      get { return this.wallpaperDefaultSettings; }
      set {
        this.wallpaperDefaultSettings = value;
        this.OnPropertyChanged("WallpaperDefaultSettings");
      }
    }

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
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="name" /> is <c>null</c>.
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
    public WallpaperCategory(string name, IEnumerable<Wallpaper> wallpapers = null) {
      if (!name.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength)) throw new ArgumentOutOfRangeException();
      if (WallpaperCategory.Name_InvalidChars.Any(name.Contains)) throw new ArgumentException();

      this.name = name;
      if (wallpapers != null) {
        foreach (Wallpaper wallpaper in wallpapers)
          this.Add(wallpaper);
      }

      this.wallpaperDefaultSettings = new WallpaperDefaultSettings();
    }

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        this.Wallpaper_PropertyChanged(sender, (PropertyChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(WallpaperCategory.Name_InvalidChars != null);
      Contract.Invariant(this.Name != null);
      Contract.Invariant(this.Name.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength));
      Contract.Invariant(!WallpaperCategory.Name_InvalidChars.Any(this.Name.Contains));
      Contract.Invariant(this.WallpaperDefaultSettings != null);
    }

    /// <summary>
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
    public int IndexOfByImagePath(Path imagePath) {
      for (int i = 0; i < this.Count; i++) {
        if (this[i].ImagePath == imagePath)
          return i;
      }

      return -1;
    }

    /// <summary>
    ///   Tries to get the <see cref="Wallpaper" /> instance with the specified index.
    /// </summary>
    /// <param name="index">
    ///   A zero-based index.
    /// </param>
    /// <returns>
    ///   <c>null</c> if the index is invalid or the item is <c>null</c>; otherwise the
    ///   <see cref="Wallpaper" /> instance with the given index.
    /// </returns>
    public Wallpaper TryGetItem(int index) {
      if (index >= 0 && index < this.Count)
        return this[index];

      return null;
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, Wallpaper item) {
      // TODO: Throwing this exception is not allowed here.
      if (item == null) throw new ArgumentNullException();

      if (item.IsBlank) {
        this.WallpaperDefaultSettings.AssignTo(item);

        item.SuggestIsMultiscreen = this.WallpaperDefaultSettings.AutoDetermineIsMultiscreen;
        item.SuggestPlacement = this.WallpaperDefaultSettings.AutoDeterminePlacement;
      }
      base.InsertItem(index, item);

      this.UpdateIsActivatedBySingleItem(item);
      PropertyChangedEventManager.AddListener(item, this, string.Empty);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, Wallpaper item) {
      // TODO: Throwing this exception is not allowed here.
      if (item == null) throw new ArgumentNullException();

      Wallpaper oldItem = this.TryGetItem(index);

      base.SetItem(index, item);

      if (oldItem != null)
        PropertyChangedEventManager.RemoveListener(oldItem, this, string.Empty);
      this.UpdateIsActivatedBySingleItem(item);
      PropertyChangedEventManager.AddListener(item, this, string.Empty);
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index) {
      Wallpaper removedItem = this.TryGetItem(index);

      base.RemoveItem(index);

      if (removedItem != null) {
        PropertyChangedEventManager.RemoveListener(removedItem, this, string.Empty);
        this.isActivatedIsUpToDate = false;
        this.OnPropertyChanged("IsActivated");
      }
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      var removedWallpapers = new Wallpaper[this.Count];
      this.CopyTo(removedWallpapers, 0);

      base.ClearItems();

      foreach (Wallpaper wallpaper in removedWallpapers)
        PropertyChangedEventManager.RemoveListener(wallpaper, this, string.Empty);
      this.isActivatedIsUpToDate = false;
      this.OnPropertyChanged("IsActivated");
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperSettingsBase.PropertyChanged" /> event of a <see cref="Wallpaper" /> object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Wallpaper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "IsActivated")
        this.UpdateIsActivatedBySingleItem((Wallpaper)sender);
    }

    /// <summary>
    ///   Updates the <see cref="isActivated" /> and <see cref="isActivatedIsUpToDate" /> fields when a single
    ///   <see cref="Wallpaper" /> has been added, replaced or its <see cref="WallpaperSettingsBase.IsActivated" /> property
    ///   has changed.
    /// </summary>
    /// <param name="item">
    ///   The <see cref="Wallpaper" /> which has been added, replaced or of which the
    ///   <see cref="WallpaperSettingsBase.IsActivated" /> property has changed.
    /// </param>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    private void UpdateIsActivatedBySingleItem(Wallpaper item) {
      if (this.Count == 1) {
        if (this.isActivated != item.IsActivated) {
          this.isActivated = item.IsActivated;
          this.isActivatedIsUpToDate = true;
          this.OnPropertyChanged("IsActivated");
        }
      } else if (this.isActivatedIsUpToDate && (this.isActivated != null) && (this.isActivated != item.IsActivated)) {
        this.isActivated = null;
        this.OnPropertyChanged("IsActivated");
      } else {
        this.isActivatedIsUpToDate = false;
        this.OnPropertyChanged("IsActivated");
      }
    }

    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(string propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///   Returns a <see cref="string" /> containing the category's name.
    /// </summary>
    /// <returns>
    ///   A <see cref="string" /> containing the category's name.
    /// </returns>
    public override string ToString() {
      return this.Name;
    }
  }
}