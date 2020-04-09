using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json.Linq;

public class TextManager : MonoBehaviour
{
  private const string kTextDataRoot = "Assets/TextData";
  private const string kTextDataExt = ".json";
  private int m_CurrentSceneIndex;

  // Character tag as Key
  private Dictionary<string, Actor> m_Actors;
  // Character tag as Key
  private Dictionary<string, Dialogue> m_Dialogues;
  // Trigger tag as key, list of dialogue lines associated as value
  private Dictionary<string, List<DialogueLine>> m_Triggers;

  private static TextManager s_Instance;

  void Awake() {
    if (s_Instance == null) {
      s_Instance = this;
    } else {
      Destroy(this.gameObject);
      return;
    }
    DontDestroyOnLoad(this.gameObject);
    SceneManager.sceneLoaded += OnSceneLoaded;
    m_CurrentSceneIndex = -1;
    m_Actors = new Dictionary<string, Actor>();
    m_Dialogues = new Dictionary<string, Dialogue>();
    m_Triggers = new Dictionary<string, List<DialogueLine>>();
  }

  public int GetNumberOfDialogueLines(string targetCharTag
                                      , EConversationType convoType) {
    var lines = GetDialogueLinesInternal(targetCharTag, convoType);
    if (lines == null) { return -1; }
    return lines.Count;
  }

  public ActorLine GetLine(string targetCharTag
                           , EConversationType convoType
                           , int lineIndex) {
    var lines = GetDialogueLinesInternal(targetCharTag, convoType);
    DialogueLine oneLine = lines[lineIndex];
    if (!m_Actors.ContainsKey(oneLine.SpeakerTag)) {
      Debug.LogError("Unable to find actor with tag " + oneLine.SpeakerTag);
      return null;
    }
    Actor speaker = m_Actors[oneLine.SpeakerTag];
    return new ActorLine {
      ActorName = speaker.Name,
      ActorSprite = speaker.Sprite,
      LineText = oneLine.LineText
    };
  }

  private List<DialogueLine> GetDialogueLinesInternal(
                              string targetCharTag
                              , EConversationType convoType) {
    if (!m_Dialogues.ContainsKey(targetCharTag))
    {
      Debug.LogWarning("No dialogue found for character " + targetCharTag);
      return null;
    }
    var lines = m_Dialogues[targetCharTag].GetLines(convoType);
    if (lines == null)
    {
      Debug.LogWarning("No conversation type " + convoType.ToString()
                       + " found for character " + targetCharTag);
      return null;
    }
    return lines;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    if (scene.buildIndex == m_CurrentSceneIndex) { return; }
    string sceneTextDataPath = kTextDataRoot + "/" + scene.name + kTextDataExt;
    Addressables.LoadAssetAsync<TextAsset>(sceneTextDataPath)
      .Completed += (asyncRes) => {
        if (asyncRes.Status == AsyncOperationStatus.Failed) {
          throw new Exception("Failed to load scene text data " 
                                        + sceneTextDataPath);
        }
        ParseTextData(asyncRes.Result.text);
        Debug.Log("Parsed out text data from " + sceneTextDataPath);
      };
  }

  private void ParseTextData(string loadedText) {
    m_Actors.Clear();

    JObject jObj = JObject.Parse(loadedText);
    
    // Parse the actors first
    foreach (JObject actorJObj in jObj["Actors"]) {
      Actor actor = new Actor { 
        Tag = actorJObj["Tag"].ToString(),
        Name = actorJObj["Name"].ToString()
      };
      string actorSpritePath = actorJObj["Sprite"].ToString();
      if (!string.IsNullOrEmpty(actorSpritePath)) {
        Addressables.LoadAssetAsync<Sprite>(actorSpritePath).Completed 
          += (asyncRes) => {
            if (asyncRes.Status != AsyncOperationStatus.Succeeded) {
              Debug.LogError("Unable to load actor sprite " + actorSpritePath);
              return;
            }
            actor.Sprite = asyncRes.Result;
          };
      }
      m_Actors.Add(actor.Tag, actor);
    }

    // Parse the dialogues next
    foreach (JProperty dialoguesJProp in jObj["Dialogues"]) {
      string targetCharTag = dialoguesJProp.Name;
      JToken targetCharDialogueCollection = dialoguesJProp.Value;
      Dialogue oneDialogue = new Dialogue();
      foreach (JProperty oneDiag in targetCharDialogueCollection) {
        List<DialogueLine> lineList = null;
        switch (oneDiag.Name) {
          case "FirstEncounter":
            lineList = oneDialogue.FirstEncounter;
            break;
          case "QuestInstructionRepeat":
            lineList = oneDialogue.QuestInstructionRepeat;
            break;
          case "QuestComplete":
            lineList = oneDialogue.QuestComplete;
            break;
          case "DefaultRepeat":
            lineList = oneDialogue.DefaultRepeat;
            break;
          default:
            Debug.LogWarning("Unrecognized Dialogue category " + oneDiag.Name);
            break;
        }
        if (lineList == null) { continue; }
        foreach (JObject lineJObj in oneDiag.Value) {
          lineList.Add(new DialogueLine {
            SpeakerTag = lineJObj["Speaker"].ToString(),
            LineText = lineJObj["Text"].ToString()
          });
        }
      }
      m_Dialogues.Add(targetCharTag, oneDialogue);
    }

    // Finally parse out the triggers
    foreach (JProperty triggersJProp in jObj["Triggers"]) {
      string triggerTag = triggersJProp.Name;
      List<DialogueLine> triggerLineCollection = new List<DialogueLine>();
      foreach (JObject triggerLineJObj in triggersJProp.Value) {
        triggerLineCollection.Add(new DialogueLine {
          SpeakerTag = triggerLineJObj["Speaker"].ToString(),
          LineText = triggerLineJObj["Text"].ToString()
        });
      }
      m_Triggers.Add(triggerTag, triggerLineCollection);
    }
  }

  private class Actor {
    public string Tag { get; set; }
    public string Name { get; set; }
    public Sprite Sprite { get; set; }
  }

  private class DialogueLine {
    public string SpeakerTag { get; set; }
    public string LineText { get; set; }
  }

  private class Dialogue {
    public List<DialogueLine> FirstEncounter { get; } = new List<DialogueLine>();
    
    public List<DialogueLine> QuestInstructionRepeat { get; } 
      = new List<DialogueLine>();

    public List<DialogueLine> QuestComplete { get; } 
      = new List<DialogueLine>();

    public List<DialogueLine> DefaultRepeat { get; } = new List<DialogueLine>();

    private Dictionary<EConversationType, List<DialogueLine>> m_LinesMapping;
    
    public Dialogue() {
      m_LinesMapping = new Dictionary<EConversationType, List<DialogueLine>>() {
        { EConversationType.FirstEncounter, FirstEncounter },
        { EConversationType.QuestInstructionRepeat, QuestInstructionRepeat },
        { EConversationType.QuestComplete, QuestComplete },
        { EConversationType.DefaultRepeat, DefaultRepeat }
      };
    }

    public List<DialogueLine> GetLines(EConversationType convoType) {
      if (m_LinesMapping.TryGetValue(convoType, out List<DialogueLine> lines)) {
        return lines;
      }
      return null;
    }
  }
}
