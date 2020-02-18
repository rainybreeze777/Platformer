using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GoNextScene : MonoBehaviour
{
  private SceneLoader m_SceneLoader;

  void Start() {
    m_SceneLoader = GameObject.Find("/SceneLoader")
                              .GetComponent<SceneLoader>() as SceneLoader;
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_SceneLoader.LoadNextScene();
    }
  }
}
