using UnityEngine;

public class VectorMath {
  public static Vector2 RotateV2Radian(Vector2 v, float radians) {
    float _x = v.x * Mathf.Cos(radians) - v.y * Mathf.Sin(radians);
    float _y = v.x * Mathf.Sin(radians) + v.y * Mathf.Cos(radians);
    return new Vector2(_x, _y);
  }

  public static Vector2 RotateV2Degree(Vector2 v, float degrees) {
    Vector3 afterRotate = Quaternion.AngleAxis(degrees, Vector3.forward)
                          * (new Vector3(v.x, v.y));
    return new Vector2(afterRotate.x, afterRotate.y);
  }
}