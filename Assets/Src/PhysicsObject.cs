using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
  public float m_MinGroundNormalY = .75f;
  public float m_GravityModifier = 1f;

  public float m_DebugRayLength = 1f;

  protected Vector2 m_TargetVelocity;
  protected bool m_Grounded;
  protected Vector2 m_GroundNormal;
  protected Rigidbody2D m_Rb2d;
  protected Vector2 m_Velocity;
  protected ContactFilter2D m_ContactFilter;
  protected RaycastHit2D[] m_HitBuffer = new RaycastHit2D[16];
  protected List<RaycastHit2D> m_ActualHitList = new List<RaycastHit2D>(16);

  protected const float m_MinMoveDistance = 0.0001f;
  protected const float m_ShellRadius = 0.01f;

  protected float m_SettedGravityModifier;

  private bool m_ForcedMove = false;

  void OnEnable() {
    m_Rb2d = GetComponent<Rigidbody2D>();
    m_SettedGravityModifier = m_GravityModifier;
  }

  void Start() {
    m_ContactFilter.useTriggers = false;
    m_ContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    m_ContactFilter.useLayerMask = true;
  }

  void Update() {
    m_TargetVelocity = Vector2.zero;
    if (ForcedMove) {
      ComputeForcedMoveVelocity();
    } else {
      ComputeVelocity();
    }
  }

  protected virtual void ComputeVelocity() {}
  protected virtual void ComputeForcedMoveVelocity() { }
  protected virtual void FallDistance(float distance) {}
  protected virtual void Landed() {}

  private bool m_PrevGrounded = true;

  void FixedUpdate() {
    m_Velocity += m_GravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
    m_Velocity.x = m_TargetVelocity.x;

    m_Grounded = false;

    var extraMoving = new List<ISceneMovable>();

    Vector2 deltaPosition = m_Velocity * Time.fixedDeltaTime;

    // Rotation of 90 degrees of ground normal, parallel to ground
    Vector2 moveAlongGround = new Vector2(m_GroundNormal.y, -m_GroundNormal.x);

    // Debug.DrawRay(gameObject.transform.position
    //               , new Vector3(moveAlongGround.x, moveAlongGround.y, 0)
    //               , Color.cyan);

    Vector2 move = moveAlongGround * deltaPosition.x;
    // Debug.DrawRay(gameObject.transform.position
    //               , new Vector3(move.x, move.y, 0) * m_DebugRayLength
    //               , Color.red);

    Movement(move, false, extraMoving);

    move = Vector2.up * deltaPosition.y;
    // Debug.DrawRay(gameObject.transform.position
    //               , new Vector3(move.x, move.y, 0) * m_DebugRayLength
    //               , Color.green);

    Movement(move, true, extraMoving);
    Vector2 externalVelocities = Vector2.zero;
    foreach (ISceneMovable movingObj in extraMoving) {
      externalVelocities += movingObj.Velocity;
    }
    m_Rb2d.position += externalVelocities * Time.fixedDeltaTime;

    if (m_Grounded && !m_PrevGrounded) {
      Landed();
    }
    m_PrevGrounded = m_Grounded;
  }

  void Movement(Vector2 move, bool yMovement, List<ISceneMovable> additionalObjects) {
    float distance = move.magnitude;

    if (distance > m_MinMoveDistance) {
      int count = m_Rb2d.Cast(move, m_ContactFilter, m_HitBuffer
                            , distance + m_ShellRadius);
      m_ActualHitList.Clear();

      // Figure out if object actually hit something
      for (int i = 0; i < count; i++) {
        Debug.DrawRay(m_HitBuffer[i].point, m_HitBuffer[i].normal);
        PlatformEffector2D effector = m_HitBuffer[i].collider
                                        .GetComponent<PlatformEffector2D>();
        if (effector && effector.useOneWay) {
          Vector2 hitNormal = m_HitBuffer[i].normal;
          // TODO: Why does checking move here works, but checking velocity fails?
          if (Vector2.Dot(move, hitNormal) > -0.000001
              || !EffectorHelpers.IsTopEffective(effector, hitNormal)) {
            continue; 
          }
        }
        m_ActualHitList.Add(m_HitBuffer[i]);
      }

      for (int i = 0; i < m_ActualHitList.Count; i++) {
        var hit = m_ActualHitList[i];

        bool isHitGround = false;
        Vector2 currentNormal = hit.normal;
        if (currentNormal.y > m_MinGroundNormalY) {
          m_Grounded = true;
          isHitGround = true;
          if (yMovement) {
            m_GroundNormal = currentNormal;
            currentNormal.x = 0;
          }
        }

        float projection = Vector2.Dot(m_Velocity, currentNormal);
        if ((isHitGround || currentNormal.y <= 0.01f) && projection < 0) {
          m_Velocity = m_Velocity - projection * currentNormal;
          Debug.DrawRay(gameObject.transform.position
              , new Vector3(m_Velocity.x, m_Velocity.y, 0)
              , Color.red);
        }

        ISceneMovable movable = hit.transform.GetComponent<ISceneMovable>() as ISceneMovable;
        if (movable != null) {
          additionalObjects.Add(movable);
        }

        float modifiedDistance = hit.distance - m_ShellRadius;
        distance = modifiedDistance < distance ? modifiedDistance : distance;
      }

      if (yMovement && !m_Grounded && Vector2.Dot(move, Vector2.down) > -0.000001)
      {
        FallDistance(distance);
      }
    }

    m_Rb2d.position = m_Rb2d.position + move.normalized * distance;
  }

  public bool ForcedMove {
    get { return m_ForcedMove; }
    set { m_ForcedMove = value; }
  }
}