using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Common.Validation;

using Microsoft.Win32;

namespace Common.Windows {
  public partial class AutostartEntryCollection: IDictionary<String, String>, IDisposable {
    #region Constants and Fields
    private RegistryKey autostartKey;
    private Boolean isReadonly;
    private Int32 version;
    private Boolean disposed;
    #endregion

    #region Events and Properties
    public String this[String entryName] {
      get { return this.GetEntryValue(entryName); }
      set {
        this.AutostartKey.SetValue(entryName, value);
        this.version++;
      }
    }

    public String[] EntryNames {
      get { return this.AutostartKey.GetValueNames(); }
    }

    public String[] Commands {
      get {
        String[] entryNames = this.EntryNames;
        String[] entryValues = new String[entryNames.Length];

        for (Int32 i = 0; i < entryNames.Length; i++) {
          entryValues[i] = this[entryNames[i]];
        }

        return entryValues;
      }
    }

    ICollection<String> IDictionary<String, String>.Keys {
      get { return this.EntryNames; }
    }

    ICollection<String> IDictionary<String, String>.Values {
      get { return this.Commands; }
    }

    public Int32 Count {
      get { return this.AutostartKey.ValueCount; }
    }

    public Boolean IsReadOnly {
      get { return this.isReadonly; }
    }

    protected RegistryKey AutostartKey {
      get { return this.autostartKey; }
    }
    #endregion

    #region Methods
    internal AutostartEntryCollection(RegistryKey autostartKey, Boolean readOnlyAccess) {
      if (autostartKey == null)
        throw new ArgumentNullException("autostartKey");

      this.autostartKey = autostartKey;
      this.isReadonly = readOnlyAccess;
    }

    public virtual void Add(String entryName, String command) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");
      if (this.ContainsKey(entryName))
        throw new ArgumentException("entryName");

      this.SetOrAdd(entryName, command);
      this.version++;
    }

    public virtual void SetOrAdd(String entryName, String command) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");

      this.AutostartKey.SetValue(entryName, command);
    }

    public virtual void SetOrAdd(KeyValuePair<String, String> pair) {
      if (pair.Key == null)
        throw new ArgumentNullException("pair.Key");
      if (pair.Value == null)
        throw new ArgumentNullException("pair.Value");

      this.SetOrAdd(pair.Key, pair.Value);
    }

    public void Add(KeyValuePair<String, String> pair) {
      if (pair.Key == null)
        throw new ArgumentNullException("pair.Key");
      if (pair.Value == null)
        throw new ArgumentNullException("pair.Value");

      this.Add(pair.Key, pair.Value);
    }

    public Boolean Contains(KeyValuePair<String, String> pair) {
      if (pair.Key == null)
        throw new ArgumentNullException("pair.Key");
      if (pair.Value == null)
        throw new ArgumentNullException("pair.Value");
      
      return this.ContainsKey(pair.Key);
    }

    public Boolean ContainsKey(String entryName) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");

      try {
        this.GetEntryValue(entryName);

        return true;
      } catch (IOException) {
        return false;
      }
    }

    public virtual Boolean Remove(String entryName) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");

      this.AutostartKey.DeleteValue(entryName, true);
      this.version++;

      return true;
    }

    public virtual Boolean Remove(KeyValuePair<String, String> pair) {
      if (pair.Key == null)
        throw new ArgumentNullException("pair.Key");
      if (pair.Value == null)
        throw new ArgumentNullException("pair.Value");

      return this.Remove(pair.Key);
    }

    public virtual void Clear() {
      throw new NotImplementedException();

      // this.version++;
    }

    public Boolean TryGetValue(String entryName, out String command) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");

      try {
        command = this.GetEntryValue(entryName);

        return true;
      } catch (IOException) {
        command = null;

        return false;
      }
    }

    private String GetEntryValue(String entryName) {
      if (entryName == null)
        throw new ArgumentNullException("entryName");

      Object value = this.AutostartKey.GetValue(entryName, null);

      if (value is String)
        return (String)value;
      else
        throw new IOException();
    }

    public void CopyTo(KeyValuePair<String, String>[] pair, Int32 startIndex) {
      throw new NotImplementedException();
    }

    public AutostartEntryCollection.Enumerator GetEnumerator() {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    IEnumerator<KeyValuePair<String, String>> IEnumerable<KeyValuePair<String, String>>.GetEnumerator() {
      return this.GetEnumerator();
    }

    public void Dispose(Boolean disposing) {
      if (!this.disposed) {
        if (disposing) {
          if (this.autostartKey != null) {
            this.autostartKey.Close();
          }
        }

        this.autostartKey = null;
      }
    }

    public void Dispose() {
      this.Dispose(true);
    }

    ~AutostartEntryCollection() {
      this.Dispose(false);
    }
    #endregion
  }
}
