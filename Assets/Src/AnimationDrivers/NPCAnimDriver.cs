using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(Collider2D))]
public class NPCAnimDriver : AnimDriver
{
  private const string kIdle = "Idle";

  // Start is called before the first frame update
  public override void Start()
  {
    base.Start();
    m_Anim.Play(kIdle);
  }

  void OnTriggerStay2D(Collider2D collider) {
    if (collider.tag == "Player") {
      bool flip = m_Arma.flipX 
                  ? transform.position.x - collider.transform.position.x < 0
                  : transform.position.x - collider.transform.position.x > 0;
      if (flip) {
        m_Arma.flipX = !m_Arma.flipX;
      }
    }
  }

  protected override void OnAnimationComplete(string type, EventObject evObj) {}
}
