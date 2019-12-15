using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
  public float minGroundNormalY = .65f;
  public float gravityModifier;

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

    Vector2 deltaPosition = velocity * Time.deltaTime;

    Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

    Vector2 move = moveAlongGround * deltaPosition.x;

    Movement(move, false);

    move = Vector2.up * deltaPosition.y;

    Movement(move, true);
  }

  void Movement(Vector2 move, bool yMovement) {
    float distance = move.magnitude;

    if (distance > minMoveDistance) {
      int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
      actualHitList.Clear();

      // Figure out if object actually hit something
      for (int i = 0; i < count; i++) {
        PlatformEffector2D effector = hitBuffer[i].collider.GetComponent<PlatformEffector2D>();
        if (effector && effector.useOneWay) {
          
        }
        actualHitList.Add(hitBuffer[i]);
      }

      for (int i = 0; i < actualHitList.Count; i++) {
        var hit = actualHitList[i];

        bool output = hit.transform.name == "LowerEdge";
        if (output) {
          Debug.Log("HIT");
        }

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
          if (output) {
            Debug.LogWarning(System.String.Format("Projection: {0}", projection));
          }
          velocity = velocity - projection * currentNormal;
        }

        float modifiedDistance = hit.distance - shellRadius;
        distance = modifiedDistance < distance ? modifiedDistance : distance;
      }
    }

    rb2d.position = rb2d.position + move.normalized * distance;
  }
}