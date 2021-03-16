using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Common.ObjectModel.Collections {
  public class KeyedCollection<KeyType, ItemType>: System.Collections.ObjectModel.KeyedCollection<KeyType,ItemType> {
    private readonly Boolean isKeyedType;

    #region Method: Constructor
    public KeyedCollection(
      IEnumerable<ItemType> items = null, IEqualityComparer<KeyType> comparer = null, Int32 dictionaryCreationThreshold = 0
    ): 
      base(comparer, dictionaryCreationThreshold
    ) {
      this.isKeyedType = typeof(IKeyedItem<KeyType>).IsAssignableFrom(typeof(ItemType));

      if (items != null) {
        this.AddRange(items);
      }
    }
    #endregion

    #region Methods: AddRange, ContainsKey, TryGetValue, GetKeys
    public void AddRange(IEnumerable<ItemType> items) {
      if (items == null) throw new ArgumentNullException();

      foreach (ItemType item in items) {
        this.Add(item);
      }
    }

    public Boolean ContainsKey(KeyType key) {
      if (key.Equals(default(KeyType))) throw new ArgumentException();

      return this.Contains(key);
    }

    public Boolean TryGetValue(KeyType key, out ItemType value) {
      if (key.Equals(default(KeyType))) throw new ArgumentException();

      if (this.Dictionary != null) {
        return this.Dictionary.TryGetValue(key, out value);
      }

      foreach (ItemType item in base.Items) {
        if (this.Comparer.Equals(this.GetKeyForItem(item), key)) {
          value = item;
          return true;
        }
      }

      value = default(ItemType);
      return false;
    }

    public ICollection<KeyType> GetKeys() {
      if (this.Dictionary != null) {
        return this.Dictionary.Keys;
      }

      System.Collections.Generic.List<KeyType> collection = new List<KeyType>(this.Count);
      foreach (ItemType item in base.Items) {
        collection.Add(this.GetKeyForItem(item));
      }

      return collection;
    }
    #endregion

    #region Methods: InsertItem, SetItem
    protected override void InsertItem(Int32 index, ItemType item) {
      if (this.ContainsKey(this.GetKeyForItem(item))) {
        throw new ArgumentException("The key is already taken.");
      }

      base.InsertItem(index, item);
    }

    protected override void SetItem(Int32 index, ItemType item) {
      KeyType newKey = this.GetKeyForItem(item);
      if (this.ContainsKey(newKey)) {
        KeyType replacedKey = this.GetKeyForItem(this[index]);
        if (!this.Comparer.Equals(replacedKey, newKey)) {
          throw new ArgumentException("The key is already taken.");
        }
      }

      base.SetItem(index, item);
    }
    #endregion

    #region Methods: GetKeyForItem
    /// <exception cref="InvalidOperationException">
    ///   The given ItemType parameter does not implement <see cref="IKeyedItemItem{T}" /> and the <see cref="GetKeyForItem" /> method 
    ///   is not overriden.
    /// </exception>
    protected override KeyType GetKeyForItem(ItemType item) {
      if (!this.isKeyedType) {
        throw new InvalidOperationException("The given ItemType parameter does not implement IKeyedItem<T> and the GetKeyForItem method is not overriden.");
      }

      return ((IKeyedItem<KeyType>)item).Key;
    }
    #endregion
  }
}