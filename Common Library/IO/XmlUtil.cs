using System;
using System.Diagnostics.Contracts;

namespace Common.IO {
  public static class XmlUtil {
    public static Boolean IsValidName(String xmlName) {
      if (xmlName == null) throw new ArgumentNullException();

      if (xmlName.Length == 0) {
        return false;
      }

      if (!XmlUtil.IsValidNameChar(xmlName[0], true)) {
        return false;
      }

      for (Int32 i = 1; i < xmlName.Length; i++) {
        if (!XmlUtil.IsValidNameChar(xmlName[i], false)) {
          return false;
        }
      }

      return true;
    }

    public static Boolean IsValidNameChar(Char chr, Boolean isFirst) {
      if (isFirst) {
        return (
          (chr >= 0x41 && chr <= 0x5A) || // A-Z
          (chr >= 0x61 && chr <= 0x7A) || // a-z
          (chr >= 0x61 && chr <= 0x7A) || // 0-9
          chr == ':' || chr == '_' || chr == '-' || chr == '.' || chr == 0xB7 ||
          (chr >= 0xC0 && chr <= 0xD6) ||
          (chr >= 0xD8 && chr <= 0xF6) ||
          (chr >= 0xF8 && chr <= 0x2FF) ||
          (chr >= 0x300 && chr <= 0x37D) ||
          (chr >= 0x37F && chr <= 0x1FFF) ||
          (chr >= 0x200C && chr <= 0x200D) ||
          (chr >= 0x2070 && chr <= 0x218F) ||
          (chr >= 0x2C00 && chr <= 0x2FEF) ||
          (chr >= 0x3001 && chr <= 0xD7FF) ||
          (chr >= 0xF900 && chr <= 0xFDCD) ||
          (chr >= 0xFDF0 && chr <= 0xFFFD) ||
          (chr >= 0x10000 && chr <= 0xEFFFF)
        );
      }

      return (
        (chr >= 0x41 && chr <= 0x5A) || // A-Z
        (chr >= 0x61 && chr <= 0x7A) || // a-z
        chr == ':' || chr == '_' || 
        (chr >= 0xC0 && chr <= 0xD6) ||
        (chr >= 0xD8 && chr <= 0xF6) ||
        (chr >= 0xF8 && chr <= 0x2FF) ||
        (chr >= 0x370 && chr <= 0x37D) ||
        (chr >= 0x37F && chr <= 0x1FFF) ||
        (chr >= 0x200C && chr <= 0x200D) ||
        (chr >= 0x2070 && chr <= 0x218F) ||
        (chr >= 0x2C00 && chr <= 0x2FEF) ||
        (chr >= 0x3001 && chr <= 0xD7FF) ||
        (chr >= 0xF900 && chr <= 0xFDCD) ||
        (chr >= 0xFDF0 && chr <= 0xFFFD) ||
        (chr >= 0x10000 && chr <= 0xEFFFF)
      );
    }

    public static Boolean IsValidAttributeValue(String xmlAttributeValue) {
      if (xmlAttributeValue == null) throw new ArgumentNullException();

      if (xmlAttributeValue.Length == 0) {
        return true;
      }

      for (Int32 i = 0; i < xmlAttributeValue.Length; i++) {
        Char chr = xmlAttributeValue[i];

        if (chr == '<') {
          return false;
        }

        // Entity?
        if (chr == '&') {
          if (i + 1 == xmlAttributeValue.Length)
            return false;
          
          chr = xmlAttributeValue[++i];

          Boolean isNameEntity = (chr != '#');
          Boolean isFirstChar = true;

          if (!isNameEntity) {
            if (i + 1 == xmlAttributeValue.Length) {
              return false;
            }

            ++i;
          }

          while (true) {
            i++;
            if (i >= xmlAttributeValue.Length) { // No valid entity because ; is missing.
              return false;
            }

            chr = xmlAttributeValue[i];
            if (chr == ';') {
              if (isFirstChar) {
                return false;
              }

              break;
            }

            // Is a valid name char or 0-9?
            if ((isNameEntity && !XmlUtil.IsValidNameChar(chr, isFirstChar)) || (chr >= 0x61 && chr <= 0x7A)) {
              return false;
            }

            isFirstChar = false;
          }

          return false;
        }
      }

      return true;
    }
  }
}