using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableLever : MonoBehaviour, IUnlockable {
  
  public string m_UnlockItemId;
  public bool m_AutoReset = false;
  public float m_AutoResetWaitTime = 0;
  public List<Toggleable> m_ToggleTargets;
  public PlayerManager m_PlayerManager;
  private bool m_IsLocked = true;
  private Animator m_Animator;
  
  // Start is called before the first frame update
  void Start() {
    m_Animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update() {
    if (!m_IsLocked) {
      if (Input.GetButtonDown("Action")) {
        PullOn();
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player"
        && m_PlayerManager.HasItemById(m_UnlockItemId)) {
      m_PlayerManager.SpendItemById(m_UnlockItemId);
      Unlock();
    }
  }

  public void Unlock() {
    m_IsLocked = false;
    m_Animator.SetBool("isUnlocked", true);
  }

  private void PullOn() {
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOn();
    }
    m_Animator.SetBool("pulled", true);
  }

  private void PullOff() {
    foreach (var target in m_ToggleTargets) {
      target.NotifyToggleOff();
    }
    m_Animator.SetBool("pulled", false);
  }
}
