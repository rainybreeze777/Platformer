using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickNextScene : MonoBehaviour, IPointerClickHandler
{
  private SceneLoader m_SceneLoader;

  void Start() {
    m_SceneLoader = GameObject.Find("/SceneLoader")
                              .GetComponent<SceneLoader>() as SceneLoader;
  }

  public void OnPointerClick(PointerEventData pointerEventData) {
    m_SceneLoader.LoadNextScene();
  }
}
