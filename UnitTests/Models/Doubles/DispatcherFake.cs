using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WallpaperManager.Models;

namespace UnitTests.Models {
  public class DispatcherFake : IDispatcher {
    public int BeginInvokeCount { get; private set; }
    public int InvokeCount { get; private set; }

    public void BeginInvoke(DispatcherPriority priority, Delegate method, params object[] args) {
      method.DynamicInvoke(args);
      this.BeginInvokeCount++;
    }

    public void BeginInvoke(Delegate method, params object[] args) {
      method.DynamicInvoke(args);
      this.BeginInvokeCount++;
    }

    public void Invoke(DispatcherPriority priority, Action callback, CancellationToken cancellationToken = new CancellationToken(), TimeSpan timeout = new TimeSpan()) {
      callback();
      this.InvokeCount++;
    }

    public void Invoke(Action callback, CancellationToken cancellationToken = new CancellationToken(), TimeSpan timeout = new TimeSpan()) {
      callback();
      this.InvokeCount++;
    }
  }
}
