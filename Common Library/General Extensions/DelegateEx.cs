using System;

namespace Common {
  public static class DelegateExtensions {
    /// <summary>
    ///   Invokes the delegates of this <see cref="EventHandler" /> in reversed order.
    /// </summary>
    /// <param name="handler">
    ///   The <see cref="EventHandler" /> which delegates should be invoked.
    /// </param>
    /// 
    public static void ReverseInvoke(this EventHandler handler, Object sender, EventArgs e) {
      Delegate[] invocationList = handler.GetInvocationList();

      for (Int32 i = invocationList.Length - 1; i >= 0; i--) {
        ((EventHandler)invocationList[i]).Invoke(sender, e);
      }
    }

    /// <summary>
    ///   Invokes the delegates of this <see cref="EventHandler{T}" /> in reversed order.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the <see cref="EventArgs" /> type used for the <see cref="EventHandler" />.
    /// </typeparam>
    /// <param name="handler">
    ///   The <see cref="EventHandler{T}" /> which delegates should be invoked.
    /// </param>
    /// 
    public static void ReverseInvoke<T>(this EventHandler<T> handler, Object sender, EventArgs e) where T: EventArgs {
      Delegate[] invocationList = handler.GetInvocationList();

      for (Int32 i = invocationList.Length - 1; i >= 0; i--) {
        ((EventHandler<T>)invocationList[i]).Invoke(sender, (T)e);
      }
    }
  }
}