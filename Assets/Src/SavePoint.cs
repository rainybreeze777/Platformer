using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavePoint
{
  public int sceneIndex;
  public List<InvtItem> inventoryItems;

  public SavePoint(int sceneIndex) {
    this.sceneIndex = sceneIndex;
    inventoryItems = new List<InvtItem>();
  }

  public void InitInvtItems() {
    foreach (var item in inventoryItems) {
      item.AsyncInitSprite();
    }
  }

  public List<InvtItem> AllItems
  {
    get {
      var newInvtItems = new List<InvtItem>();
      foreach (var item in inventoryItems) {
        newInvtItems.Add(item.ShallowCopy());
      }
      return newInvtItems;
    }
  }
}
