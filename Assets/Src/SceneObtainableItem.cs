using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObtainableItem : MonoBehaviour
{
  public InvtItem m_Item;

  private PlayerManager m_PlayerManager;

  void Start()
  {
    m_PlayerManager = GameObject.Find("/PlayerManager").GetComponent<PlayerManager>();
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.tag == "Player")
    {
      m_PlayerManager.ObtainItem(m_Item);
      Destroy(gameObject);
    }
  }
}
