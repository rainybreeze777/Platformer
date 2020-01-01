using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
  private List<InvtItem> m_Items;

  public Inventory() {
    m_Items = new List<InvtItem>();
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

  public bool TakeOutItemById(string id) {

    for (int i = 0; i < m_Items.Count; ++i) {
      if (m_Items[i].Id == id) {
        m_Items.RemoveAt(i);
        return true;
      }
    }

    return false;
  }
}
