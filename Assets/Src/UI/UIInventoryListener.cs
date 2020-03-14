using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIInventoryListener : MonoBehaviour {

  private PlayerManager m_PlayerManager;
  private EventManager m_EventManager;
  // For Callbacks, watch out for thread synchronization
  // TODO: Get a lock maybe? Or a queue of some kind, to avoid
  // overwrites
  private InvtItem m_CacheItem;
  // Dictionary to store quick UI object look up
  // Key: ItemId
  // Value: UI GameObject
  private Dictionary<string, GameObject> m_ItemsDict;

  void Awake() {
    m_ItemsDict = new Dictionary<string, GameObject>();
  }

  void Start() {
    m_EventManager = GameObject.Find("/EventManager")
                               .GetComponent<EventManager>() as EventManager;
    m_PlayerManager = GameObject.Find("/PlayerManager")
                                .GetComponent<PlayerManager>() as PlayerManager;
    m_EventManager.AddListener<ObtainItemUEvent, InvtItem>(OnObtainItem);
    m_EventManager.AddListener<SpendItemUEvent, InvtItem>(OnSpendingItem);
    foreach (var item in m_PlayerManager.AllItems) {
      OnObtainItem(item);
    }
  }

  public void OnObtainItem(InvtItem item) {
    m_CacheItem = item;
    Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/UI/UIImage.prefab").Completed += (asyncRes) => {
      GameObject imageObj = Instantiate(asyncRes.Result, new Vector3(0, 0, 0), Quaternion.identity);
      Image image = imageObj.GetComponent<Image>();
      image.preserveAspect = true;
      image.sprite = m_CacheItem.Sprite;
      imageObj.transform.SetParent(gameObject.transform);
      m_ItemsDict[item.Id] = imageObj;
      m_CacheItem = null;
    };
  }

  public void OnSpendingItem(InvtItem item) {
    m_ItemsDict[item.Id].transform.SetParent(null);
    Destroy(m_ItemsDict[item.Id]);
    m_ItemsDict.Remove(item.Id);
  }
}