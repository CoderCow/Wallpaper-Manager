// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace WallpaperManager.Models {
  /// <summary>
  ///   A collection of <see cref="Wallpaper" /> objects with a fixed size.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If the collection count reaches the <see cref="MaximumSize" />, the collection won't grow any further and will
  ///     replace
  ///     existing items. If a new item is added for example, and the <see cref="MaximumSize" /> has been reached, all
  ///     containing
  ///     items will be moved up so that the item with index 0 will be overwritten (A, B, C goes to B, C, D).
  ///   </para>
  ///   <note type="caution">
  ///     This collection does not support inserting of items before the last item.
  ///   </note>
  /// </remarks>
  /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
  /// <threadsafety static="true" instance="false" />
  public class LastActiveWallpaperCollection : Collection<Wallpaper> {
    /// <summary>
    ///   <inheritdoc cref="MaximumSize" select='../value/node()' />
    /// </summary>
    private int maximumSize;

    /// <summary>
    ///   Gets or sets the maximum count of items this collection is allowed to have.
    /// </summary>
    /// <value>
    ///   The maximum count of items this collection is allowed to have.
    /// </value>
    public int MaximumSize {
      get { return this.maximumSize; }
      set {
        // Check whether there are too many items in this collection when the maximum size was lowered.
        if ((value < this.MaximumSize) && (this.Count > value)) {
          // Delete overflowing items.
          for (int i = this.Count - 1; i >= value; i--)
            this.RemoveAt(i);

          ((List<Wallpaper>)this.Items).TrimExcess();
        }

        this.maximumSize = value;
      }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="LastActiveWallpaperCollection" /> class.
    /// </summary>
    /// <param name="maximumSize">
    ///   The maximum size allowed for this collection.
    /// </param>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public LastActiveWallpaperCollection(int maximumSize) : base(new List<Wallpaper>(maximumSize)) {
      this.maximumSize = maximumSize;
    }

    /// <summary>
    ///   Checks whether all properties have valid values.
    /// </summary>
    [ContractInvariantMethod]
    private void CheckInvariants() {
      Contract.Invariant(this.MaximumSize >= 1);
    }

    /// <summary>
    ///   Adds a range of items to the collection.
    /// </summary>
    /// <param name="range">
    ///   The items which should be added to the collection.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="range" /> is <c>null</c>.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    public void AddRange(IEnumerable<Wallpaper> range) {
      if (range == null) throw new ArgumentNullException();

      foreach (Wallpaper wallpaper in range)
        this.Add(wallpaper);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="index" /> is not equal to <see cref="Collection{Wallpaper}.Count" />.
    /// </exception>
    /// <seealso cref="Wallpaper">Wallpaper Class</seealso>
    protected override void InsertItem(int index, Wallpaper item) {
      if (index != this.Count) throw new ArgumentOutOfRangeException();

      if (this.Count == this.MaximumSize) {
        // Pull items up.
        for (int i = 0; i < this.Count - 1; i++)
          this.Items[i] = this.Items[i + 1];

        this.Items[index - 1] = item;
      } else
        base.InsertItem(index, item);
    }
  }
}