using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public abstract class AnimDriver : MonoBehaviour
{
  public float m_FadeInTime = 0.2f;

  protected UnityArmatureComponent m_UAC;
  protected DragonBones.Animation m_Anim;
  protected DragonBones.Armature m_Arma;

  // Start is called before the first frame update
  public virtual void Start()
  {
    m_UAC = GetComponentInChildren<UnityArmatureComponent>();
    m_Anim = m_UAC.animation;
    m_Arma = m_UAC.armature;

    m_UAC.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
  }

  protected abstract void OnAnimationComplete(string type, EventObject evObj);
}
