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
  
  private PlayerManager m_PlayerManager;
  private int m_NextSceneIndex;
  private SavePoint m_SavePoint;
  private string m_SavePointFullpath;
  private int m_BeforeLoadSceneIndex = -2;
  private int m_CurrentSceneIndex = -1;
  private static SceneLoader s_Instance;

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
      m_SavePoint = InitializeSavePoint(m_SavePointFullpath);
      m_NextSceneIndex = m_SavePoint.sceneIndex;
    } else {
      m_SavePoint = new SavePoint(SceneManager.GetActiveScene().buildIndex);
      Debug.Log("New save point created");
      m_NextSceneIndex = m_SavePoint.sceneIndex + 1;
    }
  }

  void Start() {
    m_EventManager.AddListener<TransitionNextSceneUEvent>(LoadNextScene);
  }

  public void LoadNextScene() {
    SceneManager.LoadScene(m_NextSceneIndex);
    m_SavePoint.sceneIndex = m_NextSceneIndex;
    if (m_PlayerManager == null) {
      var pmObj = GameObject.Find("/PlayerManager");
      if (pmObj != null) {
        m_PlayerManager = pmObj.GetComponent<PlayerManager>() as PlayerManager;
        m_SavePoint.inventoryItems = m_PlayerManager.AllItems;
      }
    }
    if (m_ShouldSave) {
      File.WriteAllText(m_SavePointFullpath, JsonUtility.ToJson(m_SavePoint));
    }
    m_NextSceneIndex += 1;
  }

  public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    m_BeforeLoadSceneIndex = m_CurrentSceneIndex;
    m_CurrentSceneIndex = scene.buildIndex;
    m_EventManager.Invoke<PlayerSpawnedUEvent>();
  }

  public void RestartScene() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public bool IsReloadedScene { 
    get { 
      return m_CurrentSceneIndex == m_BeforeLoadSceneIndex; 
    }
  }

  public SavePoint GetSavePoint() { return m_SavePoint; }

  private SavePoint InitializeSavePoint(string savePointFilePath) {
    SavePoint sp = JsonUtility.FromJson<SavePoint>(
                    File.ReadAllText(savePointFilePath));
    sp.InitInvtItems();
    Debug.Log("Save point loaded from " + savePointFilePath);
    return sp;
  }
}
