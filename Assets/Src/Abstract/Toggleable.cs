using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Toggleable : MonoBehaviour, IToggleable {
  public abstract void NotifyToggleOn();
  public abstract void NotifyToggleOff();
}

