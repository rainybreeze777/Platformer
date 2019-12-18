using UnityEngine;

public class EffectorHelpers {
  public static bool IsTopEffective(PlatformEffector2D effector, Vector2 v) {
    return Vector2.Angle(VectorMath.RotateV2Degree(Vector2.up
                                                   , effector.rotationalOffset)
                         , v) 
            < (effector.surfaceArc / 2);
  }
}