using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D))]
public class ToggleMoveObject : Toggleable, ISceneMovable {

  public float m_Speed = 1f;
  public Vector2 m_LocalPos1;
  public Vector2 m_LocalPos2;

  private Rigidbody2D m_Rb2d;
  private Vector2 m_Pos1;
  private Vector2 m_Pos2;
  private Vector2 m_Velocity;
  private bool m_IsOn = false;

  void Start() {
    m_Rb2d = GetComponent<Rigidbody2D>();
    // Convert position in parent space to world position
    m_Pos1 = transform.parent.TransformPoint(m_LocalPos1);
    m_Pos2 = transform.parent.TransformPoint(m_LocalPos2);
    m_Rb2d.position = m_Pos1;
    m_Velocity = Vector2.zero;
  }

  void FixedUpdate() {
    Vector2 targetPos = m_IsOn ? m_Pos2 : m_Pos1;
    Vector2 movingTowards = m_IsOn ? m_Pos2 - m_Pos1 : m_Pos1 - m_Pos2;
    m_Velocity = m_Rb2d.position == targetPos ? Vector2.zero : movingTowards.normalized * m_Speed;
    m_Rb2d.MovePosition(Vector2.MoveTowards(m_Rb2d.position, targetPos, Time.fixedDeltaTime * m_Speed));
  }

  public override void NotifyToggleOn() {
    m_IsOn = true;
  }

  public override void NotifyToggleOff() {
    m_IsOn = false;
  }

  public Vector2 Velocity {
    get { 
      return m_Velocity;
    }
  }

}
