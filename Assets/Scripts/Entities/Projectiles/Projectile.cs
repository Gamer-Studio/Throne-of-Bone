using NaughtyAttributes;
using ToB.Entities.Interface;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.Projectiles
{
  public class Projectile : PooledObject, IAttacker
  {
    private const string Label = "Projectile";
    [Label("발사 방향")] public Vector2 direction;
    public GameObject launcher;

    public virtual bool Blockable => true;
    public virtual bool Effectable => true;
    public virtual Vector3 Position => transform.position;

    public static Projectile Shoot(string name, Vector3 startPosition, Vector2 direction)
    {
      var projectile = (Projectile)PoolingHelper.Pooling(Label + name);
      projectile.transform.position = startPosition;
      projectile.direction = direction;
      
      return projectile;
    }
  }
}