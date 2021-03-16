using System;

namespace Common.ObjectModel.Collections {
  public interface IKeyedItem<T> {
    T Key { get;  }
  }
}