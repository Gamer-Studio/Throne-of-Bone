using ToB.Entities.Interface;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities
{
  public class Entity : PooledObject, IAttacker
  {
    /// <summary>
    /// 엔티티의 리젠 타입입니다.
    /// </summary>
    public EntityType type;
    
    /// <summary>
    /// 엔티티의 공격이 막을 수 있는지 여부입니다.
    /// </summary>
    public virtual bool Blockable => true;
    
    /// <summary>
    /// 해당 엔티티가 공격시점에 이펙트를 발생시키는지 여부입니다.
    /// </summary>
    public virtual bool Effectable => true;

    /// <summary>
    /// 엔티티의 공격시점 위치입니다.
    /// </summary>
    public virtual Vector3 Position => transform.position;
    
    /// <summary>
    /// 엔티티의 팀입니다.
    /// </summary>
    public Team Team => Team.Enemy;
  }

  public enum EntityType
  {
    /// <summary>
    /// 방 로딩시 소환되는 몬스터입니다.
    /// </summary>
    Common,
    /// <summary>
    /// 저장하면 다시 소환되는 몬스터입니다.
    /// </summary>
    Elite,
    /// <summary>
    /// 한 번 처치되면 다시 소환되지 않는 엔티티 타입입니다.
    /// </summary>
    Boss,
    /// <summary>
    /// 임시
    /// </summary>
    Npc
  }
}