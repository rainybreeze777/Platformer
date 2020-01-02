using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

  private Inventory m_Inventory;

  public EventManager m_EventManager;

  void Awake() {
    m_Inventory = new Inventory();
  }

  public void ObtainItem(InvtItem item) {
    if (m_Inventory.PutInItem(item)) {
      m_EventManager.Invoke<ObtainItemUEvent, InvtItem>(item);
    }   
  }

  public void SpendItem(InvtItem item) {    
    if (m_Inventory.TakeOutItem(item)) {
      m_EventManager.Invoke<SpendItemUEvent, InvtItem>(item);
    }
  }

  public void SpendItemById(string id) {
    InvtItem item = m_Inventory.TakeOutItemById(id);
    if (item != null) {
      m_EventManager.Invoke<SpendItemUEvent, InvtItem>(item);
    }
  }

  public bool HasItemById(string id) {
    return m_Inventory.HasItemById(id);
  }
}
