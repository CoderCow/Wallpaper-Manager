using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Common.ObjectModel.Collections {
  public class ReadOnlyKeyedCollection<KeyType,ItemType>: System.Collections.ObjectModel.ReadOnlyCollection<ItemType> {
    #region Property: KeyedCollection
    protected KeyedCollection<KeyType,ItemType> KeyedCollection {
      get { return (KeyedCollection<KeyType,ItemType>)this.Items; }
    }
    #endregion


    #region Method: Constructor
    public ReadOnlyKeyedCollection(KeyedCollection<KeyType,ItemType> keyedCollection): base(keyedCollection) {
      if (keyedCollection == null) throw new ArgumentNullException();
    }
    #endregion

    #region Methods: ContainsKey, TryGetValue, GetKeys
    public Boolean ContainsKey(KeyType key) {
      if (key == null) throw new ArgumentNullException();

      return this.KeyedCollection.Contains(key);
    }

    public Boolean TryGetValue(KeyType key, out ItemType value) {
      if (key == null) throw new ArgumentNullException();

      return this.KeyedCollection.TryGetValue(key, out value);
    }
    
    public ICollection<KeyType> GetKeys() {
      return this.KeyedCollection.GetKeys();
    }
    #endregion
  }
}