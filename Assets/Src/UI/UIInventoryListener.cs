using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIInventoryListener : MonoBehaviour {

  public AssetReference m_UIImageAsset;

  private PlayerManager m_PlayerManager;
  private EventManager m_EventManager;
  // Dictionary to store quick UI object look up
  // Key: ItemId
  // Value: UIImage
  private Dictionary<string, UIImage> m_ItemsDict;

  private UIImage m_UIImage;

  void Awake() {
    m_ItemsDict = new Dictionary<string, UIImage>();
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

    m_UIImageAsset.LoadAssetAsync<GameObject>().Completed += (asyncRes) => {
      m_UIImage = asyncRes.Result.GetComponent<UIImage>();
    };
  }

  public void OnObtainItem(InvtItem item) {
    UIImage imageObj = Instantiate(m_UIImage
                               , new Vector3(0, 0, 0)
                               , Quaternion.identity)
                      .GetComponent<UIImage>() as UIImage;
    imageObj.Init(m_PlayerManager, item.Id, item.Sprite);
    imageObj.transform.SetParent(gameObject.transform);
    m_ItemsDict[item.Id] = imageObj;
  }

  public void OnSpendingItem(InvtItem item) {
    m_ItemsDict[item.Id].transform.SetParent(null);
    UIImage uiImage = m_ItemsDict[item.Id];
    m_ItemsDict.Remove(item.Id);
    Destroy(uiImage.gameObject);
  }
}