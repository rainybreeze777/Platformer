using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ScriptedMoveTrigger : MonoBehaviour
{
  public bool m_IsThroughHidden;

  private Vector3 m_TargetPoint;

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

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      PlayerPlatformerController player = 
        collider.gameObject.GetComponent<PlayerPlatformerController>() 
          as PlayerPlatformerController;
      if (m_IsThroughHidden) {
        player.FlipHiddenPassageLayer(true, -1);
      }
      player.ScriptedMoveToPoint(m_TargetPoint, () => {
        player.FlipHiddenPassageLayer(false);
      });
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
