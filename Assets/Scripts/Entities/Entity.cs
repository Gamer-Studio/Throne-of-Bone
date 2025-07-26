using ToB.Entities.Interface;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities
{
  public class Entity : PooledObject, IAttacker
  {
    public EntityType type;
    public virtual bool Blockable => true;
    public virtual bool Effectable => true;
    public virtual Vector3 Position => transform.position;
    public Team Team => Team.Enemy;
  }

  public enum EntityType
  {
    Common,
    Elite,
    Boss,
    Npc
  }
}