using UnityEngine;

[System.Serializable]
// Inventory Item, used InvtItem to avoid a weird
// naming conflict with C#
public class InvtItem
{
  [SerializeField] private string m_Name;
  [SerializeField] private string m_ItemId;
  [SerializeField] private Sprite m_InventorySprite;

  public InvtItem(string name, string id, Sprite renderer) {
    m_Name = name;
    m_ItemId = id;
    m_InventorySprite = renderer;
  }

  public InvtItem ShallowCopy() {
    return this.MemberwiseClone() as InvtItem;
  }

  public string Name { get { return m_Name; } }
  public string Id { get { return m_ItemId; } }
  public Sprite Sprite { get { return m_InventorySprite; } }
}
