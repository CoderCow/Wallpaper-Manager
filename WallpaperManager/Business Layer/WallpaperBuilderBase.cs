// THIS FILE IS PROVIDED UNDER THE TERMS OF THE CREATIVE COMMONS PUBLIC LICENSE WHICH CAN BE FOUND IN THE PROVIDED License.txt 
// FILE. IT IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER ITS 
// LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
// Written by David-Kay Posmyk (KayPosmyk@gmx.de)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

using Common.Drawing;
using Path = Common.IO.Path;

using WallpaperManager.Data;

namespace WallpaperManager.Business {
  /// <summary>
  ///   Base class for building a wallpaper to be applied on the Windows Desktop by using one or multiple images.
  /// </summary>
  /// <seealso href="dd316e3f-9541-46f1-961c-3c057c166f3b.htm" target="_self">Wallpaper Building Process</seealso>
  /// <threadsafety static="true" instance="false" />
  public abstract class WallpaperBuilderBase {
    #region Property: RequiredWallpapersByScreen
    /// <summary>
    ///   Gets an array of numbers representing the count of required <see cref="Wallpaper" /> instances to build a 
    ///   multiscreen wallpaper for each screen.
    /// </summary>
    /// <remarks>
    ///   The item index is equal (and in the same order) with the screen indexes.
    /// </remarks>
    /// <value>
    ///   An array of numbers representing the count of required <see cref="Wallpaper" /> instances to build a multiscreen 
    ///   wallpaper for each screen.
    /// </value>
    public abstract ReadOnlyCollection<Int32> RequiredWallpapersByScreen {
      get;
    }
    #endregion

    #region Property: ScreensSettings
    /// <summary>
    ///   <inheritdoc cref="ScreensSettings" select='../value/node()' />
    /// </summary>
    private ScreenSettingsCollection screensSettings;

    /// <summary>
    ///   Gets or sets a <see cref="ScreenSettingsCollection" /> object containing the specific properties for each single screen.
    /// </summary>
    /// <value>
    ///   A <see cref="ScreenSettingsCollection" /> object containing the specific properties for each single screen.
    /// </value>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    public ScreenSettingsCollection ScreensSettings {
      get { return this.screensSettings; }
      set {
        if (value == null) {
          throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull());
        }

        this.screensSettings = value;
      }
    }
    #endregion

    #region Property: FullScreenBounds
    /// <summary>
    ///   Gets the cached bounds of all screens.
    /// </summary>
    /// <value>
    ///   The cached bounds of all screens.
    /// </value>
    protected static Rectangle FullScreenBounds {
      get { 
        Int32 screenCount = Screen.AllScreens.Length;
        Rectangle fullScreenBounds = new Rectangle();

        for (Int32 i = 0; i < screenCount; i++) {
          fullScreenBounds = Rectangle.Union(Screen.AllScreens[i].Bounds, fullScreenBounds);
        }

        return fullScreenBounds;
      }
    }
    #endregion


    #region Constructor
    /// <summary>
    ///   Initializes a new instance of the <see cref="WallpaperBuilderBase"/> class.
    /// </summary>
    /// <param name="screensSettings">
    ///   A collection of <see cref="ScreenSettings" /> objects containing the specific properties for each single screen.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="screensSettings" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="ScreenSettingsCollection">ScreenSettingsCollection Class</seealso>
    /// <seealso cref="ScreenSettings">ScreenSettings Class</seealso>
    protected WallpaperBuilderBase(ScreenSettingsCollection screensSettings) {
      if (screensSettings == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("screenSettings"));
      }

      this.screensSettings = screensSettings;
    }
    #endregion

    #region Method: CreateMultiscreenFromSingle
    /// <summary>
    ///   Creates a multiscreen wallpaper from one <see cref="Wallpaper" /> object (from one image).
    /// </summary>
    /// <inheritdoc cref="CreateMultiscreenFromMultipleInternal" select='remarks|param|returns' />
    /// <param name="multiscreenWallpaper">
    ///   The <see cref="Wallpaper" /> object to use.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="multiscreenWallpaper" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The image file where the <paramref name="multiscreenWallpaper" /> object refers to could not be found.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public Image CreateMultiscreenFromSingle(Wallpaper multiscreenWallpaper, Single scaleFactor, Boolean useWindowsFix) {
      if (multiscreenWallpaper == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("multiscreenWallpaper"));
      }

      Rectangle fullScreenBounds = WallpaperBuilderBase.FullScreenBounds;

      Bitmap wallpaperImage = new Bitmap(
        (Int32)(fullScreenBounds.Width * scaleFactor), (Int32)(fullScreenBounds.Height * scaleFactor)
      );

      using (Graphics graphics = Graphics.FromImage(wallpaperImage)) {
        Graphics destinationGraphics = graphics;
        Bitmap preWallpaperImage = null;

        try {
          // If the fullscreenbounds rectangle has a negative x or y values, we normalize them by using 0,0 as origin and adding 
          // the difference to each drawn wallpaper later.
          Int32 xOriginAdd; 
          if (fullScreenBounds.X < 0) {
            xOriginAdd = Math.Abs(WallpaperBuilderBase.FullScreenBounds.X);
          } else {
            xOriginAdd = 0;
          }

          Int32 yOriginAdd;
          if (fullScreenBounds.Y < 0) {
            yOriginAdd = Math.Abs(WallpaperBuilderBase.FullScreenBounds.Y);
          } else {
            yOriginAdd = 0;
          }

          // Check if we have to redraw the end-wallpaper to fix it for Windows.
          Boolean requiresWindowsFix = ((useWindowsFix) && ((fullScreenBounds.Left < 0) || (fullScreenBounds.Top < 0)));

          if (requiresWindowsFix) {
            // We have to redraw it, we need another temporary image and draw on this one instead.
            preWallpaperImage = new Bitmap(wallpaperImage.Width, wallpaperImage.Height);
            destinationGraphics = Graphics.FromImage(preWallpaperImage);
          }
          
          destinationGraphics.ScaleTransform(scaleFactor, scaleFactor);

          // The rectangle of the multiscreen wallpaper should span across all screens except the ones which should display 
          // a statical wallpaper.
          Rectangle? multiscreenWallpaperRect = null;

          for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
            if (
              this.ScreensSettings[i].CycleRandomly || !this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions()
            ) {
              if (multiscreenWallpaperRect == null) {
                multiscreenWallpaperRect = this.ScreensSettings[i].BoundsWithMargin;
              } else {
                multiscreenWallpaperRect = Rectangle.Union(
                  multiscreenWallpaperRect.Value, this.ScreensSettings[i].BoundsWithMargin
                );
              }
            }
          }

          // null would mean that all screens should display static wallpapers.
          if (multiscreenWallpaperRect != null) {
            Rectangle multiscreenWallpaperRectValue = multiscreenWallpaperRect.Value;
            multiscreenWallpaperRectValue.X += xOriginAdd;
            multiscreenWallpaperRectValue.Y += yOriginAdd;

            destinationGraphics.SetClip(multiscreenWallpaperRectValue);
            WallpaperBuilderBase.DrawWallpaper(
              destinationGraphics, multiscreenWallpaperRectValue, multiscreenWallpaper, multiscreenWallpaper.Placement
              );
            destinationGraphics.ResetClip();
          }

          // Draw or overdraw all static wallpapers and draw the Overlay Texts for all screens.
          for (Int32 i = 0; i < this.ScreensSettings.Count; i++) {
            destinationGraphics.SetClip(this.ScreensSettings[i].BoundsWithMargin);

            if (
              !this.ScreensSettings[i].CycleRandomly && this.ScreensSettings[i].StaticWallpaper.EvaluateCycleConditions()
            ) {
              WallpaperBuilderBase.DrawWallpaper(
                destinationGraphics,
                this.ScreensSettings[i].BoundsWithMargin,
                this.ScreensSettings[i].StaticWallpaper,
                this.ScreensSettings[i].StaticWallpaper.Placement
              );
            }

            WallpaperBuilderBase.DrawOverlayTexts(
              destinationGraphics, 
              this.ScreensSettings[i].BoundsWithMargin, 
              new[] { multiscreenWallpaper }, 
              this.ScreensSettings[i].TextOverlays
            );
            destinationGraphics.ResetClip();
          }

          if (requiresWindowsFix) {
            // Now redraw the wallpaper but fixed.
            WallpaperBuilderBase.DrawFullWallpaperFixed(graphics, preWallpaperImage);
          }
        } finally {
          destinationGraphics.Dispose();

          if (preWallpaperImage != null) {
            preWallpaperImage.Dispose();
          }
        }
      }

      return wallpaperImage;
    }
    #endregion

    #region Method: CreateMultiscreenFromMultiple, CreateMultiscreenFromMultipleInternal
    /// <inheritdoc cref="CreateMultiscreenFromMultipleInternal" />
    /// <param name="wallpapers">
    ///   The <see cref="Wallpaper" /> objects to use for each screen.
    /// </param>
    public abstract Image CreateMultiscreenFromMultiple(
      IList<IList<Wallpaper>> wallpapers, Single scaleFactor, Boolean useWindowsFix
    );

    /// <summary>
    ///   Creates a multiscreen wallpaper from multiple <see cref="Wallpaper" /> objects (from multiple images).
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Use a low <paramref name="scaleFactor" /> to create preview images.
    ///   </para>
    ///   <para>
    ///     Only set <paramref name="useWindowsFix" /> to <c>true</c> if you are planning to apply the generated
    ///     image on the Windows Desktop.
    ///   </para>
    /// </remarks>
    /// <param name="wallpapers">
    ///   A collection containing the <see cref="Wallpaper" /> objects to use.
    /// </param>
    /// <param name="scaleFactor">
    ///   Defines the factor to scale the created image. Decrease this value to speed up the drawing process of preview images.
    /// </param>
    /// <param name="useWindowsFix">
    ///   A <see cref="Boolean" /> indicating whether the image is drawn in a special way to fix problems that occur when 
    ///   Windows applies tiled images for extended desktops. Set to <c>true</c> if you are planning to apply the generated
    ///   image on the Windows Desktop.
    /// </param>
    /// <returns>
    ///   A <see cref="Image" /> instance containing drawn the multiscreen wallpaper image. 
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   <paramref name="wallpapers" /> a <c>null</c> item.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="wallpapers" /> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="wallpapers" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The image file where on of the <see cref="Wallpaper" /> objects refers to could not be found.
    /// </exception>
    protected Image CreateMultiscreenFromMultipleInternal(
      IList<Wallpaper> wallpapers, Single scaleFactor, Boolean useWindowsFix
    ) {
      if (wallpapers == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapers"));
      }
      if (wallpapers.Count == 0) {
        throw new ArgumentOutOfRangeException(ExceptionMessages.GetCollectionIsEmpty("wallpapers"));
      }
      if (wallpapers.Contains(null)) {
        throw new ArgumentException(ExceptionMessages.GetCollectionContainsNullItem("wallpapers"));
      }

      Rectangle fullScreenBounds = WallpaperBuilderBase.FullScreenBounds;
      Int32 screenCount = Screen.AllScreens.Length;

      Bitmap wallpaperImage = new Bitmap(
        (Int32)(fullScreenBounds.Width * scaleFactor), (Int32)(fullScreenBounds.Height * scaleFactor)
      );

      using (Graphics graphics = Graphics.FromImage(wallpaperImage)) {
        Graphics destinationGraphics = graphics;
        Bitmap preWallpaperImage = null;

        try {
          // If the fullscreenbounds rectangle has a negative x or y values, we normalize them by using 0,0 as origin and adding 
          // the difference to each drawn wallpaper later.
          Int32 xOriginAdd; 
          if (fullScreenBounds.X < 0) {
            xOriginAdd = Math.Abs(WallpaperBuilderBase.FullScreenBounds.X);
          } else {
            xOriginAdd = 0;
          }

          Int32 yOriginAdd;
          if (fullScreenBounds.Y < 0) {
            yOriginAdd = Math.Abs(WallpaperBuilderBase.FullScreenBounds.Y);
          } else {
            yOriginAdd = 0;
          }

          // Check if we have to redraw the end-wallpaper to fix it for Windows' strange way of applyment.
          Boolean requiresWindowsFix = ((useWindowsFix) && ((fullScreenBounds.Left < 0) || (fullScreenBounds.Top < 0)));

          if (requiresWindowsFix) {
            // We have to redraw it, we need another temporary image and draw on this one instead.
            preWallpaperImage = new Bitmap(wallpaperImage.Width, wallpaperImage.Height);
            destinationGraphics = Graphics.FromImage(preWallpaperImage);
          }
          
          // Draw the full wallpaper.
          destinationGraphics.ScaleTransform(scaleFactor, scaleFactor);

          for (Int32 i = 0; i < screenCount; i++) {
            Rectangle screenBounds = this.ScreensSettings[i].BoundsWithMargin;
            screenBounds.X += xOriginAdd;
            screenBounds.Y += yOriginAdd;

            destinationGraphics.SetClip(screenBounds, CombineMode.Replace);
            WallpaperBuilderBase.DrawWallpaper(destinationGraphics, screenBounds, wallpapers[i], wallpapers[i].Placement);
            WallpaperBuilderBase.DrawOverlayTexts(
              destinationGraphics, screenBounds, wallpapers, this.ScreensSettings[i].TextOverlays
            );
            destinationGraphics.ResetClip();
          }

          if (requiresWindowsFix) {
            // Now redraw the wallpaper but fixed.
            WallpaperBuilderBase.DrawFullWallpaperFixed(graphics, preWallpaperImage);
          }
        } finally {
          destinationGraphics.Dispose();

          if (preWallpaperImage != null) {
            preWallpaperImage.Dispose();
          }
        }
      }

      return wallpaperImage;
    }
    #endregion

    #region Static Methods: DrawWallpaper, DrawFullWallpaperFixed, DrawOverlayTexts
    /// <summary>
    ///   Draws a wallpaper image by using the given <see cref="Graphics" /> object.
    /// </summary>
    /// <param name="destGraphics">
    ///   The <see cref="Graphics" /> object used to draw.
    /// </param>
    /// <param name="destScreenRect">
    ///   The destination screen's rectangle.
    /// </param>
    /// <param name="wallpaper">
    ///   The <see cref="Wallpaper" /> object to draw.
    /// </param>
    /// <param name="placement">
    ///   The type how the wallpaper image should be aligned.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="destGraphics" /> is <c>null</c> or <paramref name="wallpaper" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="placement" /> is not a constant defined by the <see cref="WallpaperPlacement" /> enumeration.
    /// </exception>
    /// <seealso cref="Graphics">Graphics Class</seealso>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    /// <seealso cref="WallpaperPlacement">WallpaperPlacement Enumeration</seealso>.
    protected static void DrawWallpaper(
      Graphics destGraphics, Rectangle destScreenRect, Wallpaper wallpaper, WallpaperPlacement placement
    ) {
      if (destGraphics == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("destGraphics"));
      }
      if (wallpaper == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaper"));
      }
      if (!Enum.IsDefined(typeof(WallpaperPlacement), placement)) {
        throw new ArgumentOutOfRangeException(
          ExceptionMessages.GetEnumValueInvalid("placement", typeof(WallpaperPlacement), placement)
        );
      }

      using (SolidBrush backgroundBrush = new SolidBrush(wallpaper.BackgroundColor)) {
        destGraphics.FillRectangle(backgroundBrush, destScreenRect);
      }

      if ((wallpaper.ImagePath != Path.None) && (File.Exists(wallpaper.ImagePath))) {
        Image originalImage = null;
        Image imageToDraw = null;
        
        try {
          originalImage = Image.FromFile(wallpaper.ImagePath);
          imageToDraw = originalImage;
          
          if (wallpaper.Effects != 0) {
            #region Mirror Effect
            // Required images to draw for the mirror effect.
            Int32 horizontalImages = 1;
            Int32 verticalImages = 1;

            // This values "move" the texture of the brush.
            Int32 horizontalBrushTransform = 0;
            Int32 verticalBrushTransform = 0;
            TextureBrush mirrorBrush = new TextureBrush(originalImage, WrapMode.TileFlipXY);

            if (((wallpaper.Effects & WallpaperEffects.MirrorLeft) == WallpaperEffects.MirrorLeft)) {
              horizontalImages++;
              mirrorBrush.WrapMode = WrapMode.TileFlipX;

              // This will make the brush start with the mirrored image.
              horizontalBrushTransform = originalImage.Width;
            }
            if (((wallpaper.Effects & WallpaperEffects.MirrorRight) == WallpaperEffects.MirrorRight)) {
              horizontalImages++;
              mirrorBrush.WrapMode = WrapMode.TileFlipX;

              if (((wallpaper.Effects & WallpaperEffects.MirrorLeft) != WallpaperEffects.MirrorLeft)) {
                // This will make the brush start with the unmirrored image.
                horizontalBrushTransform = 0;
              }
            }

            if (((wallpaper.Effects & WallpaperEffects.FlipHorizontal) == WallpaperEffects.FlipHorizontal)) {
              mirrorBrush.WrapMode = WrapMode.TileFlipX;

              if (horizontalBrushTransform == 0) {
                // This will make the brush start with the mirrored image.
                horizontalBrushTransform = originalImage.Width;
              } else {
                // This will make the brush start with the unmirrored image.
                horizontalBrushTransform = 0;
              }
            }

            if (((wallpaper.Effects & WallpaperEffects.MirrorTop) == WallpaperEffects.MirrorTop)) {
              verticalImages++;

              if (mirrorBrush.WrapMode != WrapMode.TileFlipXY) {
                if (mirrorBrush.WrapMode == WrapMode.TileFlipX) {
                  mirrorBrush.WrapMode = WrapMode.TileFlipXY;
                } else {
                  mirrorBrush.WrapMode = WrapMode.TileFlipY;
                }
              }

              // This will make the brush start with the mirrored image.
              verticalBrushTransform = originalImage.Height;
            }
            if (((wallpaper.Effects & WallpaperEffects.MirrorBottom) == WallpaperEffects.MirrorBottom)) {
              verticalImages++;

              if (mirrorBrush.WrapMode != WrapMode.TileFlipXY) {
                if (mirrorBrush.WrapMode == WrapMode.TileFlipX) {
                  mirrorBrush.WrapMode = WrapMode.TileFlipXY;
                } else {
                  mirrorBrush.WrapMode = WrapMode.TileFlipY;
                }
              }

              if (((wallpaper.Effects & WallpaperEffects.MirrorTop) != WallpaperEffects.MirrorTop)) {
                // This will make the brush start with the unmirrored image.
                verticalBrushTransform = 0;
              }
            }

            if (((wallpaper.Effects & WallpaperEffects.FlipVertical) == WallpaperEffects.FlipVertical)) {
              if (mirrorBrush.WrapMode != WrapMode.TileFlipXY) {
                if (mirrorBrush.WrapMode == WrapMode.TileFlipX) {
                  mirrorBrush.WrapMode = WrapMode.TileFlipXY;
                } else {
                  mirrorBrush.WrapMode = WrapMode.TileFlipY;
                }
              }

              if (verticalBrushTransform == 0) {
                // This will make the brush start with the mirrored image.
                verticalBrushTransform = originalImage.Height;
              } else {
                // This will make the brush start with the unmirrored image.
                verticalBrushTransform = 0;
              }
            }

            mirrorBrush.TranslateTransform(horizontalBrushTransform, verticalBrushTransform);

            Image modifiedImage = new Bitmap(
              originalImage.Width * horizontalImages, originalImage.Height * verticalImages
            );

            Graphics modifiedImageGraphics = null;
            try {
              modifiedImageGraphics = Graphics.FromImage(modifiedImage);

              modifiedImageGraphics.FillRectangle(
                mirrorBrush, new Rectangle(0, 0, modifiedImage.Width, modifiedImage.Height)
              );

              imageToDraw = modifiedImage;
            } finally {
              if (modifiedImageGraphics != null) {
                modifiedImageGraphics.Dispose();
              }
            }
            #endregion
          }

          GraphicsContainer graphicalContext = null;
          try {
            graphicalContext = destGraphics.BeginContainer();
            
            // Place the origin in the center of the screen.
            Single xTranslationForScale = destScreenRect.X + (destScreenRect.Width / 2f);
            Single yTranslationForScale = destScreenRect.Y + (destScreenRect.Height / 2f);
            destGraphics.TranslateTransform(xTranslationForScale, yTranslationForScale);
            destGraphics.ScaleTransform((wallpaper.Scale.X + 100) / 100f, (wallpaper.Scale.Y + 100) / 100f);
            // And reset it, that way we have a nice scaling of the image.
            destGraphics.TranslateTransform(-xTranslationForScale, -yTranslationForScale);

            // Now apply the offset.
            destGraphics.TranslateTransform(wallpaper.Offset.X, wallpaper.Offset.Y);

            destGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            destGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            destGraphics.CompositingMode = CompositingMode.SourceOver;

            switch (placement) {
              case WallpaperPlacement.Uniform:
                destGraphics.DrawImageUniformed(imageToDraw, destScreenRect);

                break;
              case WallpaperPlacement.UniformToFill:
                destGraphics.DrawImageUniformedToFill(imageToDraw, destScreenRect);

                break;
              case WallpaperPlacement.Stretch:
                destGraphics.DrawImage(imageToDraw, destScreenRect);

                break;
              case WallpaperPlacement.Center:
                destGraphics.DrawImageCentered(imageToDraw, destScreenRect);

                break;
              case WallpaperPlacement.Tile:
                destGraphics.ScaleTransform((wallpaper.Scale.X + 100) / 100f, (wallpaper.Scale.Y + 100) / 100f);
                destGraphics.DrawImageTiled(imageToDraw, destScreenRect);

                break;
            }
          } finally {
            if (graphicalContext != null) {
              destGraphics.EndContainer(graphicalContext);
            }
          }
        } finally {
          if (originalImage != null) {
            originalImage.Dispose();
          }
          if (imageToDraw != null) {
            imageToDraw.Dispose();
          }
        }
      }
    }

    /// <summary>
    ///   Redraws a finished full wallpaper image so, that Windows will display it on the Desktop as expected.
    /// </summary>
    /// <param name="destGraphics">
    ///   The <see cref="Graphics" /> object used to draw.
    /// </param>
    /// <param name="wallpaperImage">
    ///   The <see cref="Image" /> object containing the full wallpaper to redraw.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="destGraphics" /> is <c>null</c> or <paramref name="wallpaperImage" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="Graphics">Graphics Class</seealso>
    /// <seealso cref="Bitmap">Bitmap Class</seealso>
    protected static void DrawFullWallpaperFixed(Graphics destGraphics, Image wallpaperImage) {
      if (destGraphics == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("destGraphics"));
      }
      if (wallpaperImage == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpaperImage"));
      }

      Single scaleFactor = ((Single)wallpaperImage.Width / WallpaperBuilderBase.FullScreenBounds.Width);
      Rectangle scaledFullScreenBounds = WallpaperBuilderBase.FullScreenBounds.ScaleFull(scaleFactor);

      // TODO: Buggy with 2x2 monitor systems and greater.
      // Are there screens on the left side of the primary screen?
      if (WallpaperBuilderBase.FullScreenBounds.Left < 0) {
        // Negative Side (Left from Primary)
        destGraphics.DrawImage(
          wallpaperImage, 
          new Rectangle(
            scaledFullScreenBounds.Right, 0, Math.Abs(scaledFullScreenBounds.Left), scaledFullScreenBounds.Height
          ),
          new Rectangle(
            0, 0, Math.Abs(scaledFullScreenBounds.Left), scaledFullScreenBounds.Height
          ),
          GraphicsUnit.Pixel
        );

        // Positive Side (Primary and Right From Primary)
        destGraphics.DrawImage(
          wallpaperImage, 
          new Rectangle(
            0, 0, scaledFullScreenBounds.Right, scaledFullScreenBounds.Height
          ),
          new Rectangle(
            Math.Abs(scaledFullScreenBounds.Left), 0, scaledFullScreenBounds.Right, scaledFullScreenBounds.Height
          ),
          GraphicsUnit.Pixel
        );
      }

      // Are there screens above the primary screen?
      if (WallpaperBuilderBase.FullScreenBounds.Top < 0) {
        // Drawing the negative side (above primary) to the positive side (primary and below primary).
        destGraphics.DrawImage(
          wallpaperImage, 
          new Rectangle(
            0, scaledFullScreenBounds.Bottom, scaledFullScreenBounds.Width, Math.Abs(scaledFullScreenBounds.Top)
          ),
          new Rectangle(
            0, 0, scaledFullScreenBounds.Width, Math.Abs(scaledFullScreenBounds.Top)
          ),
          GraphicsUnit.Pixel
        );

        // Drawing the positive side (primary and below primary) to the negative side (above primary).
        destGraphics.DrawImage(
          wallpaperImage, 
          new Rectangle(
            0, 0, scaledFullScreenBounds.Width, scaledFullScreenBounds.Bottom
          ),
          new Rectangle(
            0, Math.Abs(scaledFullScreenBounds.Top), scaledFullScreenBounds.Width, scaledFullScreenBounds.Bottom
          ),
          GraphicsUnit.Pixel
        );
      }
    }

    /// <summary>
    ///   Draws the given <see cref="WallpaperTextOverlay" /> objects by using the given <see cref="Graphics" /> object.
    /// </summary>
    /// <param name="destGraphics">
    ///   The <see cref="Graphics" /> to draw with.
    /// </param>
    /// <param name="rect">
    ///   The destination <see cref="Rectangle" /> to draw into.
    /// </param>
    /// <param name="wallpapers">
    ///   The collection of <see cref="Wallpaper" /> objects which are actually being applied. Note that the items in this
    ///   collection require to be in the same index as the screen where they are going to be applied on.
    /// </param>
    /// <param name="overlayTexts">
    ///   A collection of <see cref="WallpaperTextOverlay" /> objects to be drawn.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="destGraphics" /> or <paramref name="wallpapers" /> or <paramref name="overlayTexts" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="Graphics">Graphics Class</seealso>
    /// <seealso cref="WallpaperTextOverlay">WallpaperTextOverlay Class</seealso>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    private static void DrawOverlayTexts(
      Graphics destGraphics, Rectangle rect, IList<Wallpaper> wallpapers, IList<WallpaperTextOverlay> overlayTexts
    ) {
      if (destGraphics == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("destGraphics"));
      }
      if (wallpapers == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("wallpapers"));
      }
      if (overlayTexts == null) {
        throw new ArgumentNullException(ExceptionMessages.GetVariableCanNotBeNull("overlayTexts"));
      }
      if (overlayTexts.Count == 0) {
        return;
      }

      GraphicsContainer graphicalContext = null;
      try {
        graphicalContext = destGraphics.BeginContainer();
        destGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        destGraphics.SetClip(rect);

        foreach (WallpaperTextOverlay overlayText in overlayTexts) {
          if (overlayText != null) {
            using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)) {
              switch (overlayText.Position) {
                case TextOverlayPosition.TopLeft:
                  format.Alignment = StringAlignment.Near;
                  format.LineAlignment = StringAlignment.Near;
                  break;
                case TextOverlayPosition.TopMiddle:
                  format.Alignment = StringAlignment.Center;
                  format.LineAlignment = StringAlignment.Near;
                  break;
                case TextOverlayPosition.TopRight:
                  format.Alignment = StringAlignment.Far;
                  format.LineAlignment = StringAlignment.Near;
                  break;
                case TextOverlayPosition.BottomLeft:
                  format.Alignment = StringAlignment.Near;
                  format.LineAlignment = StringAlignment.Far;
                  break;
                case TextOverlayPosition.BottomMiddle:
                  format.Alignment = StringAlignment.Center;
                  format.LineAlignment = StringAlignment.Far;
                  break;
                default:
                  format.Alignment = StringAlignment.Far;
                  format.LineAlignment = StringAlignment.Far;
                  break;
              }

              Rectangle textRect = rect;
              textRect.X += overlayText.HorizontalOffset;
              textRect.Y += overlayText.VerticalOffset;

              GraphicsPath textPath = null;
              FontFamily fontFamily = null;
              Pen borderPen = null;
              SolidBrush fontBrush = null;
              try {
                Single fontSizeInPixels = (destGraphics.DpiY * overlayText.FontSize / 72);
                fontFamily = new FontFamily(overlayText.FontName);

                textPath = new GraphicsPath();
                textPath.AddString(
                  overlayText.GetEvaluatedText(wallpapers), 
                  fontFamily, (Int32)overlayText.FontStyle, fontSizeInPixels, 
                  textRect, format
                );

                borderPen = new Pen(overlayText.BorderColor, (Single)Math.Round(fontSizeInPixels * 0.2, 2));
                fontBrush = new SolidBrush(overlayText.ForeColor);
                destGraphics.DrawPath(borderPen, textPath);
                destGraphics.FillPath(fontBrush, textPath);
              } finally {
                if (textPath != null) textPath.Dispose();
                if (fontFamily != null) fontFamily.Dispose();
                if (borderPen != null) borderPen.Dispose();
                if (fontBrush != null) fontBrush.Dispose();
              }
            }
          }
        }
      } finally {
        if (graphicalContext != null) {
          destGraphics.EndContainer(graphicalContext);
        }
      }
    }
    #endregion
  }
}
