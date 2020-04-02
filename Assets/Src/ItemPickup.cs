using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
  SceneObtainableItem m_ObtainableItem;

  // After instantiating, wait for this period of time before picking up
  // Useful when the player throws an item to ground, prevents instant pick up
  public float m_PickupWaitTime = 1.0f;

  private float m_InitElapsedTime;

  // Start is called before the first frame update
  void Start() {
    m_ObtainableItem = gameObject.GetComponentInParent<SceneObtainableItem>() 
                        as SceneObtainableItem;
    m_InitElapsedTime = m_PickupWaitTime;
  }

  void Update() {
    if (m_InitElapsedTime > 0) {
      m_InitElapsedTime -= Time.deltaTime;
    }
  }

  void OnTriggerStay2D(Collider2D collider) {
    if (m_InitElapsedTime <= 0 && collider.tag == "Player") {
      m_ObtainableItem.ObtainThis();
    }
  }
}
