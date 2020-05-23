using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Input = Platformer.Input;

[RequireComponent(typeof(Collider2D))]
public class ConversableChar : MonoBehaviour
{
  public string m_CharacterTag;
  public bool m_HasQuest;
  public List<SceneObtainableItem> m_NeededQuestItems;
  public ShowTipWhenInZone m_ShowTip;

  private TextManager m_TextManager;
  private PlayerManager m_PlayerManager;
  private UIManager m_UIManager;
  
  private bool m_PlayerInRange = false;
  private bool m_IsEngagedInConvo = false;
  private bool m_GetNextLine = false;
  private ConversationStateMachine m_CSM;
  private List<string> m_NeededQuestItemIds;

  // Start is called before the first frame update
  void Start()
  {
    m_TextManager = GameObject.Find("/TextManager").GetComponent<TextManager>() 
                      as TextManager;
    m_PlayerManager = GameObject.Find("/PlayerManager")
                        .GetComponent<PlayerManager>()
                          as PlayerManager;
    m_UIManager = GameObject.Find("/UICanvas").GetComponent<UIManager>()
                    as UIManager;
    if (m_HasQuest) {
      m_CSM = new ConversationStateMachine(HasAllItemsAndSpendIfTrue);
    } else {
      m_CSM = new ConversationStateMachine();
    }
    m_NeededQuestItemIds = new List<string>();
    foreach (var item in m_NeededQuestItems) {
      m_NeededQuestItemIds.Add(item.ItemId);
    }
    if (string.IsNullOrEmpty(m_CharacterTag)) {
      throw new Exception("ConversableChar has empty character tag!");
    }
  }

  void Update() {
    if (m_PlayerInRange) {
      if (m_IsEngagedInConvo
          && Input.GetActionButtonDownIgnoreAllowInput()) {
        m_GetNextLine = true;
      } else if (Input.GetButtonDown("Action")) {
        EConversationType convoType = m_CSM.AdvanceState();
        StartCoroutine(EngageInConversation(m_CharacterTag, convoType));
        m_ShowTip?.ForceStopShowTips();
      }
    }
  }
  
  void OnTriggerEnter2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_PlayerInRange = true;
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    if (collider.tag == "Player") {
      m_PlayerInRange = false;
    }
  }

  private bool HasAllItemsAndSpendIfTrue() {
    foreach (var itemId in m_NeededQuestItemIds) {
      if (!m_PlayerManager.HasItemById(itemId)) {
        return false;
      }
    }
    foreach (var itemId in m_NeededQuestItemIds) {
      m_PlayerManager.SpendItemById(itemId);
    }
    return true;
  }

  private IEnumerator EngageInConversation(string charTag,
                                           EConversationType convoType) {
    m_IsEngagedInConvo = true;
    Input.AllowInput = false;
    m_UIManager.ShowDialogueUI();
    int linesCount = m_TextManager.GetNumberOfDialogueLines(charTag, convoType);
    for (int lineIndex = 0; lineIndex < linesCount; ++lineIndex) {
      ActorLine line = m_TextManager.GetLine(charTag, convoType, lineIndex);
      m_UIManager.PopulateDialogueUI(line);
      yield return new WaitUntil(() => m_GetNextLine);
      m_GetNextLine = false;
    }
    m_UIManager.HideDialogueUI();
    Input.AllowInput = true;
    m_IsEngagedInConvo = false;
  }

  private enum EConversationState {
    NeverMet, AlreadyMet, WaitingForQuest
  }

  private class ConversationStateMachine {
    private bool m_IsQuestGiver;
    private Func<bool> m_CheckQuestComplete;

    public ConversationStateMachine() {
      m_IsQuestGiver = false;
      State = EConversationState.NeverMet;
    }

    public ConversationStateMachine(Func<bool> checkQuest) {
      m_IsQuestGiver = true;
      State = EConversationState.NeverMet;
      m_CheckQuestComplete = checkQuest;
    }

    public EConversationType AdvanceState() {
      EConversationType ret = EConversationType.None;
      switch (State) {
        case EConversationState.NeverMet:
          if (m_IsQuestGiver) {
            State = EConversationState.WaitingForQuest;
          } else {
            State = EConversationState.AlreadyMet;
          }
          ret = EConversationType.FirstEncounter;
          break;
        case EConversationState.WaitingForQuest:
          if (m_CheckQuestComplete()) {
            State = EConversationState.AlreadyMet;
            ret = EConversationType.QuestComplete;
          } else {
            ret = EConversationType.QuestInstructionRepeat;
          }
          break;
        case EConversationState.AlreadyMet:
          ret = EConversationType.DefaultRepeat;
          break;
      }

      return ret;
    }

    public EConversationState State { get; private set; }
  }
}
