using System;
using System.IO;
using System.Security.Cryptography;

namespace Common {
  public static class MD5Ex {
    public static String ComputeHashString(this MD5 md5, Byte[] buffer) {
      return MD5Ex.HashDataToString(md5.ComputeHash(buffer));
    }

    public static String ComputeHashString(this MD5 md5, Byte[] buffer, Int32 offset, Int32 count) {
      return MD5Ex.HashDataToString(md5.ComputeHash(buffer, offset, count));
    }

    public static String ComputeHashString(this MD5 md5, Stream inputStream) {
      return MD5Ex.HashDataToString(md5.ComputeHash(inputStream));
    }

    private static String HashDataToString(Byte[] data) {
      Char[] chars = new Char[data.Length * 2];

      for (Int32 y = 0, x = 0; y < data.Length; y++, x++) {
        Byte b = ((Byte)(data[y] >> 4));
        chars[x] = (Char)(b > 9 ? b + 0x37 : b + 0x30);
        b = ((Byte)(data[y] & 0xF));
        chars[++x] = (Char)(b > 9 ? b + 0x37 : b + 0x30);
      }

      return new String(chars);
    }
  }
}