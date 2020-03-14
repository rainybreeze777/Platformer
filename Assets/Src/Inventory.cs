using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
  private List<InvtItem> m_Items;

  public Inventory() {
    m_Items = new List<InvtItem>();
  }

  public Inventory(List<InvtItem> items) {
    m_Items = items;
  }

  public bool PutInItem(InvtItem item) {
    m_Items.Add(item);
    return true;
  }

  public bool TakeOutItem(InvtItem item) {
    return m_Items.Remove(item);
  }

  public bool HasItemById(string id) {
    foreach (InvtItem item in m_Items) {
      if (item.Id == id) { return true; }
    }
    return false;
  }

  public InvtItem TakeOutItemById(string id) {
    for (int i = 0; i < m_Items.Count; ++i) {
      if (m_Items[i].Id == id) {
        InvtItem ret = m_Items[i];
        m_Items.RemoveAt(i);
        return ret;
      }
    }
    return null;
  }

  public InvtItem GetItemById(string id) {
    foreach (var item in m_Items) {
      if (item.Id == id) { return item; }
    }
    return null;
  }

  public List<InvtItem> AllItems { get { return m_Items; } }

  /// <summary>
  ///   Creates a new list of shallow cloned InvtItems.
  ///   Is this acceptable? Should the InvtItem be deep cloned as well?
  /// </summary>
  public Inventory Clone() {
    var ret = new Inventory();
    foreach(var item in m_Items) {
      ret.PutInItem(item.ShallowCopy());
    }
    return ret;
  }
}
