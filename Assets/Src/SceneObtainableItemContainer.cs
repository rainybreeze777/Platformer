using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(Collider2D))]
public class SceneObtainableItemContainer : MonoBehaviour
{
  public AssetReference m_ContainedItemPrefab;
  public Vector2 m_ItemPopVector;
  public float m_ItemPopForce = 1.0f;

  private Animator m_ContainerAnimator;
  private Transform m_MainFrame;
  private bool m_ContainerOpened = false;

  void Start() {
    m_ContainerAnimator = GetComponentInChildren<Animator>() as Animator;
    m_MainFrame = GameObject.FindWithTag("MainFrame")
                    .GetComponent<Transform>() as Transform;
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (m_ContainerOpened) { return; }
    if (collider.tag == "Player") {
      m_ContainerOpened = true;
      m_ContainerAnimator.SetTrigger("shouldOpen");
    }
  }

  public void PopItem() {
    m_ContainedItemPrefab
      .InstantiateAsync(transform.position, Quaternion.identity, m_MainFrame)
      .Completed += (asyncRes) => {
        asyncRes.Result.GetComponent<Rigidbody2D>()
                       .AddForce(m_ItemPopVector.normalized * m_ItemPopForce
                                 , ForceMode2D.Impulse);
      };
  }
}
