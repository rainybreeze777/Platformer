using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class PlayerAnimDriver : MonoBehaviour
{
  public float m_FadeInTime = 0.2f;

  private UnityArmatureComponent m_UAC;
  private DragonBones.Animation m_Anim;
  private DragonBones.Armature m_Arma;

  private float m_HVelocity;
  private float m_VVelocity;

  private const string kIdleAnimName = "01Idle";
  private const string kRunAnimName = "02Run";
  private const string kDeathAnimName = "03Death";
  private const string kStartJumpAnimName = "04Jump_Part1";
  private const string kJumpingAnimName = "04Jump_Part2_Repeat";
  private const string kJumpToFallAnimName = "05Fall_Part1";
  private const string kFallingAnimName = "05Fall_Part2_Repeat";
  private const string kLandingAnimName = "05Fall_Part3";

  private Action m_OnDeathAnimFinish;

  void Awake() {
    AnimState = EPlayerAnimState.Idle;
  }

  // Start is called before the first frame update
  void Start() {
    m_UAC = GetComponentInChildren<UnityArmatureComponent>();
    m_Anim = m_UAC.animation;
    m_Arma = m_UAC.armature;

    m_UAC.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

    m_Anim.Play(kIdleAnimName);
  }

  public bool IsGrounded { get; set; }

  public float HorizontalVelocity {
    get { return m_HVelocity; }
    set {
      m_HVelocity = value;
      if (m_HVelocity < -0.01f || m_HVelocity > 0.01f) {
        bool flipSprite = (m_Arma.flipX ? (value > 0.01f) : (value < -0.01f));
        if (flipSprite) {
          m_Arma.flipX = !m_Arma.flipX;
        }
        if (AnimState == EPlayerAnimState.Idle) {
          AnimState = EPlayerAnimState.Running;
          m_Anim.FadeIn(kRunAnimName, m_FadeInTime);
        }
      } else {
        if (AnimState == EPlayerAnimState.Running) {
          AnimState = EPlayerAnimState.Idle;
          m_Anim.FadeIn(kIdleAnimName, m_FadeInTime);
        }
      }
    }
  }

  public float VerticalVelocity {
    get { return m_VVelocity; }
    set {
      m_VVelocity = value;
      if (!IsGrounded && m_VVelocity > 0.01f) {
        switch (AnimState) {
          case EPlayerAnimState.Idle:
          case EPlayerAnimState.Running:
            AnimState = EPlayerAnimState.StartJump;
            m_Anim.Play(kStartJumpAnimName, 1);
            break;
          default:
            break;
        }
      } else if (!IsGrounded && m_VVelocity < -0.01f) {
        switch (AnimState) {
          case EPlayerAnimState.Idle:
          case EPlayerAnimState.Running:
          case EPlayerAnimState.JumpingUp:
            AnimState = EPlayerAnimState.JumpToFall;
            m_Anim.Play(kJumpToFallAnimName, 1);
            break;
          default:
            break;
        }
      } else {
        switch (AnimState) {
          case EPlayerAnimState.JumpToFall:
          case EPlayerAnimState.Falling:
            AnimState = EPlayerAnimState.Landing;
            m_Anim.Play(kLandingAnimName, 1);
            break;
          default:
            break;
        }
      }
    }
  }

  public int SortingOrder {
    get { return m_UAC.sortingGroup.sortingOrder; }
    set {
      m_UAC.sortingGroup.sortingOrder = value;
    }
  }

  public void PlayerDies(Action onDeathAnimationFinish) {
    AnimState = EPlayerAnimState.Death;
    m_Anim.FadeIn(kDeathAnimName, m_FadeInTime, 1);
    m_OnDeathAnimFinish = onDeathAnimationFinish;
  }

  public EPlayerAnimState AnimState { get; private set; }

  public enum EPlayerAnimState {
    Idle, Running, StartJump, JumpingUp, JumpToFall, Falling, Landing, Death
  }

  private void OnAnimationComplete(string type, EventObject evObj) {
    DragonBones.AnimationState animState = evObj.animationState;
    if (animState.name.Equals(kStartJumpAnimName)) {
      AnimState = EPlayerAnimState.JumpingUp;
      m_Anim.FadeIn(kJumpingAnimName, m_FadeInTime);
    } else if (animState.name.Equals(kJumpToFallAnimName)) {
      AnimState = EPlayerAnimState.Falling;
      m_Anim.FadeIn(kFallingAnimName, m_FadeInTime);
    } else if (animState.name.Equals(kLandingAnimName)) {
      if (HorizontalVelocity < -0.01f || HorizontalVelocity > 0.01f) {
        AnimState = EPlayerAnimState.Running;
        m_Anim.FadeIn(kRunAnimName, m_FadeInTime);
      } else {
        AnimState = EPlayerAnimState.Idle;
        m_Anim.FadeIn(kIdleAnimName, m_FadeInTime);
      }
    } else if (animState.name.Equals(kDeathAnimName)) {
      m_OnDeathAnimFinish?.Invoke();
      m_OnDeathAnimFinish = null;
    }
  }
}
