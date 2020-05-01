using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Input = Platformer.Input;

public class CombineCollectedItems : MonoBehaviour
{
  public List<string> m_ListOfItemIdsToCombine;
  public AssetReference m_CombinedItemAsset;
  public string m_CombineTriggerTag = "CombineItems";

  private bool m_IsEngagedInConvo = false;
  private bool m_GetNextLine = false;

  private EventManager m_EventManager;
  private PlayerManager m_PlayerManager;
  private TextManager m_TextManager;
  private UIManager m_UIManager;

  private List<string> m_PossessedItemIds;

  private SceneObtainableItem m_CombinedItem;

  void Awake() {
    m_PossessedItemIds = new List<string>();
  }

  // Start is called before the first frame update
  void Start()
  {
    m_EventManager = GameObject.Find("/EventManager").GetComponent<EventManager>();
    m_PlayerManager = GameObject.Find("/PlayerManager").GetComponent<PlayerManager>();
    m_TextManager = GameObject.Find("/TextManager").GetComponent<TextManager>();
    m_UIManager = GameObject.Find("/UICanvas").GetComponent<UIManager>()
                    as UIManager;
    foreach(var item in m_PlayerManager.AllItems) {
      if (m_ListOfItemIdsToCombine.Contains(item.Id)) {
        m_PossessedItemIds.Add(item.Id);
      }
    }

    m_EventManager.AddListener<ObtainItemUEvent, InvtItem>(OnObtainItem);
    m_EventManager.AddListener<SpendItemUEvent, InvtItem>(OnSpendItem);

    m_CombinedItemAsset.LoadAssetAsync<GameObject>().Completed += (asyncRes) => {
      m_CombinedItem = asyncRes.Result.GetComponent<SceneObtainableItem>();
    };
  }

  void Update() {
    if (m_IsEngagedInConvo && Input.GetActionButtonDownIgnoreAllowInput()) {
      m_GetNextLine = true;
    }
    if (m_ListOfItemIdsToCombine.Count <= m_PossessedItemIds.Count) {
      bool shouldCombine = true;
      foreach (var itemId in m_ListOfItemIdsToCombine) {
        if (!m_PossessedItemIds.Contains(itemId)) {
          shouldCombine = false;
          break;
        }
      }
      if (shouldCombine) {
        m_EventManager.RemoveListener<SpendItemUEvent, InvtItem>(OnSpendItem);
        foreach (var itemId in m_PossessedItemIds) {
          m_PlayerManager.SpendItemById(itemId);
        }

        StartCoroutine(DisplayCombineMessage(() => {
          m_PlayerManager.ObtainItem(new InvtItem(
            m_CombinedItem.Name,
            m_CombinedItem.ItemId,
            m_CombinedItem.ItemSprite,
            m_CombinedItem.ItemSpriteAssetPath,
            m_CombinedItem.ItemPrefabAssetPath
          ));
        }));

        m_PossessedItemIds.Clear();
      }
    }
  }

  private IEnumerator DisplayCombineMessage(Action onMessageDone) {
    m_IsEngagedInConvo = true;
    Input.AllowInput = false;
    m_UIManager.ShowDialogueUI();
    int linesCount = m_TextManager.GetNumberOfTriggerLines(m_CombineTriggerTag);
    for (int lineIndex = 0; lineIndex < linesCount; ++lineIndex) {
      ActorLine line = 
        m_TextManager.GetTriggerLine(m_CombineTriggerTag, lineIndex);
      m_UIManager.PopulateDialogueUI(line);
      yield return new WaitUntil(() => m_GetNextLine);
      m_GetNextLine = false;
    }
    m_UIManager.HideDialogueUI();
    onMessageDone?.Invoke();
    Input.AllowInput = true;
    m_IsEngagedInConvo = false;
  }

  private void OnObtainItem(InvtItem item) {
    if (m_ListOfItemIdsToCombine.Contains(item.Id)) {
      m_PossessedItemIds.Add(item.Id);
    }
  }

  private void OnSpendItem(InvtItem item) {
    if (m_PossessedItemIds.Contains(item.Id)) {
      m_PossessedItemIds.Remove(item.Id);
    }
  }
}
