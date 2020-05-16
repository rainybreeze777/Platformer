using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class PlayMovieAtStart : MonoBehaviour
{
  public VideoPlayer m_StartMovie;
  public MovieController m_MovieController;
  public AudioManager m_AudioManager;

  private SceneLoader m_SceneLoader;
  private UIManager m_UIManager;

  // Start is called before the first frame update
  void Start()
  {
    // This class should be set to be initialized last in Unity project
    // settings, so that everything else is already initialized and ready,
    // avoiding initialization order problems.
    m_SceneLoader = GameObject.Find("/SceneLoader")
                              .GetComponent<SceneLoader>() as SceneLoader;
    m_UIManager = GameObject.Find("/UICanvas").GetComponent<UIManager>();
    if (!m_SceneLoader.IsReloadedScene) {
      m_MovieController.PlayMovie(m_StartMovie, null, () => {
        m_UIManager.ShowInventory();
        m_AudioManager.PlayBgm(true, true);
      });
    }
  }
}
