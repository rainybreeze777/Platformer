using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
  public float minGroundNormalY = .65f;
  public float gravityModifier;

  public float debugRayLength = 1f;

  protected Vector2 targetVelocity;
  protected bool grounded;
  protected Vector2 groundNormal;
  protected Rigidbody2D rb2d;
  protected Vector2 velocity;
  protected ContactFilter2D contactFilter;
  protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
  protected List<RaycastHit2D> actualHitList = new List<RaycastHit2D>(16);

  protected const float minMoveDistance = 0.001f;
  protected const float shellRadius = 0.01f;

  protected float settedGravityModifier;

  void OnEnable() {
    rb2d = GetComponent<Rigidbody2D>();
    settedGravityModifier = gravityModifier;
  }

  void Start() {
    contactFilter.useTriggers = false;
    contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    contactFilter.useLayerMask = true;
  }

  void Update() {
    targetVelocity = Vector2.zero;
    ComputeVelocity();
  }

  protected virtual void ComputeVelocity() {}

  void FixedUpdate() {
    velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
    velocity.x = targetVelocity.x;

    grounded = false;

    var extraMoving = new List<SelfMovableObject>();

    Vector2 deltaPosition = velocity * Time.deltaTime;

    Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

    Debug.DrawRay(gameObject.transform.position
                  , new Vector3(moveAlongGround.x, moveAlongGround.y, 0)
                  , Color.cyan);

    Vector2 move = moveAlongGround * deltaPosition.x;
    Debug.DrawRay(gameObject.transform.position
                  , new Vector3(move.x, move.y, 0) * debugRayLength
                  , Color.red);

    Movement(move, false, extraMoving);

    move = Vector2.up * deltaPosition.y;
    Debug.DrawRay(gameObject.transform.position
                  , new Vector3(move.x, move.y, 0) * debugRayLength
                  , Color.green);

    Movement(move, true, extraMoving);
    Vector2 externalVelocities = Vector2.zero;
    foreach (SelfMovableObject movingObj in extraMoving) {
      externalVelocities += movingObj.Velocity;
    }
    rb2d.position += externalVelocities * Time.deltaTime;
  }

  void Movement(Vector2 move, bool yMovement, List<SelfMovableObject> additionalObjects) {
    float distance = move.magnitude;

    if (distance > minMoveDistance) {
      int count = rb2d.Cast(move, contactFilter, hitBuffer
                            , distance + shellRadius);
      actualHitList.Clear();

      // Figure out if object actually hit something
      for (int i = 0; i < count; i++) {
        Debug.DrawRay(hitBuffer[i].point, hitBuffer[i].normal);
        PlatformEffector2D effector = hitBuffer[i].collider
                                        .GetComponent<PlatformEffector2D>();
        if (effector && effector.useOneWay) {
          Vector2 hitNormal = hitBuffer[i].normal;
          // TODO: Why does checking move here works, but checking velocity fails?
          if (Vector2.Dot(move, hitNormal) > -0.000001
              || !EffectorHelpers.IsTopEffective(effector, hitNormal)) {
            Debug.LogWarning("PlatformEffector2D is skipping!");
            continue; 
          }
        }
        actualHitList.Add(hitBuffer[i]);
      }

      for (int i = 0; i < actualHitList.Count; i++) {
        var hit = actualHitList[i];

        Vector2 currentNormal = hit.normal;
        if (currentNormal.y > minGroundNormalY) {
          grounded = true;
          if (yMovement) {
            groundNormal = currentNormal;
            currentNormal.x = 0;
          }
        }

        float projection = Vector2.Dot(velocity, currentNormal);
        if (projection < 0) {
          velocity = velocity - projection * currentNormal;
        }
        if (hit.transform.tag == "SelfMovable") {
          additionalObjects.Add(hit.transform.GetComponent<SelfMovableObject>());
        }

        float modifiedDistance = hit.distance - shellRadius;
        distance = modifiedDistance < distance ? modifiedDistance : distance;
      }
    }

    rb2d.position = rb2d.position + move.normalized * distance;
  }
}