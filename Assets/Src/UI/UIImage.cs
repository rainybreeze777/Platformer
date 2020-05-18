using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImage : MonoBehaviour
                       , IPointerClickHandler
                       , IPointerEnterHandler
                       , IPointerExitHandler
{
  public GameObject m_ItemHighlight;

  private string m_ItemId;
  private PlayerManager m_PlayerManager;

  public void Init(PlayerManager playerManager
                   , string itemId
                   , Sprite uiSprite) {
    m_ItemId = itemId;
    m_PlayerManager = playerManager;
    Image image = GetComponent<Image>();
    image.preserveAspect = true;
    image.sprite = uiSprite;
  }

  public void OnPointerClick(PointerEventData pointerEventData) {
    m_PlayerManager.DropItemById(m_ItemId);
  }

  public void OnPointerEnter(PointerEventData pointerEventData) {
    m_ItemHighlight.SetActive(true);
  }

  public void OnPointerExit(PointerEventData pointerEventData) {
    m_ItemHighlight.SetActive(false);
  }
}
