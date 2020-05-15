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

  void OnTriggerEnter2D(Collider2D collider) {
    Input.AllowInput = false;
    m_MovieController.PlayMovie(m_EndGameClip, null, () => {
      Application.Quit();
    });
  }
}
