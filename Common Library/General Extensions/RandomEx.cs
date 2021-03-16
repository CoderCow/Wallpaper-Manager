using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
  public static class RandomEx {
    public static Boolean NextBoolean(this Random @this) {
      return (@this.Next(0, 2) == 1);
    }

    public static Percentage NextPercentage(this Random @this) {
      return new Percentage(Convert.ToSingle(@this.NextDouble()));
    }
  }
}
