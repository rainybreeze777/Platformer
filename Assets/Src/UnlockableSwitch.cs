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
  public ShowTipWhenInZone m_ShowTip;
  private PlayerManager m_PlayerManager;
  private bool m_IsLocked = true;
  private bool m_IsOn = false;
  private bool m_IsPlayerInRange = false;
  private SwitchAnimDriver m_AnimDriver;
  
  private List<string> m_UnlockNeededItemIds;

  // Start is called before the first frame update
  void Start() {
    m_PlayerManager = GameObject.Find("/PlayerManager")
                           .GetComponent<PlayerManager>() as PlayerManager;
    m_AnimDriver = GetComponent<SwitchAnimDriver>();
    m_UnlockNeededItemIds = new List<string>();
    foreach (var item in m_UnlockItems) {
      m_UnlockNeededItemIds.Add(item.ItemId);
    }
  }

  // Update is called once per frame
  void Update() {
    if (!m_IsLocked) {
      if (m_IsPlayerInRange && Input.GetButtonDown("Action")) {
        m_ShowTip?.ForceStopShowTips();
        if (!m_IsOn) {
          m_IsOn = true;
          TurnOn();
        } else if (!m_AutoReset) {
          m_IsOn = false;
          TurnOff();
        }
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
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOn();
    }
    m_AnimDriver.PlayTurnOnSwitch();
  }

  private void TurnOff() {
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOff();
    }
    m_AnimDriver.PlayTurnOffSwitch();
  }
}
