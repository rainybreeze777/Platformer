using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

  private Inventory m_Inventory;

  void Awake()
  {
    m_Inventory = new Inventory();
  }

  public void ObtainItem(InvtItem item)
  {
    m_Inventory.PutInItem(item);
  }

  public void SpendItem(InvtItem item)
  {
    m_Inventory.TakeOutItem(item);
  }
}
