using System;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities.Buffs
{
  [Serializable]
  public class Buff
  {
    [ReadOnly] public string name;
    private readonly Action<GameObject, BuffInfo> onApply, onEffect;
    private readonly Action<GameObject> onRemove;

    /// <summary>
    /// 버프를 구현합니다.
    /// 각 이벤트는 필요없으면 Null를 넣어줘도 됩니다.
    /// </summary>
    /// <param name="name">버프의 이름입니다.</param>
    /// <param name="onApply">버프가 적용될 때 호출되는 이벤트입니다.</param>
    /// <param name="onRemove">버프가 해제될 때 호출되는 이벤트입니다.</param>
    /// <param name="onEffect">일정 딜레이마다 발생하는 버프의 효과입니다.</param>
    public Buff(string name, Action<GameObject, BuffInfo> onApply, Action<GameObject> onRemove, Action<GameObject, BuffInfo> onEffect)
    {
      this.name = name;
      this.onApply = onApply;
      this.onRemove = onRemove;
      this.onEffect = onEffect;
    }
    
    /// <summary>
    /// 이벤트 트리거용 메소드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Apply(GameObject target, BuffInfo info) => onApply?.Invoke(target, info);
    /// <summary>
    /// 이벤트 트리거용 메소드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Effect(GameObject target, BuffInfo info) => onEffect?.Invoke(target, info);
    /// <summary>
    /// 이벤트 트리거용 메소드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Remove(GameObject target) => onRemove?.Invoke(target);
    
    /// <summary>
    /// 독 디버프입니다. 디버프 레벨만큼 고정 피해를 줍니다.
    /// </summary>
    public static readonly Buff Poison = new Buff("Poison", null, null, (target, info) => target.Damage(info.level));
    
  }
}