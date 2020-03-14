using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GoNextScene : MonoBehaviour
{
  private EventManager m_EventManager;

  void Start() {
    m_EventManager = GameObject.Find("/EventManager")
                               .GetComponent<EventManager>() as EventManager;
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_EventManager.Invoke<TransitionNextSceneUEvent>();
    }
  }
}
