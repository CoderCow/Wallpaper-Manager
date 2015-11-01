// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;

namespace WallpaperManager.Data {
  /// <summary>
  ///   Contains data to be applied to newly added wallpapers.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class WallpaperDefaultSettings: WallpaperSettingsBase {
    #region Property: AutoDetermineIsMultiscreen
    /// <summary>
    ///   <inheritdoc cref="AutoDetermineIsMultiscreen" select='../value/node()' />
    /// </summary>
    private Boolean autoDetermineIsMultiscreen;

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be 
    ///   determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.IsMultiscreen" /> property should be determined 
    ///   automatically; otherwise <c>false</c>.
    /// </value>
    public Boolean AutoDetermineIsMultiscreen {
      get { return this.autoDetermineIsMultiscreen; }
      set { 
        this.autoDetermineIsMultiscreen = value; 
        this.OnPropertyChanged("AutoDetermineIsMultiscreen");
      }
    }
    #endregion

    #region Property: AutoDeterminePlacement
    /// <summary>
    ///   <inheritdoc cref="AutoDeterminePlacement" select='../value/node()' />
    /// </summary>
    private Boolean autoDeterminePlacement;

    /// <summary>
    ///   Gets or sets a value indicating whether the <see cref="WallpaperSettingsBase.Placement" /> 
    ///   property should be determined automatically or not.
    /// </summary>
    /// <value>
    ///   <c>true</c> whether the <see cref="WallpaperSettingsBase.Placement" /> property should be 
    ///   determined automatically; otherwise <c>false</c>.
    /// </value>
    public Boolean AutoDeterminePlacement {
      get { return this.autoDeterminePlacement; }
      set { 
        this.autoDeterminePlacement = value; 
        this.OnPropertyChanged("AutoDeterminePlacement");
      }
    }
    #endregion


    #region Methods: Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperDefaultSettings" /> class.
    /// </summary>
    public WallpaperDefaultSettings() {
      this.autoDetermineIsMultiscreen = true;
      this.autoDeterminePlacement = true;
    }
    #endregion

    #region ICloneable Implementation, IAssignable Implementation
    /// <inheritdoc />
    public override Object Clone() {
      WallpaperDefaultSettings clonedInstance = new WallpaperDefaultSettings();

      // Clone all fields defined by WallpaperSettingsBase.
      base.Clone(clonedInstance);

      clonedInstance.autoDetermineIsMultiscreen = this.AutoDetermineIsMultiscreen;
      clonedInstance.autoDeterminePlacement = this.AutoDeterminePlacement;

      return clonedInstance;
    }

    /// <summary>
    ///   Assigns all member values of this instance to the respective members of the given instance.
    /// </summary>
    /// <param name="other">
    ///   The target instance to assign to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="other" /> is <c>null</c>.
    /// </exception>
    protected override void AssignTo(WallpaperSettingsBase other) {
      if (other == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("other"));
      }

      // Assign all members defined by WallpaperSettingsBase.
      base.AssignTo(other);

      WallpaperDefaultSettings defaultSettingsInstance = (other as WallpaperDefaultSettings);
      if (defaultSettingsInstance != null) {
        defaultSettingsInstance.AutoDetermineIsMultiscreen = this.AutoDetermineIsMultiscreen;
        defaultSettingsInstance.AutoDeterminePlacement = this.AutoDeterminePlacement;
      }
    }
    #endregion
  }
}
