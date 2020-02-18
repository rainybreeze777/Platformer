using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public bool m_ShouldSave = false;
  public string m_SaveFilename;
  public EventManager m_EventManager;

  private static SceneLoader s_Instance;
  private int m_NextSceneIndex;
  private SavePoint m_SavePoint;
  private string m_SavePointFullpath;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
      SceneManager.sceneLoaded += OnSceneLoaded;
    } else {
      Destroy(this.gameObject);
      return;
    }
    DontDestroyOnLoad(this.gameObject);

    m_SavePointFullpath = Path.Combine(
                            Application.persistentDataPath
                            , m_SaveFilename);
    Debug.Log("Save Path file is " + m_SavePointFullpath);
    if (File.Exists(m_SavePointFullpath)) {
      m_SavePoint = JsonUtility.FromJson<SavePoint>(
                      File.ReadAllText(m_SavePointFullpath));
      Debug.Log("Save point loaded from " + m_SavePointFullpath);
      m_NextSceneIndex = m_SavePoint.sceneIndex;
    } else {
      m_SavePoint = new SavePoint();
      m_SavePoint.sceneIndex = SceneManager.GetActiveScene().buildIndex;
      Debug.Log("New save point created");
      m_NextSceneIndex = m_SavePoint.sceneIndex + 1;
    }
  }

  void Start() {
    m_EventManager.AddListener<DeadUEvent>(RestartScene);
  }

  public void LoadNextScene() {
    SceneManager.LoadScene(m_NextSceneIndex);
    m_SavePoint.sceneIndex = m_NextSceneIndex;
    if (m_ShouldSave) {
      File.WriteAllText(m_SavePointFullpath, JsonUtility.ToJson(m_SavePoint));
    }
    m_NextSceneIndex += 1;
  }

  public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    m_EventManager.Invoke<PlayerSpawnedUEvent>();
  }

  private void RestartScene() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
