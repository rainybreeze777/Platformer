using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = Platformer.Input;

[RequireComponent(typeof(BoxCollider2D))]
public class UnlockableSwitch : MonoBehaviour, IUnlockable {
  
  public List<SceneObtainableItem> m_UnlockItems;
  public bool m_AutoReset = false;
  public float m_AutoResetWaitTime = 0;
  public List<Toggleable> m_ToggleTargets;
  public string m_LockedDialogueLineTag = "MissingCombineItem";
  public ShowTipWhenInZone m_ShowTip;
  private PlayerManager m_PlayerManager;
  private TextManager m_TextManager;
  private UIManager m_UIManager;
  private bool m_IsLocked = true;
  private bool m_IsOn = false;
  private bool m_IsPlayerInRange = false;
  private bool m_IsEngagedInConvo = false;
  private bool m_GetNextLine = false;
  private SwitchAnimDriver m_AnimDriver;
  
  private List<string> m_UnlockNeededItemIds;

  // Start is called before the first frame update
  void Start() {
    m_PlayerManager = GameObject.Find("/PlayerManager")
                           .GetComponent<PlayerManager>() as PlayerManager;
    m_TextManager = GameObject.Find("/TextManager").GetComponent<TextManager>();
    m_UIManager = GameObject.Find("/UICanvas").GetComponent<UIManager>();
    m_AnimDriver = GetComponent<SwitchAnimDriver>();
    m_UnlockNeededItemIds = new List<string>();
    foreach (var item in m_UnlockItems) {
      m_UnlockNeededItemIds.Add(item.ItemId);
    }
  }

  // Update is called once per frame
  void Update() {
    if (m_IsEngagedInConvo && Input.GetActionButtonDownIgnoreAllowInput()) {
      m_GetNextLine = true;
    }
    if (m_IsPlayerInRange && Input.GetButtonDown("Action")) {
      if (!m_IsLocked) {
        m_ShowTip?.ForceStopShowTips();
        if (!m_IsOn) {
          m_IsOn = true;
          TurnOn();
        } else if (!m_AutoReset) {
          m_IsOn = false;
          TurnOff();
        }
      } else if (!m_IsEngagedInConvo 
                 && !string.IsNullOrEmpty(m_LockedDialogueLineTag)) {
        StartCoroutine(DisplayLockedDialogue());
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_IsPlayerInRange = true;
    } else {
      SceneObtainableItem item = 
        collider.gameObject.GetComponent<SceneObtainableItem>()
          as SceneObtainableItem;
      if (item != null && m_IsLocked) {

        if (m_UnlockNeededItemIds.Contains(item.ItemId)) {
          m_UnlockNeededItemIds.Remove(item.ItemId);
          Destroy(item.gameObject);
        }
        if (m_UnlockNeededItemIds.Count == 0) {
          Unlock();
        }
      }
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_IsPlayerInRange = false;
    }
  }

  public void Unlock() {
    m_IsLocked = false;
    m_AnimDriver.PlayUnlockSwitch();
  }

  private void TurnOn() {
    m_AnimDriver.PlayTurnOnSwitch(() => {
      foreach (var target in m_ToggleTargets) {
        target.NotifyToggleOn();
      }
    });
  }

  private void TurnOff() {
    m_AnimDriver.PlayTurnOffSwitch(() => {
      foreach (var target in m_ToggleTargets) {
        target.NotifyToggleOff();
      }
    });
  }

  private IEnumerator DisplayLockedDialogue() {
    m_IsEngagedInConvo = true;
    Input.AllowInput = false;
    m_UIManager.ShowDialogueUI();
    int linesCount = m_TextManager
                      .GetNumberOfTriggerLines(m_LockedDialogueLineTag);
    for (int lineIndex = 0; lineIndex < linesCount; ++lineIndex)
    {
      ActorLine line =
        m_TextManager.GetTriggerLine(m_LockedDialogueLineTag, lineIndex);
      m_UIManager.PopulateDialogueUI(line);
      yield return new WaitUntil(() => m_GetNextLine);
      m_GetNextLine = false;
    }
    m_UIManager.HideDialogueUI();
    Input.AllowInput = true;
    m_IsEngagedInConvo = false;
  }
}
