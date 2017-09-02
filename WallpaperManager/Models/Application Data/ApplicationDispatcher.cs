using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WallpaperManager.Models {
  public class ApplicationDispatcher : IDispatcher {
    public void BeginInvoke(DispatcherPriority priority, Delegate method, params object[] args) {
      Dispatcher.CurrentDispatcher.BeginInvoke(priority, method, args);
    }

    public void BeginInvoke(Delegate method, params object[] args) {
      Dispatcher.CurrentDispatcher.BeginInvoke(method, args);
    }

    public void Invoke(DispatcherPriority priority, Action callback, CancellationToken cancellationToken = default(CancellationToken), TimeSpan timeout = default(TimeSpan)) {
      bool isTokenGiven = (cancellationToken == default(CancellationToken));
      bool isTimeoutGiven = (timeout == default(TimeSpan));

      if (isTimeoutGiven && isTokenGiven)
        Dispatcher.CurrentDispatcher.Invoke(callback, priority, cancellationToken, timeout);
      else if (isTokenGiven)
        Dispatcher.CurrentDispatcher.Invoke(callback, priority, cancellationToken);
      else if (isTimeoutGiven)
        Dispatcher.CurrentDispatcher.Invoke(priority, timeout, callback);
      else
        Dispatcher.CurrentDispatcher.Invoke(callback);
    }

    public void Invoke(Action callback, CancellationToken cancellationToken = new CancellationToken(), TimeSpan timeout = new TimeSpan()) {
      this.Invoke(DispatcherPriority.Normal, callback, cancellationToken, timeout);
    }
  }
}
