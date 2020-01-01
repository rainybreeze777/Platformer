using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

  private Inventory m_Inventory;

  public EventManager m_EventManager;

  void Awake()
  {
    m_Inventory = new Inventory();
  }

  public void ObtainItem(InvtItem item)
  {
    m_Inventory.PutInItem(item);
    m_EventManager.Invoke<ObtainItemUEvent, InvtItem>(item);
  }

  public void SpendItem(InvtItem item)
  {
    m_Inventory.TakeOutItem(item);
    m_EventManager.Invoke<SpendItemUEvent, InvtItem>(item);
  }
}
