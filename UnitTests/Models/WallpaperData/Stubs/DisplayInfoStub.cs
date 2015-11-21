using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;

namespace UnitTests {
  public class DisplayInfoStub: IDisplayInfo {
    public event DisplaysChanged DisplaysChanged;

    public IDisplay PrimaryDisplay { get; set; }
    public ReadOnlyCollection<IDisplay> Displays { get; set; }
    public bool IsMultiDisplaySystem { get; set; }

    public void RaiseDisplaysChanged() {
      this.DisplaysChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose() {
      throw new NotImplementedException();
    }
  }
}
