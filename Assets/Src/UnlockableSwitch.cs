using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class UnlockableSwitch : MonoBehaviour, IUnlockable {
  
  public List<SceneObtainableItem> m_UnlockItems;
  public bool m_AutoReset = false;
  public float m_AutoResetWaitTime = 0;
  public List<Toggleable> m_ToggleTargets;
  private PlayerManager m_PlayerManager;
  private bool m_IsLocked = true;
  private bool m_IsOn = false;
  private bool m_IsPlayerInRange = false;
  private Animator m_Animator;
  
  private List<string> m_UnlockNeededItemIds;

  // Start is called before the first frame update
  void Start() {
    m_PlayerManager = GameObject.Find("/PlayerManager")
                           .GetComponent<PlayerManager>() as PlayerManager;
    m_Animator = GetComponent<Animator>();
    m_UnlockNeededItemIds = new List<string>();
    foreach (var item in m_UnlockItems) {
      m_UnlockNeededItemIds.Add(item.ItemId);
    }
  }

  // Update is called once per frame
  void Update() {
    if (!m_IsLocked) {
      if (m_IsPlayerInRange && Input.GetButtonDown("Action")) {
        if (!m_IsOn) {
          m_IsOn = true;
          PullOn();
        } else if (!m_AutoReset) {
          m_IsOn = false;
          PullOff();
        }
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_IsPlayerInRange = true;
      if (m_IsLocked) {
        var itemsToUse = new List<string>();
        foreach (string itemId in m_UnlockNeededItemIds) {
          if (m_PlayerManager.HasItemById(itemId))
          {
            itemsToUse.Add(itemId);
          }
        }
        foreach (string itemId in itemsToUse) {
          m_PlayerManager.SpendItemById(itemId);
          m_UnlockNeededItemIds.Remove(itemId);
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
    if (m_Animator != null) {
      m_Animator.SetBool("isUnlocked", true);
    }
  }

  private void PullOn() {
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOn();
    }
    if (m_Animator != null) {
      m_Animator.SetBool("pulled", true);
    }
  }

  private void PullOff() {
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOff();
    }
    if (m_Animator != null) {
      m_Animator.SetBool("pulled", false);
    }
  }
}
