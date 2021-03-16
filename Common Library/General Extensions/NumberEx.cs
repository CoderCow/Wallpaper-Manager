using System;

namespace Common {
  /// <threadsafety static="false" instance="false" />
  public static class NumberEx {
    // Note: Don't use IComparable instead of explicit types here.
    public static Boolean IsBetween(this Byte @this, Byte min, Byte max) {
      return ((@this.CompareTo(min) >= 0) && (@this.CompareTo(max) <= 0));
    }

    public static Boolean IsBetween(this Int32 @this, Int32 min, Int32 max) {
      return ((@this.CompareTo(min) >= 0) && (@this.CompareTo(max) <= 0));
    }
  }
}
