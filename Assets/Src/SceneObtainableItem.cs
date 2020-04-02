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

  private BoxCollider2D m_Collider;

  void Awake() {
    m_Collider = GetComponent<BoxCollider2D>();
  }

  void Start() {
    m_PlayerManager = GameObject.Find("/PlayerManager").GetComponent<PlayerManager>();
  }

  public void ObtainThis() {
    m_PlayerManager.ObtainItem(new InvtItem(m_ItemName
                                            , m_ItemId
                                            , m_ItemSprite
                                            , m_ItemSpriteAssetPath
                                            , m_PrefabAssetPath));
    Destroy(gameObject);
  }

  public string ItemId { get { return m_ItemId; } }
  public Collider2D ItemCollider { get { return m_Collider; } }
}
