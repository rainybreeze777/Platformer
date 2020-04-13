using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scene Obtainable Item Container AnimEventListener
public class SOIContainerAnimEventListener : AnimEventListener
{
  private SceneObtainableItemContainer m_Container;

  void Start() {
    m_Container = GetComponentInParent<SceneObtainableItemContainer>()
                    as SceneObtainableItemContainer;
  }

  void OnContainerOpen() {
    m_Container.PopItem();
  }
}
