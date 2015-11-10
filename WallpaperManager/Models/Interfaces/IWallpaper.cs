// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using Common.IO;

namespace WallpaperManager.Models {
  /// <summary>
  ///   Defines general wallpaper related data.
  /// </summary>
  [ContractClass(typeof(IWallpaperContracts))]
  public interface IWallpaper : IWallpaperBase {
    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether any properties of this instance had been changed since it has been
    ///   instanced.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether any properties of this instance had been changed since it has been
    ///   instanced.
    /// </value>
    bool IsBlank { get; }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether the <see cref="WallpaperBase.IsMultiscreen" /> setting should
    ///   be automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether the <see cref="WallpaperBase.IsMultiscreen" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    bool SuggestIsMultiscreen { get; set; }

    /// <summary>
    ///   Gets a <see cref="bool" /> indicating whether <see cref="WallpaperBase.Placement" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </summary>
    /// <value>
    ///   A <see cref="bool" /> indicating whether <see cref="WallpaperBase.Placement" /> setting should be
    ///   automatically suggested for this wallpaper or not.
    /// </value>
    bool SuggestPlacement { get; set; }

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
    /// <remarks>
    ///   When this property is changed for the first time, and their respective <see cref="SuggestPlacement" /> and
    ///   <see cref="SuggestIsMultiscreen" /> properties are <c>true</c>, it will cause the
    ///   <see cref="WallpaperBase.Placement" /> and <see cref="WallpaperBase.IsMultiscreen" /> properties to
    ///   be
    ///   suggested automatically related to the new image size.
    /// </remarks>
    /// <seealso cref="WallpaperBase.Placement">WallpaperBase.Placement Property</seealso>
    /// <seealso cref="WallpaperBase.IsMultiscreen">WallpaperBase.IsMultiscreen Property</seealso>
    /// <seealso cref="SuggestPlacement">SuggestPlacement Property</seealso>
    /// <seealso cref="SuggestIsMultiscreen">SuggestIsMultiscreen Property</seealso>
    Size ImageSize { get; set; }

    DateTime TimeLastCycled { get; set; }
    DateTime TimeAdded { get; set; }
    int CycleCount { get; set; }

    /// <summary>
    ///   Checks whether the cycle conditions for this wallpaper match or not.
    /// </summary>
    /// <returns>
    ///   A <see cref="bool" /> indicating whether the cycle conditions match.
    /// </returns>
    bool EvaluateCycleConditions();
  }

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
    public abstract Collection<int> DisabledScreens { get; }
    public abstract bool IsBlank { get; set; }
    public abstract bool SuggestIsMultiscreen { get; set; }
    public abstract bool SuggestPlacement { get; set; }
    public abstract Path ImagePath { get; set; }
    public abstract Size ImageSize { get; set; }
    public abstract DateTime TimeLastCycled { get; set; }
    public abstract DateTime TimeAdded { get; set; }
    public abstract int CycleCount { get; set; }
    public abstract bool EvaluateCycleConditions();

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.ImagePath != Path.None);
      Contract.Invariant(this.ImageSize.Width > 0 && this.ImageSize.Height > 0);
    }

    public abstract object Clone();
    public abstract void AssignTo(object other);
  }
}