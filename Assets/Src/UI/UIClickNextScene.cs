using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickNextScene : MonoBehaviour, IPointerClickHandler
{
  private EventManager m_EventManager;

  void Start() {
    m_EventManager = GameObject.Find("/EventManager")
                               .GetComponent<EventManager>() as EventManager;
  }
  public void OnPointerClick(PointerEventData pointerEventData) {
    m_EventManager.Invoke<TransitionNextSceneUEvent>();
  }
}
