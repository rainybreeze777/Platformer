using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Input = Platformer.Input;

public class UILandingPage : MonoBehaviour
{
  public GameObject m_StartMenu;
  public UIFader m_Fader;

  private bool m_UIEnabled = true;
  private AudioSource m_MenuLoopMusic;

  // Start is called before the first frame update
  void Start()
  {
    m_MenuLoopMusic = GetComponent<AudioSource>();
    m_Fader.FadeOutBlack();
  }
  
  private void EnableAllUI() {
    foreach (RectTransform rect in transform) {
      rect.gameObject.SetActive(true);
    }
    m_UIEnabled = true;
  }

  private void DisableAllUI() {
    foreach (RectTransform rect in transform) {
      rect.gameObject.SetActive(false);
    }
    m_UIEnabled = false;
  }
}
