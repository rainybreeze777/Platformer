using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Hide Unity Input class as we want to have more convenient control
using Input = Platformer.Input;

public class PlayerPlatformerController : PhysicsObject {

  public float maxSpeed = 7;
  public float jumpTakeOffSpeed = 7;

  public float m_FallDeathThreshold = 10f;
  public float m_DropItemDistance = 1.0f;
  // How much % of player char height can the item be raised to drop onto
  // the ground
  [Range(0.0f, 100.0f)]
  public float m_DropItemMaxRaisePercentage = 20.0f;

  private EventManager m_EventManager;
  private PlayerManager m_PlayerManager;

  public bool m_SpawnAtSpawnPoint = true;

  private bool isClimbing = false;
  private bool isDucking = false;
  private float m_FallDist = 0;

  private SpriteRenderer spriteRenderer;
  private Animator m_Animator;

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
  }

  protected override void ComputeVelocity() {
    Vector2 move = Vector2.zero;

    move.x = Input.GetAxis("Horizontal");

    if (Input.GetButton("Vertical") && isClimbing) {
      velocity.y = Input.GetAxis("Vertical") * maxSpeed;
    } else if (Input.GetButtonUp("Vertical") && isClimbing) {
      velocity = Vector2.zero;
    } else if (Input.GetButtonDown("Jump") && grounded) {
      if (!isDucking) {
        velocity.y = jumpTakeOffSpeed;
      }
      m_EventManager.Invoke<JumpUEvent>();
    } else if (Input.GetButtonUp("Jump")) {
      if (velocity.y > 0) {
        velocity.y = velocity.y * 0.5f;
      }
      m_EventManager.Invoke<JumpReleaseUEvent>();
    }

    if (Input.GetButton("Duck") && grounded) {
      if (!isDucking) {
        m_EventManager.Invoke<DuckingUEvent, bool>(true);
      }
      isDucking = true;
    } else if (grounded && isDucking) {
      m_EventManager.Invoke<DuckingUEvent, bool>(false);
      isDucking = false;
    }

    bool flipSprite = (spriteRenderer.flipX 
                        ? (move.x > 0.01f) : (move.x < 0.01f));
    if (flipSprite) {
      spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // animator.SetBool("grounded", grounded);
    // animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

    targetVelocity = move * maxSpeed;
  }

  public bool CanDropItem(Collider2D itemCollider, out Vector3 dropPosition) {
    dropPosition = Vector3.zero;
    var playerCollider = GetComponent<Collider2D>() as Collider2D;
    // TODO: Left and Right dropping
    float playerReach = playerCollider.bounds.max.x;
    Vector3 potentialDropPoint = 
      new Vector3(playerReach 
                    + m_DropItemDistance 
                    + itemCollider.bounds.extents.x
                  , transform.position.y, 0);

    itemCollider.transform.position = potentialDropPoint;

    itemCollider.isTrigger = false;

    ContactFilter2D filter = new ContactFilter2D();
    filter.useTriggers = false;
    filter.SetLayerMask(
      Physics2D.GetLayerCollisionMask(itemCollider.gameObject.layer));
    filter.useLayerMask = true;
    List<Collider2D> collidings = new List<Collider2D>();

    if (itemCollider.OverlapCollider(filter, collidings) > 0) {
      
      Vector2 vecToShift = Vector2.zero;

      foreach (var collider in collidings) {
        ColliderDistance2D dist = itemCollider.Distance(collider);
        vecToShift += dist.normal * dist.distance;
      }
      if (Mathf.Abs(vecToShift.x) >= m_DropItemDistance) {
        // Not enough space to place the item
        return false;
      } else if (vecToShift.y < 0) {
        // No scenario should the item be colliding with ceiling
        return false;
      }
      potentialDropPoint += new Vector3(vecToShift.x, vecToShift.y, 0);

      if ((potentialDropPoint.y - itemCollider.bounds.extents.y) 
            - playerCollider.bounds.min.y 
          > playerCollider.bounds.size.y * m_DropItemMaxRaisePercentage / 100.0f) {
        // The item needs to be raised too high to be dropped
        return false;
      }
    }

    dropPosition = potentialDropPoint;
    itemCollider.isTrigger = true;
    return true;
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
          isClimbing = true;
          rb2d.gravityScale = 0;
          gravityModifier = 0;
      }
  }

  void OnTriggerExit2D(Collider2D c) {
    if (c.transform.tag == "Climbable") {
      isClimbing = false;
      rb2d.gravityScale = 1;
      gravityModifier = settedGravityModifier;
    }
  }

  void OnDeathAnimationFinish() {
    m_EventManager.Invoke<DeadUEvent>();
  }

  bool IsDucking {
    get { return isDucking; }
  }

  public bool SpawnAtSpawnPoint { get { return m_SpawnAtSpawnPoint; } }
}