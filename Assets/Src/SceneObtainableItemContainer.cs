using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ContainerAnimDriver))]
public class SceneObtainableItemContainer : MonoBehaviour
{
  public AssetReference m_ContainedItemPrefab;
  public Vector2 m_ItemPopVector;
  public float m_ItemPopForce = 1.0f;

  private ContainerAnimDriver m_AnimDriver;
  private Transform m_MainFrame;
  private bool m_ContainerOpened = false;

  void Start() {
    m_AnimDriver = GetComponent<ContainerAnimDriver>();
    m_MainFrame = GameObject.FindWithTag("MainFrame")
                    .GetComponent<Transform>() as Transform;
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (m_ContainerOpened) { return; }
    if (collider.tag == "Player") {
      m_ContainerOpened = true;
      m_AnimDriver.PlayOpenContainerAnim();
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
