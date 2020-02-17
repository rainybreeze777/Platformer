using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public bool m_IsStartup = false;
  public EventManager m_EventManager;

  private int m_NextSceneIndex;

  private static SceneLoader s_Instance;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
      SceneManager.sceneLoaded += OnSceneLoaded;
    } else {
      Destroy(this.gameObject);
    }
    DontDestroyOnLoad(this.gameObject);
  }

  void Start() {
    m_EventManager.AddListener<DeadUEvent>(RestartScene);

    if (m_IsStartup) {
      // Todo: start up, prepare to load saved file
    } else {
      m_NextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }
  }

  public void LoadNextScene() {
    SceneManager.LoadScene(m_NextSceneIndex);
  }

  public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    m_EventManager.Invoke<PlayerSpawnedUEvent>();
  }

  private void RestartScene() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
