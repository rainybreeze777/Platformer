using UnityEngine;

public class OneWayPlatform : MonoBehaviour {

  public EventManager m_EventManager;

  private bool m_IsDucking = false;
  private PlatformEffector2D m_Effector;
  private float m_OriginalRotation;

  void Start() {
    m_EventManager.AddListener<DuckingUEvent, bool>(OnPlayerDucking);
    m_EventManager.AddListener<JumpUEvent>(OnPlayerJump);
    m_EventManager.AddListener<JumpReleaseUEvent>(OnPlayerJumpRelease);
    m_Effector = GetComponent<PlatformEffector2D>();
    m_OriginalRotation = m_Effector.rotationalOffset;
  }

  public void OnPlayerDucking(bool isDucking) {
    Debug.Log("Player is ducking: " + isDucking);
    m_IsDucking = isDucking;
  }

  public void OnPlayerJump() {
    if (m_IsDucking) {
      m_Effector.rotationalOffset += 180f;
    }
  }

  public void OnPlayerJumpRelease() {
    m_Effector.rotationalOffset = m_OriginalRotation;
  }
}