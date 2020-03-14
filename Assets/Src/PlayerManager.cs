﻿using System;
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
  [SerializeField] private PlayerPlatformerController m_PlayerChar;
  private SpawnPoint[] m_SpawnPoints;

  private Inventory m_Inventory;
  private Inventory m_LevelStartInventory;

  private static PlayerManager s_Instance;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
    } else {
      Destroy(this.gameObject);
      return;
    }
    DontDestroyOnLoad(this.gameObject);

    m_Inventory = new Inventory();
  }

  void Start() {
    m_EventManager = GameObject.Find("/EventManager")
                           .GetComponent<EventManager>() as EventManager;
    if (m_PlayerChar == null) {
      m_PlayerChar = GameObject.FindGameObjectWithTag("Player")
                               .GetComponent<PlayerPlatformerController>() 
                                  as PlayerPlatformerController;
    }
    m_EventManager.AddListener<PlayerSpawnedUEvent>(OnSpawn);
    m_EventManager.AddListener<AboutToDieUEvent>(AboutToDie);
    m_EventManager.AddListener<TransitionNextSceneUEvent>(SaveInventory);
    InitSpawnPoints();
    var sceneLoader = GameObject.Find("/SceneLoader")
                                .GetComponent<SceneLoader>() as SceneLoader;
    if (sceneLoader != null) {
      m_Inventory = new Inventory(sceneLoader.GetSavePoint().AllItems);
      m_LevelStartInventory = m_Inventory.Clone();
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

  public List<InvtItem> AllItems { get { return m_Inventory.AllItems; } }

  private void AboutToDie() {
    Debug.Log("You Died");
    Input.AllowInput = false;
  }

  private void OnSpawn() {
    m_PlayerChar = GameObject.FindWithTag("Player")
                             .GetComponent<PlayerPlatformerController>()
                                as PlayerPlatformerController;
    InitSpawnPoints();
    if (m_LevelStartInventory != null) {
      m_Inventory = m_LevelStartInventory.Clone();
    }
    Input.AllowInput = true;
  }

  private void InitSpawnPoints() {
    GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    m_SpawnPoints = new SpawnPoint[spawnPoints.Length];
    foreach (GameObject gObj in spawnPoints)
    {
      SpawnPoint point = gObj.GetComponent<SpawnPoint>() as SpawnPoint;
      if (point.SpawnIndex >= m_SpawnPoints.Length)
      {
        point.ThrowIndexOutOfRangeException(m_SpawnPoints.Length);
      }
      if (m_SpawnPoints[point.SpawnIndex] != null)
      {
        point.ThrowDuplicateSpawnPointsException();
        continue;
      }
      m_SpawnPoints[point.SpawnIndex] = point;
    }

    if (m_PlayerChar.SpawnAtSpawnPoint) {
      m_PlayerChar.transform.position = m_SpawnPoints[0].transform.position;
    }
  }

  private void SaveInventory() {
    m_LevelStartInventory = m_Inventory.Clone();
  }
}
