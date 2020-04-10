using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
  [SerializeField] private UIInventoryListener m_UIInventoryListener;
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
  }

  public void ShowDialogueUI() {
    m_UIDialogue.Show();
  }

  public void HideDialogueUI() {
    m_UIDialogue.Hide();
  }

  public void PopulateDialogueUI(ActorLine actorLine) {
    m_UIDialogue.CharacterSpeak(actorLine.ActorSprite
                                , actorLine.ActorName
                                , actorLine.LineText);
  }
}
