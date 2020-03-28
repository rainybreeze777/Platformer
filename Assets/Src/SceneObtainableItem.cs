using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneObtainableItem : MonoBehaviour
{
  private PlayerManager m_PlayerManager;

  public string m_ItemName;
  public string m_ItemId;
  public Sprite m_ItemSprite;
  public string m_ItemSpriteAssetPath;
  public SceneObtainableItem m_SceneObtainablePrefab;
  public string m_PrefabAssetPath;

  void Start()
  {
    m_PlayerManager = GameObject.Find("/PlayerManager").GetComponent<PlayerManager>();
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.tag == "Player")
    {
      m_PlayerManager.ObtainItem(
        new InvtItem(m_ItemName
                     , m_ItemId
                     , m_ItemSprite
                     , m_ItemSpriteAssetPath
                     , mPrefabAssetPath));
      Destroy(gameObject);
    }
  }

  public string ItemId { get { return m_ItemId; } }
}
