using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = Platformer.Input;

[RequireComponent(typeof(BoxCollider2D))]
public class ScriptedMoveTrigger : MonoBehaviour
{
  public bool m_IsThroughHidden;
  public int m_HiddenSortingLayer = -1;
  public float m_PushSecondsToTrigger = 0.5f;
  public Collider2D m_StoppingCollider;

  private Vector3 m_TargetPoint;
  private bool m_IsTouching = false;

  private PlayerPlatformerController m_Player;
  private float m_PushedSeconds = 0;

  void Start() {
    bool noTargetPoint = true;
    foreach (Transform childTransform in transform) {
      if (childTransform.gameObject.name == "TargetPoint") {
        noTargetPoint = false;
        m_TargetPoint = childTransform.position;
        break;
      }
    }

    if (noTargetPoint) {
      throw new Exception("No TargetPoint as child object!");
    }
  }

  void Update() {
    if (m_IsTouching) {
      if (Input.GetAxis("Horizontal") != 0) {
        m_PushedSeconds += Time.deltaTime;
      } else {
        m_PushedSeconds = 0;
      }
      if (m_PushedSeconds >= m_PushSecondsToTrigger) {
        m_PushedSeconds = 0;
        DoScriptedMove(m_Player);
      }
    }
  }

  private void DoScriptedMove(PlayerPlatformerController player) {
    m_StoppingCollider.enabled = false;
    if (m_IsThroughHidden) {
      player.FlipHiddenPassageLayer(true, m_HiddenSortingLayer);
    }
    player.ScriptedMoveToPoint(m_TargetPoint, () => {
      player.FlipHiddenPassageLayer(false);
    });
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_Player = collider.gameObject.GetComponent<PlayerPlatformerController>() 
                    as PlayerPlatformerController;
      m_IsTouching = true;
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_IsTouching = false;
      m_Player = null;
      m_PushedSeconds = 0;
      m_StoppingCollider.enabled = true;
    }
  }

  void Reset() {
#if UNITY_EDITOR
    bool needToAddTargetPoint = true;

    foreach (Transform childTransform in transform) {
      if (childTransform.gameObject.name == "TargetPoint") {
        needToAddTargetPoint = false;
      }
    }

    if (needToAddTargetPoint) {
      GameObject targetPoint = new GameObject("TargetPoint");
      targetPoint.transform.SetParent(transform);
      targetPoint.AddComponent<DebugShowPoint>();
    }
#endif
  }
}
