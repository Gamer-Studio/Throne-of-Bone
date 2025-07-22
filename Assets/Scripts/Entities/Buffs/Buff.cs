using System;
using UnityEngine;

namespace ToB.Entities.Buffs
{
  [Serializable]
  public partial class Buff : ScriptableObject
  {
    /// <summary>
    /// 버프가 적용될 때 호출되는 이벤트입니다.
    /// </summary>
    public virtual void Apply(GameObject target, BuffInfo info) {}
    
    /// <summary>
    /// 버프가 해제될 때 호출되는 이벤트입니다.
    /// </summary>
    public virtual void Effect(GameObject target, BuffInfo info) {}
    
    /// <summary>
    /// 일정 딜레이마다 발생하는 버프의 효과입니다.
    /// </summary>
    public virtual void Remove(GameObject target) {}
  }
}