using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WallpaperManager.Models {
  [ContractClass(typeof(IDispatcherContracts))]
  public interface IDispatcher {
    void BeginInvoke(DispatcherPriority priority, Delegate method, params object[] args);
    void BeginInvoke(Delegate method, params object[] args);
    void Invoke(DispatcherPriority priority, Action callback, CancellationToken cancellationToken = default(CancellationToken), TimeSpan timeout = default(TimeSpan));
    void Invoke(Action callback, CancellationToken cancellationToken = default(CancellationToken), TimeSpan timeout = default(TimeSpan));
  }

  [ContractClassFor(typeof(IDispatcher))]
  internal abstract class IDispatcherContracts: IDispatcher {
    public void BeginInvoke(DispatcherPriority priority, Delegate method, params object[] args) {
      Contract.Requires<ArgumentNullException>(method != null);

      throw new NotImplementedException();
    }

    public void BeginInvoke(Delegate method, params object[] args) {
      Contract.Requires<ArgumentNullException>(method != null);

      throw new NotImplementedException();
    }

    public void Invoke(DispatcherPriority priority, Action callback, CancellationToken cancellationToken = new CancellationToken(), TimeSpan timeout = new TimeSpan()) {
      Contract.Requires<ArgumentNullException>(callback != null);

      throw new NotImplementedException();
    }

    public void Invoke(Action callback, CancellationToken cancellationToken = new CancellationToken(), TimeSpan timeout = new TimeSpan()) {
      Contract.Requires<ArgumentNullException>(callback != null);

      throw new NotImplementedException();
    }
  }
}
