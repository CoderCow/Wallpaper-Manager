using System;
using System.Collections;
using System.Collections.Generic;

using Common.IO;

namespace Common.Windows {
  public partial class AutostartEntryCollection {
    public class Enumerator: IEnumerator<KeyValuePair<String, String>>, IDictionaryEnumerator, IEnumerator, IDisposable {
      #region Constants and Fields
      private AutostartEntryCollection collection;
      private Int32 expectedCollectionVersion;
      private Int32 currentIndex; 
      private KeyValuePair<String, String> current;
      #endregion
      
      #region Events and Properties
      public KeyValuePair<String, String> Current {
        get { return this.current; }
      }

      public DictionaryEntry Entry {
        get { return new DictionaryEntry(this.Current.Key, this.Current.Value); }
      }

      public String Key {
        get { return this.Current.Key; }
      }

      public String Value {
        get { return this.Current.Value; }
      }

      Object IEnumerator.Current {
        get { return this.Current; }
      }

      Object IDictionaryEnumerator.Key {
        get { return this.Current.Key; }
      }

      Object IDictionaryEnumerator.Value {
        get { return this.Current.Value; }
      }
      #endregion
      
      #region Methods
      internal Enumerator(AutostartEntryCollection collection) {
        this.collection = collection;
        this.expectedCollectionVersion = collection.version;
        this.Reset();
      }

      public Boolean MoveNext() {
       if (this.expectedCollectionVersion == this.collection.version)
         throw new InvalidOperationException();

        throw new NotImplementedException();
      }

      public void Reset() {
        if (this.expectedCollectionVersion == this.collection.version)
          throw new InvalidOperationException();

        this.currentIndex = 0;
      }

      public void Dispose() {
        
      }
      #endregion
    }
  }
}