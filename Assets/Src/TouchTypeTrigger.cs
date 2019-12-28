using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTypeTrigger : MonoBehaviour
{
  public List<Triggerable> m_TriggerTargets;

  private Animator m_Anim;

  void Start()
  {
    m_Anim = GetComponent<Animator>();
  }

  void OnTriggerEnter2D(Collider2D collider) {
    m_Anim.SetBool("isTriggered", true);
    foreach (var t in m_TriggerTargets) {
      t.NotifyTriggerEnter();
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    m_Anim.SetBool("isTriggered", false);
    foreach (var t in m_TriggerTargets) {
      t.NotifyTriggerExit();
    }
  }
}
