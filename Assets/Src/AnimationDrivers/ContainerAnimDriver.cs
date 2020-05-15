using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(SceneObtainableItemContainer))]
public class ContainerAnimDriver : MonoBehaviour
{
  public float m_FadeInTime = 0.2f;

  public EContainerAnimNames m_OpenContainerAnimName;

  private SceneObtainableItemContainer m_Container;
  private UnityArmatureComponent m_UAC;
  private DragonBones.Animation m_Anim;
  private DragonBones.Armature m_Arma;

  void Start() {
    m_UAC = GetComponentInChildren<UnityArmatureComponent>();
    m_Anim = m_UAC.animation;
    m_Arma = m_UAC.armature;
    m_UAC.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

    m_Container = GetComponent<SceneObtainableItemContainer>();
  }

  public void PlayOpenContainerAnim() {
    m_Anim.Play(m_OpenContainerAnimName.GetName(), 1);
  }

  public enum EContainerAnimNames {
    Unknown, CofferOpen
  }
  
  private void OnAnimationComplete(string type, EventObject evObj) {
    DragonBones.AnimationState animState = evObj.animationState;
    if (animState.name.Equals(EContainerAnimNames.CofferOpen.GetName())) {
      m_Container.PopItem();
    }
  }
}

public static class EContainerAnimNamesExtensions {
  private static Dictionary<ContainerAnimDriver.EContainerAnimNames, string> s_AnimNameEnumToString =
    new Dictionary<ContainerAnimDriver.EContainerAnimNames, string> {
      { ContainerAnimDriver.EContainerAnimNames.CofferOpen, "cofferOpen" }
    };

  public static string GetName(this ContainerAnimDriver.EContainerAnimNames animEnum)
  {
    return s_AnimNameEnumToString[animEnum];
  }
}
