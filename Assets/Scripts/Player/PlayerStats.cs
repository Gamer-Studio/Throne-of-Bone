using System;
using UnityEngine.Events;

namespace ToB.Player
{
  [Serializable]
  public class PlayerStats
  {
    public UnityEvent<int> onHpChanged = new();
    public UnityEvent onDeath = new();
    
    // 최대 체력입니다.
    public int maxHp = 100;
    
    // 현재 체력입니다. 변경시 가능한 Hp 필드를 이용해주세요.
    public int currentHp = 100;

    public int Hp
    {
      get => currentHp;
      set {
        currentHp = Math.Max(0, Math.Min(value, maxHp));
        onHpChanged?.Invoke(currentHp);
        if (currentHp <= 0) onDeath?.Invoke();
      }
    }

    public PlayerStats()
    {
    }
    
    public int Damage(int damage)
    {
      currentHp -= Math.Min(Math.Max(damage, 0), currentHp);
      onHpChanged?.Invoke(currentHp);
      return currentHp;
    }
  }
}