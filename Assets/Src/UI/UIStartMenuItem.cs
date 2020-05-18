using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIStartMenuItem : MonoBehaviour, IPointerClickHandler
{
  public MenuButton m_ButtonType;
  public UIFader m_Fader;

  private EventManager m_EventManager;

  void Start() {
    m_EventManager = GameObject.Find("/EventManager")
                               .GetComponent<EventManager>() as EventManager;
  }
  public void OnPointerClick(PointerEventData pointerEventData) {
    switch (m_ButtonType) {
      case MenuButton.Start:
        m_Fader.m_FadeInBlackComplete.AddListener(() => {
          m_EventManager.Invoke<TransitionNextSceneUEvent>();
        });
        break;
      case MenuButton.Exit:
        m_Fader.m_FadeInBlackComplete.AddListener(() => { Application.Quit(); });
        break;
    }
    m_Fader.FadeInBlack();
  }

  public enum MenuButton {
    Start, Exit
  }
}
