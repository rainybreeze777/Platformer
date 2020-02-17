using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = Platformer.Input;

public class PlayerManager : MonoBehaviour
{
  // Since This object will not get destroyed upon unload, references
  // can be dragged in via inspector
  private EventManager m_EventManager;
  private PlayerPlatformerController m_PlayerChar;
  private Transform m_SpawnPoint;

  private Inventory m_Inventory;

  private static PlayerManager s_Instance;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
    } else {
      Destroy(this.gameObject);
    }
    DontDestroyOnLoad(this.gameObject);

    m_Inventory = new Inventory();
  }

  void Start() {
    m_PlayerChar = GameObject
                    .FindWithTag("Player")
                    .GetComponent<PlayerPlatformerController>() 
                      as PlayerPlatformerController;
    m_EventManager = GameObject.Find("/EventManager")
                           .GetComponent<EventManager>() as EventManager;
    m_EventManager.AddListener<PlayerSpawnedUEvent>(OnSpawn);
    m_EventManager.AddListener<AboutToDieUEvent>(AboutToDie);
    if (m_PlayerChar.SpawnAtSpawnPoint) {
      m_PlayerChar.transform.position = m_SpawnPoint.position;
    }
  }

  public void ObtainItem(InvtItem item) {
    if (m_Inventory.PutInItem(item)) {
      m_EventManager.Invoke<ObtainItemUEvent, InvtItem>(item);
    }   
  }

  public void SpendItem(InvtItem item) {    
    if (m_Inventory.TakeOutItem(item)) {
      m_EventManager.Invoke<SpendItemUEvent, InvtItem>(item);
    }
  }

  public void SpendItemById(string id) {
    InvtItem item = m_Inventory.TakeOutItemById(id);
    if (item != null) {
      m_EventManager.Invoke<SpendItemUEvent, InvtItem>(item);
    }
  }

  public bool HasItemById(string id) {
    return m_Inventory.HasItemById(id);
  }

  private void AboutToDie() {
    Debug.Log("You Died");
    Input.AllowInput = false;
  }

  private void OnSpawn() {
    Input.AllowInput = true;
  }
}
