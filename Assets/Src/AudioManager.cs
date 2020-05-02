using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  public AudioSource m_Bgm;
  public MovieController m_MovieController;

  private bool m_IsPaused = false;
  private bool m_AutoPauseAtMovie = false;
  private bool m_AutoResumeAfterMovie = false;

  private static AudioManager s_Instance;

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

  public void PlayBgm(bool autoPauseAtMovie, bool autoResumeAfterMovie) {
    m_AutoPauseAtMovie = autoPauseAtMovie;
    m_AutoResumeAfterMovie = autoResumeAfterMovie;
    m_IsPaused = false;
    m_Bgm.Play();
  }

  // Update is called once per frame
  void Update()
  {
    if (m_AutoResumeAfterMovie 
        && m_IsPaused 
        && !m_MovieController.IsMoviePlaying) {
      m_IsPaused = false;
      m_Bgm.Play();
    }
    if (m_AutoPauseAtMovie && m_MovieController.IsMoviePlaying) {
      m_IsPaused = true;
      m_Bgm.Pause();
    }
  }

  public bool IsBgmPlaying { get { return m_Bgm.isPlaying; } }
}
