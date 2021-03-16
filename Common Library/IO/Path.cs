using System;
using System.Diagnostics.Contracts;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.IO {
  /// <summary>
  ///   Represents a path refering to a file or directory in the file system.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This structure represents a single file or directory path in the local or on a remote file system which can be either 
  ///     relative or absolute, normalized or not. It does not support multiple paths at once, path patterns which contain 
  ///     wildcards or a path pointing to a physical device (like <c>\\.\PHYSICALDRIVE0</c>).
  ///   </para>
  ///   <para>
  ///     The advantage of using this structure over raw <see cref="String" /> objects is that the path string is fully 
  ///     validated when its assigned to this structure and therefore does not require any more validation by methods
  ///     using it, so its validated only one time in the whole call stack which saves redudant code and performance.
  ///   </para>
  ///   <para>
  ///     It is recommended to use methods provided by this structure to get sub-parts of its represented path, such as the
  ///     parent or root directory because the performance of is significantly increased compared to the native methods 
  ///     and classes provided by the .NET Framework, also path structures created by this type are always valid and therefore 
  ///     skip the validation process. 
  ///   </para>
  ///   <para>
  ///     Use the path structure returned by the <see cref="None" /> property or by using <c>default(Path)</c> to indicate an
  ///     empty path object. Note that all properties and methods of an empty path will throw a <see cref="PathException" />.
  ///   </para>
  /// </remarks>
  /// <seealso cref="PathException">PathException Class</seealso>
  [Serializable]
  public struct Path: IComparable, IComparable<Path>, IEquatable<Path> {
    #region Static Fields: pathValidationRegex
    /// <summary>
    ///   The <see cref="Regex" /> used to check whether a path is normalized or not.
    /// </summary>
    private readonly static Regex pathCheckIsNotNormalizedRegex = new Regex(
      @"((^\.+) | ([\\/]\.+)) \s* ($ | [\\/])", 
      RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    #endregion

    #region Fields: value
    /// <summary>
    ///   The path string representing the value of this structure.
    /// </summary>
    private readonly String value;
    #endregion

    #region Static Property: None
    /// <summary>
    ///   Gets the empty <see cref="Path" /> structure representation. This is equal to <c>default(Path)</c>.
    /// </summary>
    /// <value>
    ///   The empty <see cref="Path" /> structure representation.
    /// </value>
    public static Path None {
      get { return default(Path); }
    }
    #endregion

    #region Static Property: InvalidFileNameChars
    /// <summary>
    ///   <inheritdoc cref="InvalidFileNameChars" select='../value/node()' />
    /// </summary>
    private static Char[] invalidFileNameChars;

    /// <summary>
    ///   Gets an array of characters that are not allowed in file names.
    /// </summary>
    /// <remarks>
    ///   The value of this property is cached after the get-accessor has been called the first time.
    /// </remarks>
    /// <value>
    ///   An array of characters that are not allowed in file names.
    /// </value>
    public static Char[] InvalidFileNameChars {
      get {
        if (Path.invalidFileNameChars == null)
          Path.invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();

        return Path.invalidFileNameChars;
      }
    }
    #endregion

    #region Static Property: InvalidPathChars
    /// <summary>
    ///   <inheritdoc cref="InvalidPathChars" select='../value/node()' />
    /// </summary>
    private static Char[] invalidPathChars;

    /// <summary>
    ///   Gets an array of characters that are not allowed in paths.
    /// </summary>
    /// <remarks>
    ///   The value of this property is cached after the get-accessor has been called the first time.
    /// </remarks>
    /// <value>
    ///   An array of characters that are not allowed in paths.
    /// </value>
    public static Char[] InvalidPathChars {
      get {
        if (Path.invalidPathChars == null)
          Path.invalidPathChars = System.IO.Path.GetInvalidPathChars();

        return Path.invalidPathChars;
      }
    }
    #endregion

    #region Static Property: PathSeparator
    /// <summary>
    ///   Gets the character for separating paths in environment variables.
    /// </summary>
    /// <value>
    ///   The character for separating paths in environment variables.
    /// </value>
    public static Char PathSeparator {
      get { return System.IO.Path.PathSeparator; }
    }
    #endregion

    #region Static Property: VolumeSeparator
    /// <summary>
    ///   Gets the character used to separate the volume character from the path.
    /// </summary>
    /// <value>
    ///   The character used to separate the volume character from the path.
    /// </value>
    public static Char VolumeSeparator {
      get { return System.IO.Path.VolumeSeparatorChar; }
    }
    #endregion

    #region Static Property: DirectorySeparator
    /// <summary>
    ///   Gets the character used to separate multiple directories in a path.
    /// </summary>
    /// <value>
    ///   The character used to separate multiple directories in a path.
    /// </value>
    /// <seealso cref="AltDirectorySeparator">AltDirectorySeparator Property</seealso>
    public static Char DirectorySeparator {
      get { return System.IO.Path.DirectorySeparatorChar; }
    }
    #endregion

    #region Static Property: AltDirectorySeparator
    /// <summary>
    ///   Gets the alternative character used to separate multiple directories in a path.
    /// </summary>
    /// <value>
    ///   The alternative character used to separate multiple directories in a path.
    /// </value>
    /// <seealso cref="DirectorySeparator">DirectorySeparator Property</seealso>
    public static Char AltDirectorySeparator {
      get { return System.IO.Path.AltDirectorySeparatorChar; }
    }
    #endregion

    #region Property: RootDirectory
    /// <summary>
    ///   Gets the <see cref="Path" /> of the root directory of this path.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If no root directory is found, because this path is a relative path, then a <see cref="PathException" /> is thrown.
    ///   </para>
    ///   <para>
    ///     It is recommended to use this property to get the root path instead of creating a new <see cref="Path" /> structure
    ///     object on your own, because no path validation is perfromed for paths created internally by this structure.
    ///   </para>
    /// </remarks>
    /// <value>
    ///   The <see cref="Path" /> of the root directory of this path.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is relative or <see cref="None" />.
    /// </exception>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Path RootDirectory {
      get { 
        if (this == Path.None) throw new PathException(this);
        if (this.IsRelative) throw new PathException(this);
        Contract.Ensures(Contract.Result<Path>() != Path.None);

        String pathString = null;

        if (this.value.Length >= 1 && Path.IsDirectorySeparator(this.value[0])) {
          if (this.value.Length >= 2 && Path.IsDirectorySeparator(this.value[1])) {
            // Remote path root like "\\ComputerName"
            Int32 index = this.value.IndexOfAny(new[] { Path.DirectorySeparator, Path.AltDirectorySeparator }, 2);
            if (index != -1)
              pathString = this.value.Substring(0, index).TrimEnd();
            else
              pathString = this.value.TrimEnd();
          } else if (Path.IsDirectorySeparator(this.value[0])) {
            // Only a slash always has the same meaning as "C:\"
            pathString = @"C:\";
          }
        } else {
          // Usual root like "C:"
          if (this.value.Length >= 3 && Path.IsDirectorySeparator(this.value[2]))
            pathString = this.value.Substring(0, 3);
          else
            pathString = this.value.Substring(0, 2) + '\\';
        }
        
        return new Path(pathString, false, true);
      }
    }
    #endregion

    #region Property: ParentDirectory
    /// <summary>
    ///   Gets the <see cref="Path" /> of the parent directory.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If this path referes to a file, the directory containing that file will be returned. If no parent directory is 
    ///     found, because the path referes to the root directory of an volume or a relative path defines no more directories 
    ///     on its left side, then a <see cref="PathException" /> is thrown.
    ///   </para>
    ///   <para>
    ///     It is recommended to use this property to get the parent path instead of creating a new <see cref="Path" /> 
    ///     structure object on your own, because no path validation is perfromed for paths created internally by this structure.
    ///   </para>
    /// </remarks>
    /// <value>
    ///   The <see cref="Path" /> of the parent directory.
    /// </value>
    /// <exception cref="PathException">
    ///   The path has no parent directory or is <see cref="None" />.
    /// </exception>
    /// <seealso cref="None">None Property</seealso>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Path ParentDirectory {
      get {
        if (this == Path.None) throw new PathException(this);
        Contract.Ensures(Contract.Result<Path>() != Path.None);

        String newValue = Path.GetParentDirInternal(this);
        if (newValue == null)
          throw new PathException(this.value, "This path has no parent directory.");

        return new Path(newValue, this.IsRelative, true);
      }
    }
    #endregion

    #region Property: FileName
    /// <summary>
    ///   Gets the a new path containing the name of the file with its extension where this path refers to.
    /// </summary>
    /// <remarks>
    ///   If the path refers to a directory, the directory name will be returned. If this path is <see cref="None" /> then a
    ///   <see cref="PathException" /> will be thrown.
    /// </remarks>
    /// <value>
    ///   The name of the file where this path refers to.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is <see cref="None" />.
    /// </exception>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Path FileName {
      get { 
        if (this == Path.None) throw new PathException(this);
        Contract.Ensures(Contract.Result<Path>() != Path.None);

        return new Path(System.IO.Path.GetFileName(this.value), true, true);
      }
    }
    #endregion

    #region Property: FileNameWithoutExt
    /// <summary>
    ///   Gets the a new path containing the name of the file without its extension where this path refers to.
    /// </summary>
    /// <remarks>
    ///   If the path refers to a directory, the directory name will be returned. If this path is <see cref="None" /> then a
    ///   <see cref="PathException" /> will be thrown.
    /// </remarks>
    /// <value>
    ///   The a new path containing the name of the file without its extension where this path refers to.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is <see cref="None" />.
    /// </exception>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Path FileNameWithoutExt {
      get { 
        if (this == Path.None) throw new PathException(this);
        Contract.Ensures(Contract.Result<Path>() != Path.None);

        return new Path(System.IO.Path.GetFileNameWithoutExtension(this.value), true, true);
      }
    }
    #endregion

    #region Property: HasParentDirectory
    /// <summary>
    ///   <inheritdoc cref="HasParentDirectory" select='../value/node()' />
    /// </summary>
    private readonly Boolean hasParentDirectory;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether this path represents a single file system element or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this path represents a single file system element or not.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is <see cref="None" />.
    /// </exception>
    /// <seealso cref="None">None Property</seealso>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Boolean HasParentDirectory {
      get {
        if (this == Path.None) throw new PathException(this);
        return this.hasParentDirectory;
      }
    }
    #endregion

    #region Property: IsRelative
    /// <summary>
    ///   <inheritdoc cref="IsRelative" select='../value/node()' />
    /// </summary>
    private readonly Boolean isRelative;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether this path is relative or a rooted path.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether this path is relative or a rooted path.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is <see cref="None" />.
    /// </exception>
    /// <seealso cref="None">None Property</seealso>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Boolean IsRelative {
      get {
        if (this == Path.None) throw new PathException(this);
        return this.isRelative;
      }
    }
    #endregion

    #region Property: IsNormalized
    /// <summary>
    ///   <inheritdoc cref="IsNormalized" select='../value/node()' />
    /// </summary>
    private readonly Boolean isNormalized;

    /// <summary>
    ///   Gets a <see cref="Boolean" /> indicating whether the path is normalized or not.
    /// </summary>
    /// <value>
    ///   A <see cref="Boolean" /> indicating whether the path is normalized or not.
    /// </value>
    /// <exception cref="PathException">
    ///   The path is <see cref="None" />.
    /// </exception>
    /// <seealso cref="None">None Property</seealso>
    /// <seealso cref="PathException">PathException Class</seealso>
    public Boolean IsNormalized {
      get {
        if (this == Path.None) throw new PathException(this);
        return this.isNormalized;
      }
    }
    #endregion

    #region Property: Length
    /// <summary>
    ///   Gets the zero based length of the path.
    /// </summary>
    /// <value>
    ///   The zero based length of the path. <c>0</c> if the path is <see cref="None" />.
    /// </value>
    public Int32 Length {
      get { 
        Contract.Ensures(Contract.Result<Int32>() >= 0);

        if (this == Path.None)
          return 0;

        return this.value.Length;
      }
    }
    #endregion


    #region Methods: Constructors
    /// <summary>
    ///   Initializes the <see cref="Path" /> structure with a path value which is expected to be a valid path.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If you plan to combine multiple strings or other <see cref="Path" /> object to one path use the either the 
    ///     <see cref="Concat(String[])" /> or <see cref="Concat(Path[])" /> method.
    ///   </para>
    /// </remarks>
    /// <param name="value">
    ///   The path string representing the path.
    /// </param>
    /// <param name="isRelative">
    ///   A <see cref="Boolean" /> indicating whether the new path is relative or not.
    /// </param>
    /// <param name="isValidated">
    ///   Always has to be <c>false</c>. If you want path validation use the other constructor.
    /// </param>
    /// <seealso cref="Concat(String[])">Concat(String) Method</seealso>
    /// <seealso cref="Concat(Path[])">Concat(Path) Method</seealso>
    internal Path(String value, Boolean isRelative, Boolean isValidated) {
      // Should this be an empty path?
      if (value == null) {
        this.isRelative = false;
        this.isNormalized = false;
        this.hasParentDirectory = false;
        this.value = null;
        return;
      }

      this.value = value;
      this.isRelative = isRelative;
      this.isNormalized = !Path.pathCheckIsNotNormalizedRegex.IsMatch(value);;
      this.hasParentDirectory = false;
      this.hasParentDirectory = (Path.GetParentDirInternal(this) != null);
    }

    /// <summary>
    ///   Initializes the <see cref="Path" /> structure with the given <paramref name="value" /> used as path string.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The constructor uses the <see cref="System.IO.Path.GetFullPath" /> function internally which accesses the file 
    ///     system to check the security behaviour of an absolute path. For more documentation refer to the
    ///     <see cref="System.IO.Path.GetFullPath">System.IO.Path.GetFullPath Method</see>.
    ///   </para>
    /// </remarks>
    /// <param name="value">
    ///   The path string representing the path.
    /// </param>
    /// <param name="getFullPath">
    ///   A <see cref="Boolean" /> indicating whether the absolute path should be resolved for the given path string.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The given <paramref name="value" /> represents an invalid path because its empty, contains white spaces only,
    ///   contains invalid characters, is a path to directly to a physical drive or has an invalid format.
    /// </exception>
    /// <exception cref="System.IO.PathTooLongException">
    ///   <paramref name="value" /> is an absolute path and is too long or <paramref name="getFullPath" /> is <c>true</c> and 
    ///   the resolved path is too long. A valid path may be at most 247 characters long, a valid file name 259 characters.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   <paramref name="value" /> is an absolute path or <paramref name="getFullPath" /> is <c>true</c> and the file where 
    ///   this path refers to does exist in the file system and the caller is missing the 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery" /> right to access the full path of this file.
    /// </exception>
    /// <permission cref="FileIOPermission">
    ///   <see cref="FileIOPermission" /> to discover the path string in case the path is rooted.
    ///   Associated enumeration: <see cref="FileIOPermissionAccess.PathDiscovery" />.
    /// </permission>
    /// <seealso cref="System.IO.Path.GetFullPath">System.IO.Path.GetFullPath Method</seealso>
    /// <seealso cref="FileIOPermissionAccess">FileIOPermissionAccess Enumeration</seealso>
    public Path(String value, Boolean getFullPath = false) {
      // Should this be an empty path?
      if (value == null) {
        this.hasParentDirectory = false;
        this.isRelative = false;
        this.isNormalized = false;
        this.value = null;
        return;
      }

      String trimmedValue = value.Trim();
      this.isRelative = !(getFullPath || System.IO.Path.IsPathRooted(trimmedValue));
      this.value = value;

      // We do the path check by using the System.IO.Path class.
      try {
        if (getFullPath) {
          this.value = System.IO.Path.GetFullPath(value);
        } else {
          if (!this.isRelative)
            this.value = trimmedValue;

          // GetFullPath throws an Exception for network paths containing a root only (like "\\Root", "\\Root\." or "\\Root\.."), 
          // but this structure should be able to handle those path's anyway, so we have to manually check and validate them.
          if (Path.IsRootOnlyNetworkPath(trimmedValue)) {
            Char[] invalidPathChars = Path.InvalidPathChars;

            if (trimmedValue.IndexOfAny(invalidPathChars) != -1)
              throw new ArgumentException("The path contains invalid characters.", "value");

            this.value = trimmedValue;
          } else {
            // Just valiate, don't use the returned value at all.
            System.IO.Path.GetFullPath(value);
          }
        }
      } catch (System.IO.PathTooLongException) {
        // If this is relative path, then its full path will be determined later and therefore have a different length.
        if (!this.isRelative)
          throw;
      } catch (NotSupportedException exception) {
        // If this is relative path, then its full path will be determined later and therefore have a different length.
        throw new ArgumentException("The path defines an invalid root.", value, exception);
      }

      this.value = this.value.TrimEnd('\\', '/');
      this.isNormalized = !Path.pathCheckIsNotNormalizedRegex.IsMatch(value);
      this.hasParentDirectory = false;
      this.hasParentDirectory = (Path.GetParentDirInternal(this) != null);
    }
    #endregion

    #region Static Methods: Operator +
    /// <summary>
    ///   Concats two <see cref="Path" /> objects to a new one, separating them with a <see cref="DirectorySeparator" />.
    /// </summary>
    /// <remarks>
    ///   If one of both provided <see cref="Path" /> objects is <see cref="None" />, then the one which is not 
    ///   <see cref="None" /> will be returned. If both provided <see cref="Path" /> objects are <see cref="None" />, then
    ///   <see cref="None" /> will be returned aswell.
    /// </remarks>
    /// <param name="left">
    ///   The first <see cref="Path" /> to be concat.
    /// </param>
    /// <param name="right">
    ///   The second <see cref="Path" /> to be concat.
    /// </param>
    /// <returns>
    ///   The resulting concatenated <see cref="Path" />.
    /// </returns>
    /// <exception cref="PathException">
    ///   <paramref name="right" /> is a rooted path.
    /// </exception>
    [Pure]
    public static Path operator +(Path left, Path right) {
      if (left.value == null || right.value == null) {
        if (left.value == null)
          return right;

        return left;
      }

      if (!right.IsRelative)
        throw new PathException("The right operand of the expression is a rooted path.");
      
      return new Path(String.Concat(left.value, Path.DirectorySeparator, right.value), left.IsRelative, true);
    }

    /// <summary>
    ///   Concats a <see cref="Path" /> object with a <see cref="String" /> and creates a new <see cref="Path" /> object by
    ///   separating them with a <see cref="DirectorySeparator" />.
    /// </summary>
    /// <remarks>
    ///   If <paramref name="path" /> is <see cref="None" />, then the method will try to use the <paramref name="pathString" />
    ///   to create the new <see cref="Path" /> object.
    /// </remarks>
    /// <param name="path">
    ///   The <see cref="Path" /> to be concat.
    /// </param>
    /// <param name="pathString">
    ///   The string to be concat with the <see cref="Path" />.
    /// </param>
    /// <returns>
    ///   The resulting concatenated <see cref="Path" />.
    /// </returns>
    /// <exception cref="PathException">
    ///   The built path is invalid. See its inner exception for details.
    /// </exception>
    [Pure]
    public static Path operator +(Path path, String pathString) {
      if (path.value == null) {
        try {
          return new Path(pathString);
        } catch (Exception exception) {
          throw new PathException(pathString, "The built path is invalid. See its inner exception for details.", exception);
        }
      }

      String value = String.Concat(path.value, Path.DirectorySeparator, pathString);
      try {
        return new Path(value);
      } catch (Exception exception) {
        throw new PathException(value, "The built path is invalid. See its inner exception for details.", exception);
      }
    }

    /// <summary>
    ///   Concats a <see cref="String" /> with a <see cref="Path" /> objects and creates a new <see cref="Path" /> object by
    ///   separating them with a <see cref="DirectorySeparator" />.
    /// </summary>
    /// <exception cref="PathException">
    ///   <paramref name="path" /> is rooted or the built path is invalid.
    /// </exception>
    /// <inheritdoc />
    [Pure]
    public static Path operator +(String pathString, Path path) {
      if (path.value == null) {
        try {
          return new Path(pathString);
        } catch (Exception exception) {
          throw new PathException(pathString, "The built path is invalid. See its inner exception for details.", exception);
        }
      }

      if (!path.IsRelative) {
        throw new PathException(path.value, "The path can not be rooted.");
      }

      String value = String.Concat(pathString, Path.DirectorySeparator, path.value);
      try {
        return new Path(value);
      } catch (Exception exception) {
        throw new PathException(value, "The built path is invalid. See its inner exception for details.", exception);
      }
    }
    #endregion

    #region Static Methods: Implicit/Explicit Casts
    /// <summary>
    ///   Implicitly converts <see cref="Path" /> to <see cref="String" />.
    /// </summary>
    /// <param name="path">
    ///   The <see cref="Path" /> to convert.
    /// </param>
    /// <returns>
    ///   The <see cref="String" /> containing the path.
    /// </returns>
    [Pure]
    public static implicit operator String(Path path) {
      return path.value;
    }

    /// <summary>
    ///   Explicitly converts <see cref="String" /> to <see cref="Path" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="String" /> to convert.
    /// </param>
    /// <returns>
    ///   The converted <see cref="String" />.
    /// </returns>
    /// <exception cref="PathException">
    ///   <paramref name="value" /> is not a valid path. See inner exception for more details.
    /// </exception>
    [Pure]
    public static explicit operator Path(String value) {
      try {
        return new Path(value);
      } catch (Exception exception) {
        throw new PathException(
          value, "The path represented by the string is invalid. See inner exception for more details.", exception);
      }
    }
    #endregion

    #region Static Methods: Concat
    /// <summary>
    ///   Combines multiple <see cref="Path" /> structures to one by separating them with <see cref="DirectorySeparator" /> 
    ///   characters.
    /// </summary>
    /// <remarks>
    ///   The <see cref="Path" /> objects may either be all relative paths or the first <see cref="Path" /> in the array may be
    ///   a rooted path. If the array contains multiple rooted paths or a relative path is provided before a rooted path or a
    ///   <see cref="Path" /> is <see cref="None" />, then a <see cref="PathException" /> is thrown.
    /// </remarks>
    /// <param name="paths">
    ///   The <see cref="Path" /> structures to be combined.
    /// </param>
    /// <returns>
    ///   The new combined <see cref="Path" /> structure.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="paths" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="paths" /> is an empty array.
    /// </exception>
    /// <exception cref="PathException">
    ///   A <see cref="Path" /> is rooted but its not the first element in the array or the array contains multiple rooted 
    ///   paths or a <see cref="None" /> path.
    /// </exception>
    /// <seealso cref="PathException">PathException Class</seealso>
    /// 
    /// <overloads>
    ///   <summary>
    ///     Combines multiple path elements to one new <see cref="Path" /> object by separating them with 
    ///     <see cref="DirectorySeparator" /> characters.
    ///   </summary>
    /// </overloads>
    [Pure]
    public static Path Concat(params Path[] paths) {
      if (paths == null) throw new ArgumentNullException();
      if (paths.Length == 0) throw new ArgumentOutOfRangeException();
      Contract.Ensures(Contract.Result<Path>() != Path.None);

      StringBuilder valueBuilder = new StringBuilder(paths.Length * 20);
      Char separator = Path.DirectorySeparator;
      Boolean firstIsRooted = !paths[0].IsRelative;

      for (Int32 i = 0; i < paths.Length; i++) {
        Path path = paths[i];

        if (path.value == null)
          throw new PathException("An empty path is given in the path array.");

        if (i > 0) {
          if (!path.IsRelative) {
            if (!firstIsRooted)
              throw new PathException(path.value, "The path is rooted but it is not the first path in the array.");

            throw new PathException(path.value, "The path array contains multiple rooted paths.");
          }

          if (!Path.IsDirectorySeparator(path.value[0]))
            valueBuilder.Append(separator);
        }

        valueBuilder.Append(path.value);
      }

      return new Path(valueBuilder.ToString(), paths[0].IsRelative, true);
    }

    /// <summary>
    ///   Combines multiple <see cref="String" /> objects to one new <see cref="Path" /> object by separating them with 
    ///   <see cref="DirectorySeparator" /> characters.
    /// </summary>
    /// <param name="pathStrings">
    ///   The <see cref="String" /> objects to be combined.
    /// </param>
    /// <returns>
    ///   The new combined <see cref="Path" /> structure.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="pathStrings" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="pathStrings" /> is an empty array.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="pathStrings" /> array contains either a <c>null</c> or empty <see cref="String" />.
    /// </exception>
    /// <exception cref="PathException">
    ///   The built path is invalid. See its inner exception for details.
    /// </exception>
    /// <seealso cref="PathException">PathException Class</seealso>
    [Pure]
    public static Path Concat(params String[] pathStrings) {
      if (pathStrings == null) throw new ArgumentNullException();
      if (pathStrings.Length == 0) throw new ArgumentOutOfRangeException();
      Contract.Ensures(Contract.Result<Path>() != Path.None);

      StringBuilder valueBuilder = new StringBuilder(pathStrings.Length * 20);
      Char separator = Path.DirectorySeparator;
      for (Int32 i = 0; i < pathStrings.Length; i++) {
        String pathString = pathStrings[i];

        if (String.IsNullOrEmpty(pathString))
          throw new ArgumentException("One of the strings in the array was either null or empty.");

        if (i > 0 && !Path.IsDirectorySeparator(pathString[0]))
          valueBuilder.Append(separator);

        valueBuilder.Append(pathString);
      }

      String value = valueBuilder.ToString();
      try {
        return new Path(value);
      } catch (Exception exception) {
        throw new PathException(value, "The built path is invalid. See its inner exception for details.", exception);
      }
    }
    #endregion

    #region Static Methods: GetParentDirInternal, IsRootOnlyNetworkPath
    /// <summary>
    ///   Retrieves the parent directory of the given path string while considering relative path patterns like "\.." and "\.".
    /// </summary>
    /// <remarks>
    ///   The given <paramref name="path" /> is not validated by this method.
    /// </remarks>
    /// <param name="path">
    ///   The path to get the parent directory path of.
    /// </param>
    /// <returns>
    ///   A <see cref="String" /> containing the path of the parent directory or <c>null</c> if no parent directory was found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="path" /> is <c>null</c>.
    /// </exception>
    [Pure]
    private static String GetParentDirInternal(String path) {
      if (path == null)
        return null;

      if (path.Length <= 2) {
        // Only a slash always has the same meaning as "C:\"
        if ((path.Length == 1 && path[0] == '\\') || path == "\\.")
          return @"C:\";

        return path;
      }

      Int32 blockLength = 0;
      Int32 blocksToIgnore = 1; // Since we want the parent directory, the first block may not be counted.
      String pathString = path;

      for (Int32 i = pathString.Length - 1; i >= 0; i--) {
        Char chr = pathString[i];
        // This check prevents whitespaces after any chars from being counted at all. "Abc\Def   " => "Abc\Def"
        if (chr == ' ' && blockLength == 0)
          continue;

        // New block?
        if (Path.IsDirectorySeparator(chr)) {
          // Prevent any empty blocks or whitespace only blocks from being considered. "Abc\  \Def\\Ghi" => "Abc\Def\Ghi"
          if (blockLength == 0)
            continue;

          // Check if the block we just stepped over was "." or "..".
          if (pathString[i + 1] == '.') {
            if (pathString.Length - i >= 3 && pathString[i + 2] == '.') {
              // ".." means we have to jump over the next block.
              blocksToIgnore++;
            }
            blockLength = 0;

            // "." means do nothing and process with the next block.
            continue;
          }

          // This is a real directory block, so check whether to jump over or take it.
          if (blocksToIgnore > 0) {
            blocksToIgnore--;
            blockLength = 0;
            continue;
          }

          // Take it as parent directory.
          return pathString.Substring(0, i + blockLength + 1);
        }
        
        // Begin of path and no directory separator, also no blocks to ignore.
        if (i == 0 && blocksToIgnore == 0) {
          return pathString.Substring(0, i + blockLength + 1);
        }

        blockLength++;
      }

      // No parent directory found.
      return null;
    }

    /// <summary>
    ///   Checks whether a given path string is a network path with a root definition only.
    /// </summary>
    /// <remarks>
    ///   The given <paramref name="path" /> is not validated or trimmed by this method.
    /// </remarks>
    /// <param name="path">
    ///   The path to check.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating whether this path is a network path with a root definition only or not.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="path" /> is <c>null</c>.
    /// </exception>
    [Pure]
    private static Boolean IsRootOnlyNetworkPath(String path) {
      if (path == null) throw new ArgumentNullException();

      if (path.Length < 3)
        return false;

      if (Path.IsDirectorySeparator(path[0]) && Path.IsDirectorySeparator(path[1]))
        return Path.GetParentDirInternal(path) == null;

      return false;
    }

    /// <summary>
    ///   Checks whether a character is <see cref="Path.DirectorySeparator" /> or <see cref="Path.AltDirectorySeparator" />.
    /// </summary>
    /// <param name="character">
    ///   The character to be checked.
    /// </param>
    /// <returns>
    ///   A <see cref="Boolean" /> indicating if <paramref name="character" /> is either 
    ///   <see cref="Path.DirectorySeparator" /> or <see cref="Path.AltDirectorySeparator" />.
    /// </returns>
    [Pure]
    private static Boolean IsDirectorySeparator(Char character) {
      return (character == Path.DirectorySeparator || character == Path.AltDirectorySeparator);
    }
    #endregion

    #region Methods: GetFullPath, ResolveEnvironmentVariables, ToString
    /// <summary>
    ///   Gets the absolute path of the path represented by this structure.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This method uses the <see cref="System.IO.Path.GetFullPath" /> function internally which accesses the file system
    ///     to check the security behaviour of the absolute path. For more documentation refer to the
    ///     <see cref="System.IO.Path.GetFullPath">System.IO.Path.GetFullPath Method</see>.
    ///   </para>
    ///   <para>
    ///     It is recommended to use this methods to get the full path instead of creating a new <see cref="Path" /> structure
    ///     object on your own, because no path validation is perfromed for paths created internally by this structure.
    ///   </para>
    /// </remarks>
    /// <exception cref="PathException">
    ///   The system could not retrieve the absolute path or the path is <see cref="None" />.
    /// </exception>
    /// <exception cref="SecurityException">
    ///   The file where this path refers to does exist in the file system and the caller is missing the 
    ///   <see cref="FileIOPermissionAccess.PathDiscovery" /> right to access the full path of this file.
    /// </exception>
    /// <permission cref="FileIOPermission">
    ///   <see cref="FileIOPermission" /> to discover the path. 
    ///   Associated enumeration: <see cref="FileIOPermissionAccess.PathDiscovery" />.
    /// </permission>
    /// <exception cref="System.IO.PathTooLongException">
    ///   The resolved path is too long. A valid path may be at most 247 characters long, a valid file name 259 characters.
    /// </exception>
    /// <seealso cref="System.IO.Path.GetFullPath">System.IO.Path.GetFullPath Method</seealso>
    /// <seealso cref="FileIOPermissionAccess">FileIOPermissionAccess Enumeration</seealso>
    [Pure]
    public Path GetFullPath() {
      if (this == Path.None) throw new PathException(this);
      Contract.Ensures(Contract.Result<Path>() != Path.None);

      if (!this.IsRelative)
        return this;

      String newValue;
      try {
        newValue = System.IO.Path.GetFullPath(this.value);
      } catch (ArgumentException exception) {
        throw new PathException(
          this.value, "The system could not retrieve the absolute path. See inner exception for more details.", exception);
      }

      return new Path(newValue, false, true);
    }

    /// <summary>
    ///   Not implemented yet.
    /// </summary>
    [Pure]
    public Path ResolveEnvironmentVariables() {
      // TODO: Implement this!
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    [Pure]
    public override String ToString() {
      if (this.value != null)
        return this.value;

      return String.Empty;
    }
    #endregion

    #region IComparable Implementation
    /// <inheritdoc />
    [Pure]
    public Int32 CompareTo(Path other) {
      return this.value.CompareTo(other.value);
    }

    /// <inheritdoc />
    [Pure]
    public Int32 CompareTo(Object other) {
      //if (other is Path || other is String) throw new ArgumentException();

      if (other is Path)
        return this.value.CompareTo(((Path)other).value);

      return this.value.CompareTo((String)other);
    }
    #endregion

    #region IEquatable Implementation
    /// <inheritdoc />
    [Pure]
    public Boolean Equals(Object other, StringComparison comparisonType) {
      if (!Enum.IsDefined(typeof(StringComparison), comparisonType)) throw new ArgumentException();

      String otherValue;

      if (other is Path)
        otherValue = ((Path)other).value;
      else if (other is String)
        otherValue = (String)other;
      else
        return false;


      return this.value.Equals(otherValue, comparisonType);
    }

    /// <inheritdoc />
    [Pure]
    public Boolean Equals(Path other) {
      return this.value.Equals(other.value);
    }

    /// <inheritdoc />
    [Pure]
    public override Boolean Equals(Object other) {
      return this.Equals(other, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <inheritdoc />
    [Pure]
    public override Int32 GetHashCode() {
      // TODO: This is invalid, fix it!
      if (this.value == null)
        return 0;

      return this.value.GetHashCode();
    }
    #endregion
  }
}

