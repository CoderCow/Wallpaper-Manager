using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;

namespace UnitTests {
  public class DisplayStub : IDisplay {
    public string UniqueDeviceId { get; set; }
    public bool IsRemovable { get; set; }
    public string DeviceName { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle WorkingArea { get; set; }
    public bool IsPrimary { get; set; }
    public int BitsPerPixel { get; set; }

    public override string ToString() {
      return $"{nameof(this.DeviceName)}: {this.DeviceName}, {nameof(this.IsPrimary)}: {this.IsPrimary}";
    }
  }
}
