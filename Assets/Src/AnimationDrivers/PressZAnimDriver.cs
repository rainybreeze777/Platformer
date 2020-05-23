using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(ShowTipWhenInZone))]
public class PressZAnimDriver : AnimDriver
{
  private ShowTipWhenInZone m_ShowTip;

  private const string kLoop = "Loop";
  private const string kStatic = "Static";
  private const string kDisappear = "Disappear";

  private DragonBones.AnimationState m_LoopState;

  // Start is called before the first frame update
  public override void Start()
  {
    base.Start();
    m_ShowTip = GetComponent<ShowTipWhenInZone>();
    m_Anim.GotoAndPlayByProgress(kDisappear, 100.0f, 1);
  }

  public void PlayLoop() {
    if (m_LoopState == null) {
      m_LoopState = m_Anim.FadeIn(kLoop);
    }
  }

  public void StopLoop() {
    if (m_LoopState != null) {
      // Continue current animation until end.
      m_Anim.GotoAndPlayByTime(kLoop, m_LoopState.currentTime, 1);
    }
  }

  protected override void OnAnimationComplete(string type, EventObject evObj) {
    DragonBones.AnimationState animState = evObj.animationState;
    if (animState.name.Equals(kLoop)) {
      m_LoopState = null;
      m_Anim.FadeIn(kDisappear, m_FadeInTime, 1);
    }
  }
}
