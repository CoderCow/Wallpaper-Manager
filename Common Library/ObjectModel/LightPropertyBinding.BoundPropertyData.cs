using System;
using System.Reflection;
using System.Windows;

namespace Common.ObjectModel {
  public partial class LightPropertyBindingManager {
    protected partial class LightPropertyBinding {
      protected struct BoundPropertyData {
        #region Property: SourceInfo
        /// <summary>
        ///   <inheritdoc cref="SourceInfo" select='../value/node()' />
        /// </summary>
        private PropertyInfo sourceInfo;

        /// <summary>
        ///   Gets or sets the <see cref="PropertyInfo" /> instance of the source object.
        /// </summary>
        /// <value>
        ///   The <see cref="PropertyInfo" /> instance of the source object.
        /// </value>
        public PropertyInfo SourceInfo {
          get { return this.sourceInfo; }
          set { this.sourceInfo = value; }
        }
        #endregion

        #region Property: TargetInfo
        /// <summary>
        ///   <inheritdoc cref="TargetInfo" select='../value/node()' />
        /// </summary>
        private PropertyInfo targetInfo;

        /// <summary>
        ///   Gets or sets the <see cref="PropertyInfo" /> instance of the target object.
        /// </summary>
        /// <value>
        ///   The <see cref="PropertyInfo" /> instance of the target object.
        /// </value>
        public PropertyInfo TargetInfo {
          get { return this.targetInfo; }
          set { this.targetInfo = value; }
        }
        #endregion


        #region Method: Constructor
        /// <summary>
        ///   Initializes a new instance of the <see cref="BoundPropertyData">BoundPropertyData Class</see>.
        /// </summary>
        /// <param name="sourceInfo">
        ///   <inheritdoc cref="SourceInfo" select='../value/node()' />
        /// </param>
        /// <param name="targetInfo">
        ///   <inheritdoc cref="TargetInfo" select='../value/node()' />
        /// </param>
        public BoundPropertyData(PropertyInfo sourceInfo, PropertyInfo targetInfo) {
          this.sourceInfo = sourceInfo;
          this.targetInfo = targetInfo;
        }
        #endregion
      }
    }
  }
}