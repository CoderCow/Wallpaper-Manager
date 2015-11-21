// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;
using Common.IO;
using PropertyChanged;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines general wallpaper related data.
  /// </summary>
  [ContractClass(typeof(IWallpaperContracts))]
  public interface IWallpaper : IWallpaperBase, INotifyPropertyChanged {
    bool IsImageSizeResolved { get; }

    /// <summary>
    ///   Gets or sets the path of the image file of this wallpaper.
    /// </summary>
    /// <value>
    ///   The path of the image file of this wallpaper.
    /// </value>
    Path ImagePath { get; set; }

    /// <summary>
    ///   Gets or sets the size of the image where <see cref="ImagePath" /> is reffering to.
    /// </summary>
    /// <value>
    ///   The size of the image where <see cref="ImagePath" /> is reffering to.
    /// </value>
    Size? ImageSize { get; set; }

    DateTime TimeLastCycled { get; set; }
    DateTime TimeAdded { get; set; }
    int CycleCountWeek { get; set; }
    int CycleCountTotal { get; set; }
  }

  [DoNotNotify]
  [ContractClassFor(typeof(IWallpaper))]
  internal abstract class IWallpaperContracts : IWallpaper {
    public abstract bool IsActivated { get; set; }
    public abstract bool IsMultiscreen { get; set; }
    public abstract byte Priority { get; set; }
    public abstract TimeSpan OnlyCycleBetweenStart { get; set; }
    public abstract TimeSpan OnlyCycleBetweenStop { get; set; }
    public abstract WallpaperPlacement Placement { get; set; }
    public abstract Point Offset { get; set; }
    public abstract Point Scale { get; set; }
    public abstract WallpaperEffects Effects { get; set; }
    public abstract Color BackgroundColor { get; set; }
    public abstract HashSet<string> DisabledDevices { get; }
    public abstract bool IsBlank { get; set; }
    public abstract bool SuggestIsMultiscreen { get; set; }
    public abstract bool SuggestPlacement { get; set; }
    public abstract Size? ImageSize { get; set; }
    public abstract DateTime TimeLastCycled { get; set; }
    public abstract DateTime TimeAdded { get; set; }
    public abstract int CycleCountWeek { get; set; }
    public abstract int CycleCountTotal { get; set; }
    public abstract Path ImagePath { get; set; }

    public bool IsImageSizeResolved { 
      get {
        Contract.Ensures(Contract.Result<bool>() == (this.ImageSize != null));
        throw new NotImplementedException();
      }
    }
    public abstract object Clone();
    public abstract void AssignTo(object other);

    public event PropertyChangedEventHandler PropertyChanged;
    protected abstract void OnPropertyChanged([CallerMemberName] string propertyName = null);
  }
}