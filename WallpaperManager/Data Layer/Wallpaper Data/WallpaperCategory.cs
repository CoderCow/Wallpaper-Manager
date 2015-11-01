// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Common;
using Common.IO;

namespace WallpaperManager.Data {
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
  public class WallpaperCategory: ObservableCollection<Wallpaper>, IWeakEventListener {
    #region Constants: Name_MinLength, Name_MaxLength
    /// <summary>
    ///   Represents the version number contained in the serialization info for backward compatibility.
    /// </summary>
    protected const String DataVersion = "2.0";

    /// <summary>
    ///   Represents the lowest length allowed for a category name.
    /// </summary>
    public const Int32 Name_MinLength = 1;
    
    /// <summary>
    ///   Represents the highest length allowed for a category name.
    /// </summary>
    public const Int32 Name_MaxLength = 100;
    #endregion

    #region Static Property: Name_InvalidChars
    /// <summary>
    ///   <inheritdoc cref="Name_InvalidChars" select="../value/node()" />
    /// </summary>
    private static ReadOnlyCollection<Char> name_InvalidChars;

    /// <summary>
    ///   Gets an array of characters which are not allowed for category names.
    /// </summary>
    /// <value>
    ///   An array of characters which are not allowed for category names.
    /// </value>
    public static ReadOnlyCollection<Char> Name_InvalidChars {
      get {
        if (WallpaperCategory.name_InvalidChars == null) {
          WallpaperCategory.name_InvalidChars = new ReadOnlyCollection<char>(
            new[] { '\r', '\n', '\t', '\b', '\a', '\v', '\f', '\x7F', '[', ']' }
          );
        }

        return WallpaperCategory.name_InvalidChars;
      }
    }
    #endregion

    #region Property: IsActivated
    /// <summary>
    ///   A <see cref="Boolean" /> indicating whether the cached <see cref="isActivated" /> value is currently up to date or not.
    /// </summary>
    private Boolean isActivatedIsUpToDate;

    /// <summary>
    ///   <inheritdoc cref="IsActivated" select="../value/node()" />
    /// </summary>
    private Boolean? isActivated;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether this category and the underlying <see cref="Wallpaper" /> objects 
    ///   are activated or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indiciating whether this category and the underlying <see cref="Wallpaper" /> objects are 
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
    public Boolean? IsActivated {
      get {
        if (this.Count == 0) {
          return false;
        }

        // Do we need to refresh the isActivated value?
        if (!this.isActivatedIsUpToDate) {
          this.isActivated = this[0].IsActivated;

          // Loop through all items except the first one.
          for (Int32 i = 1; i < this.Items.Count; i++) {
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
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        foreach (Wallpaper wallpaper in this.Items) {
          wallpaper.IsActivated = value.Value;
        }

        this.isActivated = value;
        this.isActivatedIsUpToDate = true;

        this.OnPropertyChanged("IsActivated");
      }
    }
    #endregion

    #region Property: Name
    /// <summary>
    ///   <inheritdoc cref="Name" select="../value/node()" />
    /// </summary>
    private String name;

    /// <summary>
    ///   Gets or sets the name of this category.
    /// </summary>
    /// <value>
    ///   The name of this category.
    /// </value>
    /// <exception cref="ArgumentException">
    ///   Attempted to set a <see cref="String" /> which contains invalid characters. Refer to the 
    ///   <see cref="Name_InvalidChars" /> property to get a list of invalid characters for a category name.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to set a <see cref="String" /> of an invalid length. Refer to the <see cref="Name_MinLength" /> and
    ///   <see cref="Name_MaxLength" /> constants for the respective suitable lengths.
    /// </exception>
    /// <seealso cref="Name_InvalidChars">Name_InvalidChars Property</seealso>
    /// <seealso cref="Name_MinLength">Name_MinLength Constant</seealso>
    /// <seealso cref="Name_MaxLength">Name_MaxLength Constant</seealso>
    public String Name {
      get { return this.name; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("value"));
        }
        if (!value.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength)) {
          throw new ArgumentOutOfRangeException(ExceptionMessages.GetCategoryNameLengthInvalid(
            value.Length, WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength
          ));
        }
        if (WallpaperCategory.Name_InvalidChars.Any(c => value.Contains(c))) {
          throw new ArgumentException(ExceptionMessages.GetCategoryNameInvalid(value, WallpaperCategory.Name_InvalidChars));
        }

        this.name = value;
        this.OnPropertyChanged("Name");
      }
    }
    #endregion

    #region Property: WallpaperDefaultSettings
    /// <summary>
    ///   <inheritdoc cref="WallpaperDefaultSettings" select='../value/node()' />
    /// </summary>
    private WallpaperDefaultSettings wallpaperDefaultSettings;
    
    /// <summary>
    ///   Gets or sets the settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </summary>
    /// <value>
    ///   The settings used for any new <see cref="Wallpaper" /> objects added to this collection.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///   Attempted to set a <c>null</c> value.
    /// </exception>
    /// <seealso cref="WallpaperManager.Data.WallpaperDefaultSettings">WallpaperDefaultSettings Class</seealso>
    public WallpaperDefaultSettings WallpaperDefaultSettings {
      get { return this.wallpaperDefaultSettings; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        this.wallpaperDefaultSettings = value;
      }
    }
    #endregion


    #region Methods: Constructor, IndexOfByImagePath, TryGetItem
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
    /// 
    /// <overloads>
    ///   <summary>
    ///     Initializes a new instance of the <see cref="WallpaperCategory" /> class.
    ///   </summary>
    ///   <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// </overloads>
    public WallpaperCategory(String name, IEnumerable<Wallpaper> wallpapers = null) {
      if (name == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("name"));
      }
      if (!name.Length.IsBetween(WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength)) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCategoryNameLengthInvalid(
          name.Length, WallpaperCategory.Name_MinLength, WallpaperCategory.Name_MaxLength, "name"
        ));
      }
      if (WallpaperCategory.Name_InvalidChars.Any(c => name.Contains(c))) {
        throw new ArgumentException(
          ExceptionMessages.GetCategoryNameInvalid(name, WallpaperCategory.Name_InvalidChars, "name")
        );
      }

      this.name = name;
      if (wallpapers != null) {
        foreach (Wallpaper wallpaper in wallpapers) {
          this.Add(wallpaper);
        }
      }

      this.wallpaperDefaultSettings = new WallpaperDefaultSettings();
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
    public Int32 IndexOfByImagePath(Path imagePath) {
      for (Int32 i = 0; i < this.Count; i++) {
        if (this[i].ImagePath == imagePath) {
          return i;
        }
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
    public Wallpaper TryGetItem(Int32 index) {
      if (index >= 0 && index < this.Count) {
        return this[index];
      }
      
      return null;
    }
    #endregion

    #region Methods: InsertItem, SetItem, RemoveItem, ClearItems, Item_PropertyChanged, UpdateIsActivatedBySingleItem
    /// <inheritdoc />
    protected override void InsertItem(Int32 index, Wallpaper item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      if (item.IsBlank) {
        this.WallpaperDefaultSettings.AssignTo(item);

        item.SuggestIsMultiscreen = this.WallpaperDefaultSettings.AutoDetermineIsMultiscreen;
        item.SuggestPlacement = this.WallpaperDefaultSettings.AutoDeterminePlacement;
      }
      base.InsertItem(index, item);

      this.UpdateIsActivatedBySingleItem(item);
      PropertyChangedEventManager.AddListener(item, this, String.Empty);
    }

    /// <inheritdoc />
    protected override void SetItem(Int32 index, Wallpaper item) {
      if (item == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("item"));
      }

      Wallpaper oldItem = this.TryGetItem(index);

      base.SetItem(index, item);

      if (oldItem != null) {
        PropertyChangedEventManager.RemoveListener(oldItem, this, String.Empty);
      }
      this.UpdateIsActivatedBySingleItem(item);
      PropertyChangedEventManager.AddListener(item, this, String.Empty);
    }

    /// <inheritdoc />
    protected override void RemoveItem(Int32 index) {
      Wallpaper removedItem = this.TryGetItem(index);

      base.RemoveItem(index);

      if (removedItem != null) {
        PropertyChangedEventManager.RemoveListener(removedItem, this, String.Empty);
        this.isActivatedIsUpToDate = false;
        this.OnPropertyChanged("IsActivated");
      }
    }

    /// <inheritdoc />
    protected override void ClearItems() {
      Wallpaper[] removedWallpapers = new Wallpaper[this.Count];
      this.CopyTo(removedWallpapers, 0);

      base.ClearItems();

      foreach (Wallpaper wallpaper in removedWallpapers) {
        PropertyChangedEventManager.RemoveListener(wallpaper, this, String.Empty);
      }
      this.isActivatedIsUpToDate = false;
      this.OnPropertyChanged("IsActivated");
    }

    /// <summary>
    ///   Handles the <see cref="WallpaperSettingsBase.PropertyChanged" /> event of a <see cref="Wallpaper" /> object.
    /// </summary>
    /// <commondoc select='All/Methods/EventHandlers[@Params="Object,+EventArgs"]/*' />
    private void Wallpaper_PropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "IsActivated") {
        this.UpdateIsActivatedBySingleItem((Wallpaper)sender);
      }
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
    #endregion

    #region Methods: OnPropertyChanged, ToString
    /// <commondoc select='INotifyPropertyChanged/Methods/OnPropertyChanged/*' />
    protected virtual void OnPropertyChanged(String propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///   Returns a <see cref="String" /> containing the category's name.
    /// </summary>
    /// <returns>
    ///   A <see cref="String" /> containing the category's name.
    /// </returns>
    public override String ToString() {
      return this.Name;
    }
    #endregion

    #region IWeakEventListener Implementation
    /// <inheritdoc />
    public Boolean ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
      if (managerType == typeof(PropertyChangedEventManager)) {
        this.Wallpaper_PropertyChanged(sender, (PropertyChangedEventArgs)e);
        return true;
      }

      return false;
    }
    #endregion
  }
}
