using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HiddenPassageEnds : MonoBehaviour
{
  public bool m_IsExit = false;
  public HiddenPassageEnds m_OtherEnd;

  private bool m_PrepareToTrigger;
  private BoxCollider2D m_SelfCollider;

  void Start() {
    if (m_OtherEnd == null) { 
      throw new Exception("The other hidden passage end should not be null!"); 
    }
    if (m_IsExit == m_OtherEnd.m_IsExit) {
      throw new Exception("Both passage ends should not be the same!");
    }
    m_PrepareToTrigger = !m_IsExit;
    m_SelfCollider = GetComponent<BoxCollider2D>();
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      Vector2 playerPos = collider.transform.position;
      if (!m_IsExit 
          && playerPos.y >= m_SelfCollider.bounds.max.y
          && playerPos.x >= m_SelfCollider.bounds.min.x
          && playerPos.x <= m_SelfCollider.bounds.max.x) {
        (collider.gameObject.GetComponent<PlayerPlatformerController>()
          as PlayerPlatformerController).FlipHiddenPassageLayer(true);
        m_OtherEnd.PrepareToTrigger();
      } else if (m_IsExit && m_PrepareToTrigger) {
        (collider.gameObject.GetComponent<PlayerPlatformerController>()
          as PlayerPlatformerController).FlipHiddenPassageLayer(false);
        m_PrepareToTrigger = false;
      }
    }
  }

  public void PrepareToTrigger() {
    m_PrepareToTrigger = true;
  }
}
