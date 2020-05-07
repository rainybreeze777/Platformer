using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Hide Unity Input class as we want to have more convenient control
using Input = Platformer.Input;

public class PlayerPlatformerController : PhysicsObject {

  public float m_MaxSpeed = 7;
  public float m_JumpTakeOffSpeed = 7;

  public float m_FallDeathThreshold = 10f;
  public float m_DropItemDistance = 1.0f;
  // How much % of player char height can the item be raised to drop onto
  // the ground
  [Range(0.0f, 100.0f)]
  public float m_DropItemMaxRaisePercentage = 20.0f;

  public bool m_SpawnAtSpawnPoint = true;

  public float m_ThrowAngle = 0.0f; // In Degrees
  public float m_ThrowForce = 1.0f;

  private EventManager m_EventManager;
  private PlayerManager m_PlayerManager;

  private bool m_IsClimbing = false;
  private bool m_IsDucking = false;
  private float m_FallDist = 0;
  private int m_OriginalSortingOrder;
  private Vector3 m_Facing;
  private Vector3 m_ScriptedTargetPos;

  private SpriteRenderer spriteRenderer;
  private Animator m_Animator;

  private Action m_OnForceMoveArrive;

  // Use this for initialization
  void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    m_Animator = GetComponent<Animator>();
    // Must find across-scene shared objects instead of dragging them in
    // in inspector, otherwise upon scene reload, the references go missing
    m_EventManager = GameObject.Find("/EventManager")
                               .GetComponent<EventManager>() as EventManager;
    m_PlayerManager = GameObject.Find("/PlayerManager")
                                .GetComponent<PlayerManager>() as PlayerManager;
    m_Facing = new Vector3(1, 0);
    m_OriginalSortingOrder = spriteRenderer.sortingOrder;
  }

  public void FlipHiddenPassageLayer(bool toHidden, int targetSortingLayer = -100) {
    int layer = toHidden
                ? LayerMask.NameToLayer("PlayerInHiddenPassage")
                : LayerMask.NameToLayer("Player");
    int sortingOrder = toHidden ? targetSortingLayer : m_OriginalSortingOrder;
    gameObject.layer = layer;
    m_ContactFilter.useTriggers = false;
    m_ContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(layer));
    m_ContactFilter.useLayerMask = true;
    spriteRenderer.sortingOrder = sortingOrder;
  }

  public void ScriptedMoveToPoint(Vector3 pos, Action onForceMoveArrive) {
    ForcedMove = true;
    m_ScriptedTargetPos = pos;
    m_OnForceMoveArrive = onForceMoveArrive;
    Vector3 rb2dPos = m_Rb2d.position;
    Vector3 moveDirection = pos - rb2dPos;
    moveDirection.y = moveDirection.z = 0;
    m_Facing = moveDirection.normalized;
  }

  protected override void ComputeVelocity() {
    Vector2 move = Vector2.zero;

    move.x = Input.GetAxis("Horizontal");

    if (Input.GetButton("Vertical") && m_IsClimbing) {
      m_Velocity.y = Input.GetAxis("Vertical") * m_MaxSpeed;
    } else if (Input.GetButtonUp("Vertical") && m_IsClimbing) {
      m_Velocity = Vector2.zero;
    } else if (Input.GetButtonDown("Jump") && m_Grounded) {
      if (!m_IsDucking) {
        m_Velocity.y = m_JumpTakeOffSpeed;
      }
      m_EventManager.Invoke<JumpUEvent>();
    } else if (Input.GetButtonUp("Jump")) {
      if (m_Velocity.y > 0) {
        m_Velocity.y = m_Velocity.y * 0.5f;
      }
      m_EventManager.Invoke<JumpReleaseUEvent>();
    }

    if (Input.GetButton("Duck") && m_Grounded) {
      if (!m_IsDucking) {
        m_EventManager.Invoke<DuckingUEvent, bool>(true);
      }
      m_IsDucking = true;
    } else if (m_Grounded && m_IsDucking) {
      m_EventManager.Invoke<DuckingUEvent, bool>(false);
      m_IsDucking = false;
    }

    if (move.x > 0.01f) { m_Facing = new Vector3(1, 0, 0); }
    else if (move.x < -0.01f) { m_Facing = new Vector3(-1, 0, 0); }

    bool flipSprite = (spriteRenderer.flipX 
                        ? (move.x > 0.01f) : (move.x < -0.01f));
    if (flipSprite) {
      spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // animator.SetBool("grounded", m_Grounded);
    // animator.SetFloat("velocityX", Mathf.Abs(m_Velocity.x) / m_MaxSpeed);

    m_TargetVelocity = move * m_MaxSpeed;
    Debug.DrawRay(gameObject.transform.position
                  , ThrowVector * m_DebugRayLength
                  , Color.white);
  }

  protected override void ComputeForcedMoveVelocity() {
    Vector2 move = Vector2.zero;
    Vector3 rb2dPos = m_Rb2d.position;
    if (Vector3.Dot((rb2dPos - m_ScriptedTargetPos), m_Facing) > 0) {
      m_TargetVelocity = Vector2.zero;
      ForcedMove = false;
      m_ScriptedTargetPos = Vector3.zero;
      m_OnForceMoveArrive?.Invoke();
      m_OnForceMoveArrive = null;
    } else {
      move.x = m_ScriptedTargetPos.x > m_Rb2d.position.x ? 1 : -1;
      m_TargetVelocity = move * m_MaxSpeed;
    }
  }

  protected override void FallDistance(float distance) {
    m_FallDist += distance;
  }

  protected override void Landed() {
    if (m_FallDist > m_FallDeathThreshold) {
      m_EventManager.Invoke<AboutToDieUEvent>();
      m_Animator.SetTrigger("Die");
    }
    m_FallDist = 0; 
  }

  void OnTriggerEnter2D(Collider2D c) {
      if (c.transform.tag == "Climbable" && Input.GetButton("Jump")) {
        m_IsClimbing = true;
        m_Rb2d.gravityScale = 0;
        m_GravityModifier = 0;
      }
  }

  void OnTriggerExit2D(Collider2D c) {
    if (c.transform.tag == "Climbable") {
      m_IsClimbing = false;
      m_Rb2d.gravityScale = 1;
      m_GravityModifier = m_SettedGravityModifier;
    }
  }

  void OnDeathAnimationFinish() {
    m_EventManager.Invoke<DeadUEvent>();
  }

  bool IsDucking {
    get { return m_IsDucking; }
  }

  public bool SpawnAtSpawnPoint { get { return m_SpawnAtSpawnPoint; } }

  public Vector3 ThrowVector { 
    get { 
      return Quaternion.Euler(0, 0, m_Facing.x * m_ThrowAngle) 
                * m_Facing 
                * m_ThrowForce;
    }
  }
}