using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Input = Platformer.Input;

[RequireComponent(typeof(Collider2D))]
public class EndGameArea : MonoBehaviour
{
  public MovieController m_MovieController;
  public VideoPlayer m_EndGameClip;

  private UIManager m_UIManager;

  void Start() {
    m_UIManager = GameObject.Find("/UICanvas").GetComponent<UIManager>();
  }

  void OnTriggerEnter2D(Collider2D collider) {
    Input.AllowInput = false;
    m_UIManager.HideInventory();
    m_MovieController.ResetFadeListenerThenPlayMovie(m_EndGameClip, null, () => {
      Application.Quit();
    });
  }
}
