using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = Platformer.Input;

public class PlayerManager : MonoBehaviour
{
  public EventManager m_EventManager;
  public PlayerPlatformerController m_PlayerChar;
  public Transform m_SpawnPoint;

  private Inventory m_Inventory;
  private bool m_IsDead;

  private static bool s_Registered = false;

  void Awake() {
    m_Inventory = new Inventory();
    if (!s_Registered) {
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    s_Registered = true;
  }

  void Start() {
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

  public void Die() {
    if (!m_IsDead) {
      Debug.Log("You Died");
      Input.AllowInput = false;
      m_IsDead = true;
    }
  }

  public void CompletelyDead() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    m_IsDead = false;
    Input.AllowInput = true;
  }
}
