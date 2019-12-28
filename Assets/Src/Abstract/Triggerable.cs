using System;
using UnityEngine;

[System.Serializable]
public abstract class Triggerable : MonoBehaviour, ITriggerable {
  public abstract void NotifyTriggerEnter();
  public abstract void NotifyTriggerExit();
}