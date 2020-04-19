using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Input = Platformer.Input;

public class StartSceneMovieController : MonoBehaviour
{
  public UIFader m_Fader;

  private MoviePlayStatus m_PlayStatus;

  public void PlayMovie(VideoPlayer vid, Action onMovieStart, Action onMovieEnd) {
    m_PlayStatus = new MoviePlayStatus(vid, m_Fader);
    m_PlayStatus.m_OnMovieStart = onMovieStart;
    vid.prepareCompleted += (source) => {
      m_PlayStatus.SetMovieReady();
    };
    vid.loopPointReached += (source) => {
      onMovieEnd?.Invoke();
      m_PlayStatus = null;
    };
    m_Fader.m_FadeInBlackComplete.AddListener(m_PlayStatus.SetAllBlack);
    vid.Prepare();
    m_Fader.FadeInBlack();
    StartCoroutine(m_PlayStatus.FadeInBlackWhenMovieAlmostEnd());
  }

  void Update() {
    if (m_PlayStatus != null && Input.GetKeyDown(KeyCode.Escape)) {
      StopCoroutine(m_PlayStatus.FadeInBlackWhenMovieAlmostEnd());
      m_PlayStatus.SkipMovie();
    }
  }

  private class MoviePlayStatus {
    public bool IsAllBlack { get; private set; } = false;
    public bool IsMovieReady { get; private set; } = false;

    public Action m_OnMovieStart;

    private float m_StartFadeInBlackSeconds;
    private bool m_MoviePlaying = false;
    private VideoPlayer m_Movie;
    private UIFader m_Fader;

    public MoviePlayStatus(VideoPlayer movie, UIFader fader) {
      m_Movie = movie;
      m_Fader = fader;
      movie.loopPointReached += (source) => {
        movie.gameObject.SetActive(false);
        m_Fader.FadeOutBlack();
      };
    }

    public void SetAllBlack() {
      IsAllBlack = true;
      m_Fader.m_FadeInBlackComplete.RemoveAllListeners();
      if (!m_MoviePlaying && IsMovieReady) {
        PlayMovie();
      }
    }

    public void SetMovieReady() {
      m_StartFadeInBlackSeconds = Mathf.Max((float) m_Movie.length - m_Fader.FadeTime, 0);
      IsMovieReady = true;
      if (!m_MoviePlaying && IsAllBlack) {
        PlayMovie();
      }
    }

    public void SkipMovie() {
      m_Fader.m_FadeInBlackComplete.AddListener(() => {
        m_Movie.time = m_Movie.length;
      });
      m_Fader.FadeInBlack();
    }

    public IEnumerator FadeInBlackWhenMovieAlmostEnd() {
      yield return new WaitUntil(() => m_Movie.isPlaying);
      while(m_Movie.time < m_StartFadeInBlackSeconds) {
        yield return new WaitForSeconds(1.0f);
      }
      m_Fader.FadeInBlack();
    }

    private void PlayMovie() {
      m_MoviePlaying = true;
      m_Fader.FadeOutBlack();
      m_Movie.Play();
      m_OnMovieStart?.Invoke();
    }
  }
}
