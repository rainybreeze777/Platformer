using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HiddenUnlockable : MonoBehaviour, IUnlockable
{
  public List<SceneObtainableItem> m_UnlockItems;
  public List<Toggleable> m_ToggleTargets;
  public SecretTriggerAction m_Secret;
  public EventManager m_EventManager;

  private bool m_IsUnlocked = false;
  private bool m_PlayerInRange = false;
  private List<string> m_UnlockItemIds;
  private List<string> m_ItemIdsNeeded;
  private List<SceneObtainableItem> m_CurrentInRangeItems;
  private SecretTriggerAction m_SecretTracker;

  void Start() {
    m_UnlockItemIds = new List<string>();
    foreach (var item in m_UnlockItems) {
      m_UnlockItemIds.Add(item.ItemId);
    }
    m_ItemIdsNeeded = new List<string>(m_UnlockItemIds);
    m_CurrentInRangeItems = new List<SceneObtainableItem>();
    m_SecretTracker = m_Secret.Clone();
    m_EventManager.AddListener<JumpUEvent>(() => { 
      ResolveSecret(ETriggerAction.Jump);
    });
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_PlayerInRange = true;
      return;
    }
    SceneObtainableItem item =
      collider.gameObject.GetComponent<SceneObtainableItem>()
        as SceneObtainableItem;
    if (item != null && !m_IsUnlocked) {
      m_CurrentInRangeItems.Add(item);
      if (m_UnlockItemIds.Contains(item.ItemId)) {
        m_ItemIdsNeeded.Remove(item.ItemId);
      }
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_PlayerInRange = false;
      m_SecretTracker = m_Secret.Clone();
      return;
    }
    SceneObtainableItem item =
      collider.gameObject.GetComponent<SceneObtainableItem>()
        as SceneObtainableItem;
    if (item != null && !m_IsUnlocked) {
      m_CurrentInRangeItems.Remove(item);
      if (m_UnlockItemIds.Contains(item.ItemId)) {
        m_ItemIdsNeeded.Add(item.ItemId);
        m_SecretTracker = m_Secret.Clone();
      }
    }
  }

  public void Unlock() {
    if (!m_IsUnlocked) {
      foreach (string unlockId in m_UnlockItemIds) {
        foreach (var inRangeItem in m_CurrentInRangeItems) {
          if (inRangeItem.ItemId == unlockId) {
            m_CurrentInRangeItems.Remove(inRangeItem);
            Destroy(inRangeItem.gameObject);
            break;
          }
        }
      }
      m_IsUnlocked = true;
      foreach (Toggleable t in m_ToggleTargets) {
        t.NotifyToggleOn();
      }
    }
  }

  private void ResolveSecret(ETriggerAction triggerAction) {
    if (!m_IsUnlocked && m_PlayerInRange && m_ItemIdsNeeded.Count == 0) {
      if (triggerAction == m_SecretTracker.m_TriggerAction) {
        --m_SecretTracker.m_ActionValue;
      }
      if (m_SecretTracker.m_ActionValue == 0) {
        Unlock();
      } 
    }
  }

  [System.Serializable]
  public class SecretTriggerAction
  {
    public ETriggerAction m_TriggerAction;
    public int m_ActionValue;
    public SecretTriggerAction Clone() {
      SecretTriggerAction ret = new SecretTriggerAction();
      ret.m_TriggerAction = m_TriggerAction;
      ret.m_ActionValue = m_ActionValue;
      return ret;
    }
  }

  public enum ETriggerAction { None, Jump }
}
