using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Common.IO {
  public struct FileInfo {
    public static FileInfo Invalid = default(FileInfo);

    #region Property: Path
    private Path path;

    public Path Path {
      get {
        Contract.Ensures(Contract.Result<Path>() != Path.None);
        return this.path;
      }
      set {
        if (value == Path.None) throw new ArgumentException();
        this.path = value;
      }
    }
    #endregion

    #region Property: Exists
    private Boolean exists;

    public Boolean Exists {
      get { return this.exists; }
      set { this.exists = value; }
    }
    #endregion

    #region Property: Size
    private Int64 size;

    public Int64 Size {
      get {
        Contract.Ensures(Contract.Result<Int64>() >= 0);
        return this.size;
      }
      set {
        if (value < 0) throw new ArgumentOutOfRangeException();
        this.size = value;
      }
    }
    #endregion

    #region Property: Attributes, IsDirectory
    private FileAttributes attributes;

    public FileAttributes Attributes {
      get { return this.attributes; }
      set { this.attributes = value; }
    }

    public Boolean IsDirectory {
      get { return (this.Attributes & FileAttributes.Directory) != 0; }
    }
    #endregion

    #region Property: CreationTime
    private DateTime creationTime;

    public DateTime CreationTime {
      get { return this.creationTime; }
      set { this.creationTime = value; }
    }
    #endregion

    #region Property: LastAccessTime
    private DateTime lastAccessTime;

    public DateTime LastAccessTime {
      get { return this.lastAccessTime; }
      set { this.lastAccessTime = value; }
    }
    #endregion

    #region Property: LastWriteTime
    private DateTime lastWriteTime;

    public DateTime LastWriteTime {
      get { return this.lastWriteTime; }
      set { this.lastWriteTime = value; }
    }
    #endregion


    #region Static Methods: FromFile
    public static FileInfo FromFile(Path path) {
      if (path == Path.None) throw new ArgumentException();
      
      System.IO.FileInfo ioFileInfo = new System.IO.FileInfo(path);
      return new FileInfo {
        Path = path,
        Exists = ioFileInfo.Exists,
        Size = ioFileInfo.Length,
        Attributes = ioFileInfo.Attributes,
        CreationTime = ioFileInfo.CreationTime,
        LastAccessTime = ioFileInfo.LastAccessTime,
        LastWriteTime = ioFileInfo.LastWriteTime,
      };
    }
    #endregion

    #region Methods: Constructor, IntersectWith
    public FileInfo(Path path, Boolean exists = false, Int64 size = 0) {
      if (path == Path.None) throw new ArgumentException();

      this.path = path;
      this.exists = exists;
      this.size = size;
      this.attributes = FileAttributes.Normal;
      this.creationTime = DateTime.MinValue;
      this.lastAccessTime = DateTime.MinValue;
      this.lastWriteTime = DateTime.MinValue;
    }

    public FileInfo IntersectWith(FileInfo otherInfo) {
      if (this.Equals(FileInfo.Invalid))
        return otherInfo;

      if (this.Exists == otherInfo.Exists) {
        FileInfo newFileInfo = this;
        newFileInfo.Attributes = otherInfo.Attributes;
        newFileInfo.Size = otherInfo.Size;
        
        if (newFileInfo.CreationTime != DateTime.MinValue)
          newFileInfo.CreationTime = otherInfo.CreationTime;

        if (otherInfo.LastAccessTime != DateTime.MinValue)
          newFileInfo.LastAccessTime = otherInfo.LastAccessTime;

        if (otherInfo.LastWriteTime != DateTime.MinValue)
          newFileInfo.LastWriteTime = otherInfo.LastWriteTime;

        return newFileInfo;
      }

      if (!this.Exists)
        if (!otherInfo.Exists)
          return new FileInfo(otherInfo.Path);
        else
          return otherInfo;

      return this;
    }
    #endregion

    #region Methods: ToString
    public override String ToString() {
      return this.Path;
    }
    #endregion
  }
}
