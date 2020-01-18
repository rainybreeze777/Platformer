using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelfMovableObject : MonoBehaviour, ITriggerable
{
  public bool m_ShouldMove = false;
  public bool m_OneTimeTrigger = true;
  public bool m_Loop = false;
  public Vector3 m_Pos1;
  public Vector3 m_Pos2;
  [Range(0, 1)]
  public float m_T;
  public float m_Speed;

  private bool m_AlreadyTriggered = false;

  // Start is called before the first frame update
  void Start()
  {
    transform.position = Vector3.Lerp(m_Pos1, m_Pos2, m_T);
  }

  // Update is called once per frame
  void Update()
  {
    if(!Application.IsPlaying(gameObject)) {
      transform.position = Vector3.Lerp(m_Pos1, m_Pos2, m_T);
      return;
    }
  }

  public void NotifyTriggerEnter() {
    if (!(m_AlreadyTriggered && m_OneTimeTrigger)) {
      m_ShouldMove = !m_ShouldMove;
      m_AlreadyTriggered = true;
    }
  }

  public void NotifyTriggerExit() {
    if (m_AlreadyTriggered && !m_OneTimeTrigger) {
      m_ShouldMove = !m_ShouldMove;
    }
  }
}
