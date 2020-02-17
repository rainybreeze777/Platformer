using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
  public int m_SpawnIndex;

  public int SpawnIndex { get { return m_SpawnIndex; }}

  public void ThrowIndexOutOfRangeException(int totalFoundSpawnPoints) {
    throw new IndexOutOfRangeException(
      "Spawn index " + m_SpawnIndex + " out of range, " 
      + "total number of spawn points in current scene: "
      + totalFoundSpawnPoints);
  }

  public void ThrowDuplicateSpawnPointsException()
  {
    throw new InvalidOperationException("Duplicate spawn point indices "
      + m_SpawnIndex + " found!");
  }
}
