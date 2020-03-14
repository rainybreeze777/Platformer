using UnityEngine;

namespace Platformer {
public class Input : UnityEngine.Input {

  private static bool s_AllowInput = true;

  public static new float GetAxis(string axisName) {
    if (!s_AllowInput) { return 0; }
    return UnityEngine.Input.GetAxis(axisName);
  }

  public static new bool GetButton(string buttonName) {
    if (!s_AllowInput) { return false; }
    return UnityEngine.Input.GetButton(buttonName);
  }

  public static new bool GetButtonDown(string buttonName) {
    if (!s_AllowInput) { return false; }
    return UnityEngine.Input.GetButtonDown(buttonName);
  }

  public static new bool GetButtonUp(string buttonName) {
    if (!s_AllowInput) { return false; }
    return UnityEngine.Input.GetButtonUp(buttonName);
  }

  public static bool AllowInput { 
    get { return s_AllowInput; }
    set { s_AllowInput = value; }
  }
}
}