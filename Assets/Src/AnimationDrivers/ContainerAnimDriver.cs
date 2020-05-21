using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(SceneObtainableItemContainer))]
public class ContainerAnimDriver : AnimDriver
{
  public EContainerAnimNames m_OpenContainerAnimName;

  private SceneObtainableItemContainer m_Container;

  public override void Start() {
    base.Start();
    m_Container = GetComponent<SceneObtainableItemContainer>();
  }

  public void PlayOpenContainerAnim() {
    m_Anim.Play(m_OpenContainerAnimName.GetName(), 1);
  }

  public enum EContainerAnimNames {
    Unknown, CofferOpen
  }
  
  protected override void OnAnimationComplete(string type, EventObject evObj) {
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
