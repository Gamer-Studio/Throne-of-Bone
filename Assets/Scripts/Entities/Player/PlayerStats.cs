using System;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  [Serializable]
  public class PlayerStats
  {
    /// <summary>
    /// 플레이어 체력이 변경될 때 호출됩니다. <br/>
    /// 매개변수로 현재 체력을 넘겨줍니다.
    /// </summary>
    public UnityEvent<float> onHpChanged = new();
    
    /// <summary>
    /// 플레이어의 방어 게이지가 변경될 떄 호출됩니다. <br/>
    /// 매개변수로 현재 방어 게이지를 넘겨줍니다.
    /// </summary>
    public UnityEvent<float> onBlockEnergyChanged = new();
    
    /// <summary>
    /// 플레이어가 어떤 이유로든 사망시 호출됩니다.
    /// </summary>
    public UnityEvent onDeath = new();
    
    [Label("최대 체력")] public float maxHp = 100;
    [Label("현재 체력"), SerializeField, GetSet(nameof(Hp))] private float currentHp = 100;
    [Label("최대 방어 게이지")] public float maxBlockEnergy = 100;
    [Label("현재 방어 게이지"), SerializeField, GetSet(nameof(BlockEnergy))] private float currentBlockEnergy = 0;

    /// <summary>
    /// 플레이어의 현재 방어력입니다. % 비율로 피해를 감소시킵니다.
    /// </summary>
    public Stat def;

    /// <summary>
    /// 플레이어의 임시 방어력입니다. \n
    /// 기본 방어력 스텟으로 취급되지않으며, 버프등에 활용할 수 있습니다.
    /// </summary>
    public float tempDef = 0;

    /// <summary>
    /// 플레이어의 현재 체력을 고정적으로 변경하거나 받아올 수 있습니다. 
    /// </summary>
    public float Hp
    {
      get => currentHp;
      set {
        currentHp = Math.Max(0, Math.Min(value, maxHp));
        onHpChanged?.Invoke(currentHp);
        if (currentHp <= 0)
        {
          onDeath?.Invoke();
        }
      }
    }

    /// <summary>
    /// 플레이어의 현재 방어 게이지를 변경하거나 가져올 수 있습니다.
    /// </summary>
    public float BlockEnergy
    {
      get => currentBlockEnergy;
      set {
        currentBlockEnergy = Math.Max(0, Math.Min(value, maxBlockEnergy));
        onBlockEnergyChanged?.Invoke(currentBlockEnergy);
      }
    }

    /// <summary>
    /// 플레이어의 공격력입니다.
    /// </summary>
    [Label("공격력")] public float atk = 10;

    public PlayerStats()
    {
      def = new Stat(0, v => v + tempDef);
    }

    /// <summary>
    /// 방어력을 반영해서 체력 피해를 줍니다.
    /// </summary>
    /// <param name="damage">줄 피해의 양입니다.</param>
    /// <param name="force">true일 시 고정 피해를 줍니다.</param>
    /// <returns>현재 체력을 반환합니다.</returns>
    public float Damage(float damage, bool force = false)
    {
      if(force)
        Hp -= damage;
      else
      {
        Hp -= damage * (1 - def.Value / 100);
      }
      
      return Hp;
    }
  }
}