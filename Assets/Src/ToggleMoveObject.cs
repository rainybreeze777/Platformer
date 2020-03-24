using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleMoveObject : Toggleable, ISceneMovable {

  public float m_Speed = 1f;
  public Vector2 m_LocalPos1;
  public Vector2 m_LocalPos2;

  private Rigidbody2D m_Rb2d;
  private Vector2 m_Pos1;
  private Vector2 m_Pos2;
  private bool m_IsOn = false;
  private float m_Dist;
  private float m_T;
  private int m_Direction;

  void Start() {
    m_Rb2d = GetComponent<Rigidbody2D>();
    // Convert position in parent space to world position
    m_Pos1 = transform.parent.TransformPoint(m_LocalPos1);
    m_Pos2 = transform.parent.TransformPoint(m_LocalPos2);
    m_Rb2d.position = m_Pos1;
    m_Dist = Vector2.Distance(m_Pos1, m_Pos2);
    m_T = 0;
    m_Direction = 0;
  }

  void Update() {
    if (m_IsOn && m_T != 1) {
      m_T = Mathf.Min(1, m_T + Time.deltaTime * m_Speed / m_Dist);
      m_Direction = 1;
    } else if (!m_IsOn && m_T != 0) {
      m_T = Mathf.Max(0, m_T - Time.deltaTime * m_Speed / m_Dist);
      m_Direction = -1;
    } else {
      m_Direction = 0;
    }
    m_Rb2d.position = Vector2.Lerp(m_Pos1, m_Pos2, m_T);
  }

  public override void NotifyToggleOn() {
    m_IsOn = true;
  }

  public override void NotifyToggleOff() {
    m_IsOn = false;
  }

  public Vector2 Velocity {
    get { 
      return (m_Pos2 - m_Pos1).normalized * m_Speed * m_Direction;
    }
  }

}
