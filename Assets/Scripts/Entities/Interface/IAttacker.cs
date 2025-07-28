using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.Interface
{
  public interface IAttacker
  {
    /// <summary>
    /// 막을 수 있는 공격인지 판별합니다.
    /// </summary>
    bool Blockable { get; }
    
    /// <summary>
    /// 피격시 이펙트를 발생시키는지 여부입니다.
    /// </summary>
    bool Effectable { get; }
    
    /// <summary>
    /// 피해를 발생시키는 위치입니다.
    /// </summary>
    Vector3 Position { get; }
    
    /// <summary>
    /// 현재 엔티티의 팀입니다. <br/>
    /// 일반적으로 플레이어는 Player, 적은 Enemy, 버프는 None으로 설정되어있습니다. <br/>
    /// 서로 다른 팀에만 피해를 입힐 수 있습니다.
    /// </summary>
    Team Team { get; }
  }
}