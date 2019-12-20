using UnityEngine;

public class OneWayPlatform : MonoBehaviour {
  void Update() {

  }

  void OnCollisionEnter2D(Collision2D col) {
    Debug.Log("Collide");
  }
}