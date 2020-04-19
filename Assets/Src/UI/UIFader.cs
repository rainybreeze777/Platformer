using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RawImage))]
public class UIFader : MonoBehaviour
{
  public float m_FadeSeconds = 1.0f;

  public UnityEvent m_FadeInBlackComplete;
  public UnityEvent m_FadeOutBlackComplete;

  private RawImage m_BlackScreen;
  private CanvasRenderer m_Renderer;

  private bool m_InitiatedFade = false;

  void Start() {
    m_BlackScreen = GetComponent<RawImage>() as RawImage;
    m_Renderer = GetComponent<CanvasRenderer>() as CanvasRenderer;
  }

  void Update() {
    if (m_InitiatedFade && IsFadeOutComplete) {
      m_InitiatedFade = false;
      m_FadeOutBlackComplete.Invoke();
      Debug.Log("Fade out black complete");
    } else if (m_InitiatedFade && IsFadeInComplete) {
      m_InitiatedFade = false;
      m_FadeInBlackComplete.Invoke();
      Debug.Log("Fade in black complete");
    }
  }

  public void FadeInBlack() { 
    StartCoroutine(FadeBlackScreen(true)); 
  }
  public void FadeOutBlack() { StartCoroutine(FadeBlackScreen(false)); }

  IEnumerator FadeBlackScreen(bool inOrOut) {
    float targetAlpha = inOrOut ? 1.0f : 0.0f;
    m_BlackScreen.CrossFadeAlpha(targetAlpha, m_FadeSeconds, false);
    m_InitiatedFade = true;
    yield return null;
  }

  public float FadeTime { get { return m_FadeSeconds; } }

  private bool IsFadeOutComplete { get { return m_Renderer.GetAlpha() == 0; }}
  private bool IsFadeInComplete { get { return m_Renderer.GetAlpha() == 1; } }
}
