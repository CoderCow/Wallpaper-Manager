using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;

namespace Common.Security.Cryptography {
  public sealed class CRC32: HashAlgorithm {
    #region Fields: hash, table
    private UInt32 hash;
    private UInt32[] table;
    #endregion

    #region Property: Seed
    private readonly UInt32 seed;

    public UInt32 Seed {
      get { return this.seed; }
    }
    #endregion

    #region Property: Polynomial
    private readonly UInt32 polynomial;

    public UInt32 Polynomial {
      get { return this.polynomial; }
    }
    #endregion

    #region Property: HashSize
    public override Int32 HashSize {
      get { return 32; }
    }
    #endregion


    #region Method: Constructor, Initialize
    public CRC32(UInt32 polynomial = 0xEDB88320, UInt32 seed = 0xFFFFFFFF) {
      this.polynomial = polynomial;
      this.seed = seed;
      this.Initialize();
    }

    public override void Initialize() {
      if (this.IsDisposed) throw new ObjectDisposedException("this");

      this.hash = this.seed;

      if (this.table != null)
        return;

      this.table = new UInt32[256];
      for (Int32 i = 0; i < 256; i++) {
        UInt32 entry = (UInt32)i;

        for (Int32 j = 0; j < 8; j++) {
          if ((entry & 1) == 1)
            entry = (entry >> 1) ^ this.Polynomial;
          else
            entry = entry >> 1;
        }

        this.table[i] = entry;
      }
    }
    #endregion

    #region Methods: ComputeInt32
    public Int32 ComputeHashInt32(Stream inputStream) {
      if (inputStream == null) throw new ArgumentNullException();
      if (!inputStream.CanRead) throw new ArgumentException();

      throw new NotImplementedException();
      return this.ComputeHashInt32Internal(this.ComputeHash(inputStream));
    }

    public Int32 ComputeHashInt32(Byte[] buffer, Int32 offset = 0, Int32 count = -1) {
      if (buffer == null) throw new ArgumentNullException();
      throw new NotImplementedException();
      if (count == -1)
        count = buffer.Length - offset;

      return this.ComputeHashInt32Internal(this.ComputeHash(buffer, offset, count));
    }

    private Int32 ComputeHashInt32Internal(Byte[] crcData) {
      throw new NotImplementedException();
    }
    #endregion

    #region Methods: HashCore, HashFinal, UInt32ToBigEndianBytes
    protected override void HashCore(Byte[] buffer, Int32 start, Int32 length) {
      if (this.IsDisposed) throw new ObjectDisposedException("this");
      
      this.hash = this.Seed;
      
      for (Int32 i = start; i < length; i++) {
        unchecked {
          this.hash = (this.hash >> 8) ^ this.table[buffer[i] ^ this.hash & 0xFF];
        }
      }
    }

    protected override Byte[] HashFinal() {
      if (this.IsDisposed) throw new ObjectDisposedException("this");

      return this.UInt32ToBigEndianBytes(~this.hash);
    }

    private Byte[] UInt32ToBigEndianBytes(UInt32 x) {
      return new[] {
			  (Byte)((x >> 24) & 0xFF),
			  (Byte)((x >> 16) & 0xFF),
			  (Byte)((x >> 8) & 0xFF),
			  (Byte)(x & 0xFF)
		  };
    }
    #endregion

    #region IDisposable Implementation
    private Boolean isDisposed;

    public Boolean IsDisposed {
      get { return this.isDisposed; }
    }

    protected override void Dispose(Boolean disposing) {
      if (!this.isDisposed && disposing)
        this.table = null;

      this.isDisposed = true;
    }

    ~CRC32() {
      this.Dispose(false);
    }
    #endregion
  }
}
