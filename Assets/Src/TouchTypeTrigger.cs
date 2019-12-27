using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTypeTrigger : MonoBehaviour
{
  private Animator m_Anim;
  // Start is called before the first frame update
  void Start()
  {
    m_Anim = GetComponent<Animator>();
  }

  void OnTriggerEnter2D(Collider2D collider) {
    m_Anim.SetBool("isTriggered", true);
  }

  void OnTriggerExit2D(Collider2D collider) {
    m_Anim.SetBool("isTriggered", false);
  }
}
