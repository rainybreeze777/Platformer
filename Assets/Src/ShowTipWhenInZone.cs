using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ShowTipWhenInZone : MonoBehaviour
{
  public UnityEvent ShowTipFuncion;
  public UnityEvent StopShowTipFunction;

  private bool m_IsShowingTip = false;

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_IsShowingTip = true;
      ShowTipFuncion?.Invoke();
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    if (m_IsShowingTip && collider.tag == "Player") {
      m_IsShowingTip = false;
      StopShowTipFunction?.Invoke();
    }
  }

  public void ForceStopShowTips() {
    if (m_IsShowingTip) {
      m_IsShowingTip = false;
      StopShowTipFunction?.Invoke();
    }
  }
}
