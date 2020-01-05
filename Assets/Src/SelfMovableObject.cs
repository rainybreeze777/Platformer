using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelfMovableObject : Triggerable
{
  public bool m_ShouldMove = false;
  public bool m_OneTimeTrigger = true;
  public bool m_Loop = false;
  public EDirection m_InitialDirection = EDirection.Positive;
  public Vector2 m_LocalPos1;
  public Vector2 m_LocalPos2;
  [Range(0, 1)]
  public float m_StartingPos;
  public float m_Speed;

  private bool m_AlreadyTriggered = false;
  private float m_Dist;
  private int m_CurrentDirection;
  private float m_T;
  private Vector2 m_Pos1;
  private Vector2 m_Pos2;
  private Vector2 m_Velocity;
  private Rigidbody2D m_Rb2d;

  // Start is called before the first frame update
  void Start() {
    m_Rb2d = GetComponent<Rigidbody2D>();
    // Convert position in parent space to world position
    m_Pos1 = transform.parent.TransformPoint(m_LocalPos1);
    m_Pos2 = transform.parent.TransformPoint(m_LocalPos2);
    m_Dist = Vector2.Distance(m_Pos1, m_Pos2);
    m_CurrentDirection = m_InitialDirection == EDirection.Positive ? 1 : -1;
    m_T = m_StartingPos;
    m_Rb2d.position = Vector2.Lerp(m_Pos1, m_Pos2, m_T);
    Debug.Assert(tag == "SelfMovable", name + " object is self movable, but does not have tag SelfMovable!");
  }

  void FixedUpdate() {
    if (!Application.IsPlaying(gameObject)) {
      m_Rb2d.position = Vector2.Lerp(m_Pos1, m_Pos2, m_StartingPos);
      return;
    }
    if (m_ShouldMove) {
      float prevT = m_T;
      m_T += (Time.deltaTime * m_Speed / m_Dist) * m_CurrentDirection;
      if (m_Loop) {
        // Reposition out-of-bounds t and flip the direction
        if (m_T < 0 || m_T > 1) {
          m_T = m_CurrentDirection + 1 - m_T;
          m_CurrentDirection *= -1;
        }
      } else {
        m_T = Mathf.Clamp(m_T, 0, 1);
      }
      if (m_T != prevT) {
        m_Velocity = (m_Pos2 - m_Pos1).normalized * m_CurrentDirection * m_Speed;
      }
    }
    m_Rb2d.position = Vector3.Lerp(m_Pos1, m_Pos2, m_T);
  }

  public override void NotifyTriggerEnter() {
    if (!(m_AlreadyTriggered && m_OneTimeTrigger)) {
      m_ShouldMove = !m_ShouldMove;
      m_AlreadyTriggered = true;
    }
  }

  public override void NotifyTriggerExit() {
    if (m_AlreadyTriggered && !m_OneTimeTrigger) {
      m_ShouldMove = !m_ShouldMove;
    }
  }

  public Vector2 Velocity { get { return m_Velocity; } }

  public enum EDirection { Positive, Negative };
}
