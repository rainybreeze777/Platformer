using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebugShowPoint : MonoBehaviour
{
#if UNITY_EDITOR
  public Color m_Color = Color.white;
  public float m_Radius = 1.0f;

  void OnDrawGizmos()
  {
    if (!Application.isPlaying) {
      Gizmos.color = m_Color;
      Gizmos.DrawSphere(transform.position, m_Radius);
    }
  }
#endif
}
