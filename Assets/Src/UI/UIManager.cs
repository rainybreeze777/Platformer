using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
  [SerializeField] private UIInventoryListener m_UIInventoryListener;
  [SerializeField] private GameObject m_UIInventoryLabel;
  [SerializeField] private UIDialogue m_UIDialogue;

  private static UIManager s_Instance;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
    } else {
      Destroy(this.gameObject);
      return;
    }
    DontDestroyOnLoad(this.gameObject);
  }

  // Start is called before the first frame update
  void Start()
  {
    m_UIInventoryListener.gameObject.SetActive(false);
  }

  public void ShowDialogueUI() {
    m_UIDialogue.Show();
  }

  public void HideDialogueUI() {
    m_UIDialogue.Hide();
  }

  public void ShowInventory() {
    m_UIInventoryListener.gameObject.SetActive(true);
    m_UIInventoryLabel.SetActive(true);
  }

  public void HideInventory() {
    m_UIInventoryListener.gameObject.SetActive(false);
    m_UIInventoryLabel.SetActive(false);
  }

  public void PopulateDialogueUI(ActorLine actorLine) {
    m_UIDialogue.CharacterSpeak(actorLine.ActorSprite
                                , actorLine.ActorName
                                , actorLine.LineText);
  }
}
