using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFade : MonoBehaviour
{
  public float m_FadeSpeed = 1f;
  public string m_TriggerTag;

  private SpriteRenderer m_Renderer;
  private bool m_ColliderInside = false;

  void Awake()
  {
    m_Renderer = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update()
  {
  }

  void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == m_TriggerTag) {
      m_ColliderInside = true;
      StartCoroutine("FadeOut");
    }
  }

  void OnTriggerExit2D(Collider2D collision) {
    if (collision.tag == m_TriggerTag) {
      m_ColliderInside = false;
      StartCoroutine("FadeIn");
    }
  }

  IEnumerator FadeOut() {
    Color color = m_Renderer.color;
    while (m_ColliderInside && color.a > 0)
    {
      color.a -= m_FadeSpeed * Time.deltaTime;
      m_Renderer.color = color;
      yield return null;
    }
  }

  IEnumerator FadeIn() {
    Color color = m_Renderer.color;
    while (!m_ColliderInside && color.a < 1)
    {
      color.a += m_FadeSpeed * Time.deltaTime;
      m_Renderer.color = color;
      yield return null;
    }
  }
}
