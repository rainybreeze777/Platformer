using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIInventoryListener : MonoBehaviour {
  
  public EventManager m_EventManager;

  private InvtItem m_CacheItem;

  void Start() {
    m_EventManager.AddListener<ObtainItemUEvent, InvtItem>(OnObtainItem);
  }

  public void OnObtainItem(InvtItem item) {
    m_CacheItem = item;
    Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/UI/UIImage.prefab").Completed += (asyncRes) => {
      GameObject imageObj = Instantiate(asyncRes.Result, new Vector3(0, 0, 0), Quaternion.identity);
      Image image = imageObj.GetComponent<Image>();
      image.preserveAspect = true;
      image.sprite = m_CacheItem.Sprite;
      imageObj.transform.SetParent(gameObject.transform);
      m_CacheItem = null;
    };
  }
}
