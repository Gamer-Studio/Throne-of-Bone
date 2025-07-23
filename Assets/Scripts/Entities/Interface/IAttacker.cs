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
  }
}