using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{

  public Text m_SpeakerName;
  public Text m_LineText;
  public Sprite m_SpeakerSprite;

  public void Show() {
    gameObject.SetActive(true);
  }

  public void Hide() {
    gameObject.SetActive(false);
  }

  public void CharacterSpeak(Sprite charSprite, string charName, string text) {
    m_SpeakerName.text = charName;
    m_LineText.text = text;
  }
}
