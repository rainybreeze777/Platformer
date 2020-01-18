using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

  public float maxSpeed = 7;
  public float jumpTakeOffSpeed = 7;

  public EventManager m_EventManager;

  private bool isClimbing = false;
  private bool isDucking = false;

  private SpriteRenderer spriteRenderer;
  // private Animator animator;

  // Use this for initialization
  void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    // animator = GetComponent<Animator>();
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

    bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
    if (flipSprite) {
      spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // animator.SetBool("grounded", grounded);
    // animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

    targetVelocity = move * maxSpeed;
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

  bool IsDucking {
    get { return isDucking; }
  }
}