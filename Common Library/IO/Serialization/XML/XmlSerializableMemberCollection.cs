using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Common.ObjectModel.Collections;

namespace Common.IO.Serialization {
  internal class XmlSerializableMemberCollection: KeyedCollection<String,XmlSerializableMember> {
    protected override String GetKeyForItem(XmlSerializableMember item) {
      return item.MemberInfo.Name;
    }

    public IList<XmlMemberInfo> ToMemberInfos() {
      Contract.Ensures(Contract.Result<IList<XmlMemberInfo>>() != null);

      IList<XmlMemberInfo> memberInfos = new List<XmlMemberInfo>(this.Items.Count);
      foreach (XmlSerializableMember item in this.Items) {
        memberInfos.Add(item.MemberInfo);
      }

      return memberInfos;
    }

    public void Sort() {
      // Insertion sort algorithm from english wikipedia.
      for (Int32 i = 1; i < this.Items.Count; i++) {
        XmlSerializableMember value = this.Items[i];
        Int32 x = i - 1;
        Boolean done = false;

        do {
          if (this.Items[x].CompareTo(value) > 0) {
            this.Items[x + 1] = this.Items[x];

            if (--x < 0)
              done = true;
          } else {
            done = true;
          }
        } while (!done);

        this.Items[x + 1] = value;
      }
    }
  }
}