using System;
using UnityEngine;

namespace ToB.Utils
{
  public delegate T Operator<T>(T origin);
  public static class MathUtil
  {
    #region LookAt2D

    public static void LookAt2D(this Transform transform, Transform target, Vector2 direction)
      => transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(direction.normalized, (target.position - transform.position).normalized));

    public static void LookAt2D(this Transform transform, Transform target) => LookAt2D(transform, target, Vector2.up);
    
    #endregion
    
    #region Vector3
    
    public static Vector3 X(this Vector3 origin, float x) => new (x, origin.y, origin.z);
    public static Vector3 Y(this Vector3 origin, float y) => new (origin.x, y, origin.z);
    public static Vector3 Z(this Vector3 origin, float z) => new (origin.x, origin.y, z);
    
    public static Vector3 X(this Vector3 origin, Operator<float> x) => new (x(origin.x), origin.y, origin.z);
    public static Vector3 Y(this Vector3 origin, Operator<float> y) => new (origin.x, y(origin.y), origin.z);
    public static Vector3 Z(this Vector3 origin, Operator<float> z) => new (origin.x, origin.y, z(origin.z));
    
    public static Vector3 TransformPoint(this Vector3 origin, Transform transform) => transform.TransformPoint(origin);
    
    #endregion
    
    #region Vector2
    
    public static Vector2 X(this Vector2 origin, float x) => new (x, origin.y);
    public static Vector2 Y(this Vector2 origin, float y) => new (origin.x, y);
    
    public static Vector2 X(this Vector2 origin, Operator<float> x) => new (x(origin.x), origin.y);
    public static Vector2 Y(this Vector2 origin, Operator<float> y) => new (origin.x, y(origin.y));
    
    public static float ToAngle(this Vector2 direction)
    {
      var angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      
      return (angle < 0 ? angle + 360 : angle) - 90;
    }
    
    public static Vector2 TransformPoint(this Vector2 origin, Transform transform) => transform.TransformPoint(origin);

    public static Quaternion GetDirection(this Vector2 origin, Vector2 target)
    {
      var dir = target - origin;
      var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      return Quaternion.Euler(0, 0, angle);
    }

    public static Quaternion ToQuaternion(this Vector2 origin) => Quaternion.Euler(0, 0, Mathf.Atan2(origin.y, origin.x) * Mathf.Rad2Deg);
    
    #endregion
    
    #region Float

    public static Vector2 ToDirection(this float angle)
    {
      var rad = (angle + 90f) * Mathf.Deg2Rad;
      return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
    
    #endregion
    
    #region Quaternion
    
    public static Quaternion X(this Quaternion origin, float x) => Quaternion.Euler(x, origin.y, origin.z);
    public static Quaternion Y(this Quaternion origin, float y) => Quaternion.Euler(origin.x, y, origin.z);
    public static Quaternion Z(this Quaternion origin, float z) => Quaternion.Euler(origin.x, origin.y, z);
    
    public static Quaternion AddX(this Quaternion origin, float value) => Quaternion.Euler(origin.x + value, origin.y, origin.z);
    public static Quaternion AddY(this Quaternion origin, float value) => Quaternion.Euler(origin.x, origin.y + value, origin.z);
    public static Quaternion AddZ(this Quaternion origin, float value) => Quaternion.Euler(origin.x, origin.y, origin.z + value);
    
    public static Quaternion X(this Quaternion origin, Operator<float> x) => Quaternion.Euler(x(origin.x), origin.y, origin.z);
    public static Quaternion Y(this Quaternion origin, Operator<float> y) => Quaternion.Euler(origin.x, y(origin.y), origin.z);
    public static Quaternion Z(this Quaternion origin, Operator<float> z) => Quaternion.Euler(origin.x, origin.y, z(origin.z));

    public static Vector2 ToVector2Direction(this Quaternion origin)
    {
      var angle = origin.eulerAngles.z * Mathf.Deg2Rad;
      return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    
    #endregion
  }
}