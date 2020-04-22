using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Input = Platformer.Input;

public class UILandingPage : MonoBehaviour
{
  public MovieController m_MovieController;
  public VideoPlayer m_Opening;
  public VideoPlayer m_Countdown;

  public GameObject m_PressToContinue;
  public GameObject m_StartMenu;

  private bool m_UIEnabled = false;
  private bool m_WaitingToContinue = true;

  // Start is called before the first frame update
  void Start()
  {
    m_MovieController.PlayMovie(m_Opening, null, EnableAllUI);
  }

  // Update is called once per frame
  void Update()
  {
    if (m_UIEnabled && m_WaitingToContinue && Input.AnyKeyDown) {
      Debug.Log("Continuing");
      m_WaitingToContinue = false;
      m_MovieController.PlayMovie(m_Countdown, DisableAllUI, () => {
        EnableAllUI();
        m_PressToContinue.SetActive(false);
        m_StartMenu.SetActive(true);
      });
    }
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
