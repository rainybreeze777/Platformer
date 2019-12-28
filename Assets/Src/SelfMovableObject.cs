using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelfMovableObject : Triggerable
{
  public bool m_OneTimeTrigger = true;
  public bool m_Loop = false;
  public EDirection m_InitialDirection = EDirection.Positive;
  public Vector3 m_Pos1;
  public Vector3 m_Pos2;
  [Range(0, 1)]
  public float m_StartingPos;
  public float m_Speed;

  private bool m_ShouldMove = false;
  private bool m_AlreadyTriggered = false;
  private float m_Dist;
  private int m_CurrentDirection;
  private float m_T;

  // Start is called before the first frame update
  void Start()
  {
    m_Dist = Vector3.Distance(m_Pos1, m_Pos2);
    m_CurrentDirection = m_InitialDirection == EDirection.Positive ? 1 : -1;
    m_T = m_StartingPos;
    transform.position = Vector3.Lerp(m_Pos1, m_Pos2, m_T);
  }

  // Update is called once per frame
  void Update()
  {
    if (!Application.IsPlaying(gameObject)) {
      transform.position = Vector3.Lerp(m_Pos1, m_Pos2, m_StartingPos);
      return;
    }
    if (m_ShouldMove) {
      m_T += (Time.deltaTime * m_Speed / m_Dist) * m_CurrentDirection;
      if (m_Loop)
      {
        if (m_T < 0)
        {
          m_CurrentDirection = 1;
          m_T = 0 - m_T;
        }
        else if (m_T > 1)
        {
          m_CurrentDirection = -1;
          m_T = 2 - m_T;
        }
      }
      else
      {
        m_T = Mathf.Clamp(m_T, 0, 1);
      }
    }
    transform.position = Vector3.Lerp(m_Pos1, m_Pos2, m_T);
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

  public enum EDirection { Positive, Negative };
}
