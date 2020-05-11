using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class PlayerAnimDriver : MonoBehaviour
{
  public float m_FadeInTime = 0.2f;

  private DragonBones.Animation m_Anim;
  private DragonBones.Armature m_Arma;

  private float m_HVelocity;

  private const string kIdleAnimName = "01Idle";
  private const string kRunAnimName = "02Run";

  void Awake() {
    AnimState = EPlayerAnimState.Idle;
  }

  // Start is called before the first frame update
  void Start() {
    m_Anim = GetComponentInChildren<UnityArmatureComponent>().animation;
    m_Arma = GetComponentInChildren<UnityArmatureComponent>().armature;
    m_Anim.Play(kIdleAnimName);
  }

  // Update is called once per frame
  void Update()
  {
    
  }

  public float HorizontalVelocity {
    get { return m_HVelocity; }
    set {
      m_HVelocity = value;
      if (m_HVelocity != 0) {
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

  public EPlayerAnimState AnimState { get; private set; }

  public enum EPlayerAnimState {
    Idle, Running, StartJump, JumpingUp, JumpToFall, Falling, Landing, Death
  }
}
