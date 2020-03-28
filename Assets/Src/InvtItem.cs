using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
// Inventory Item, used InvtItem to avoid a weird
// naming conflict with C#
public class InvtItem
{
  [SerializeField] private string m_Name;
  [SerializeField] private string m_ItemId;
  [SerializeField] private string m_InventorySpriteAssetPath;
  [SerializeField] private string m_SceneObtainablePrefabAssetPath;
  private Sprite m_InventorySprite;

  public InvtItem(string name
                  , string id
                  , Sprite renderer
                  , string spriteAssetPath
                  , string sceneObtainableAssetPath)
  {
    m_Name = name;
    m_ItemId = id;
    m_InventorySprite = renderer;
    m_InventorySpriteAssetPath = spriteAssetPath;
    m_SceneObtainablePrefabAssetPath = sceneObtainableAssetPath;
  }

  public InvtItem ShallowCopy() {
    return this.MemberwiseClone() as InvtItem;
  }

  public void AsyncInitSprite() {
    Addressables.LoadAssetAsync<Sprite>(m_InventorySpriteAssetPath).Completed += (asyncRes) =>
    {
      m_InventorySprite = asyncRes.Result;
    };
  }

  public string Name { get { return m_Name; } }
  public string Id { get { return m_ItemId; } }
  public Sprite Sprite { get { return m_InventorySprite; } }
}
