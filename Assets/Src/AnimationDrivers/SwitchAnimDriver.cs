using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(UnlockableSwitch))]
public class SwitchAnimDriver : AnimDriver
{
  private const string kSwitchInactive = "SwitchInactive";
  private const string kSwitchOff = "SwitchOff";
  private const string kSwitchOn = "SwitchOn";
  private const string kSwitchOnRepeat = "SwitchOnRepeat";

  private ESwitchAnimState m_State;

  private Action m_OnTurnOnAnimFinish;
  private Action m_OnTurnOffAnimFinish;

  private void Awake() {
    m_State = ESwitchAnimState.Locked;
  }

  public override void Start() {
    base.Start();
    m_Anim.Play(kSwitchInactive);
  }

  public void PlayUnlockSwitch() {
    if (m_State == ESwitchAnimState.Locked) {
      m_State = ESwitchAnimState.Off;
      m_Anim.GotoAndPlayByProgress(kSwitchOff, 100.0f, 1);
    } else {
      PrintInvalidStateAnimCall(kSwitchOff, m_State);
    }
  }

  public void PlayTurnOnSwitch(Action onTurnOnAnimFinish) {
    if (m_State == ESwitchAnimState.Off) {
      m_State = ESwitchAnimState.On;
      m_OnTurnOnAnimFinish = onTurnOnAnimFinish;
      m_Anim.FadeIn(kSwitchOn, m_FadeInTime, 1);
    } else {
      PrintInvalidStateAnimCall(kSwitchOn, m_State);
    }
  }

  public void PlayTurnOffSwitch(Action onTurnOffAnimFinish) {
    if (m_State == ESwitchAnimState.On) {
      m_State = ESwitchAnimState.Off;
      m_OnTurnOffAnimFinish = onTurnOffAnimFinish;
      m_Anim.FadeIn(kSwitchOff, m_FadeInTime, 1);
    } else {
      PrintInvalidStateAnimCall(kSwitchOff, m_State);
    }
  }

  public enum ESwitchAnimState {
    Locked, Off, On
  }

  public ESwitchAnimState SwitchState { get { return m_State; } }

  protected override void OnAnimationComplete(string type, EventObject evObj) {
    DragonBones.AnimationState animState = evObj.animationState;
    if (animState.name.Equals(kSwitchOn)) {
      m_OnTurnOnAnimFinish?.Invoke();
      m_Anim.FadeIn(kSwitchOnRepeat, m_FadeInTime);
      m_OnTurnOnAnimFinish = null;
    } else if (animState.name.Equals(kSwitchOff)) {
      m_OnTurnOffAnimFinish?.Invoke();
      m_OnTurnOffAnimFinish = null;
    }
  }

  private void PrintInvalidStateAnimCall(string animName
                                         , ESwitchAnimState state) {
    Debug.LogWarning("Trying to play " + animName + " from non valid state "
                     + state.ToString());
  }
}
