using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities.Buffs
{
  [Serializable]
  public struct Buff : IEquatable<Buff>
  {
    /// <summary>
    /// 현재 선언된 Buff의 목록입니다.
    /// </summary>
    public static readonly List<Buff> ActiveBuffs = new();
    private static int nextId;

    #region Fields

    [SerializeField] private Color color;
    [SerializeField, ReadOnly] private string name;
    [SerializeField, ReadOnly] private int id;
    
    /// <summary>
    /// Buff의 색상을 반환합니다.
    /// </summary>
    public Color Color => color;
    /// <summary>
    /// Buff의 이름을 반환합니다.
    /// </summary>
    public string Name => name;
    /// <summary>
    /// Buff의 고유 식별자를 반환합니다.
    /// </summary>
    public int Id => id;

    #endregion
    
    private Buff(string name, Color color)
    {
      this.name = name;

      var index = ActiveBuffs.FindIndex(team => team.name == name);

      if (index == -1)
      {
        id = nextId++;
        this.color = color;
        ActiveBuffs.Add(this);
      }
      else
      {
        id = index;
        this.color = ActiveBuffs[index].color;
      }
    }

    /// <summary>
    /// 지정한 이름에 해당하는 Buff 인스턴스를 반환합니다.
    /// 등록되지 않은 이름일 경우 None을 반환합니다.
    /// </summary>
    /// <param name="name">찾고자 하는 계절의 이름</param>
    /// <returns>해당 이름을 가진 Buff 인스턴스</returns>
    public static Buff Get(string name)
    {
      var buff = None;

      foreach (var activeBuff in ActiveBuffs.Where(target => target.name == name))
      {
        buff = activeBuff;
        break;
      }
      
      return buff;
    }
    
    #region Operators

    /// <summary>
    /// 두 Buff이 같은 ID를 가지는지 비교합니다.
    /// </summary>
    public static bool operator ==(Buff left, Buff right) => left.id == right.id;

    /// <summary>
    /// 두 Buff이 다른 ID를 가지는지 비교합니다.
    /// </summary>
    public static bool operator !=(Buff left, Buff right) => !(left == right);

    /// <summary>
    /// 다른 Buff과 현재 Buff이 동일한지 확인합니다.
    /// </summary>
    public bool Equals(Buff other) => id == other.id;

    /// <summary>
    /// 다른 오브젝트가 현재 Buff과 동일한지 확인합니다.
    /// </summary>
    public override bool Equals(object obj) => obj is Buff other && Equals(other);

    /// <summary>
    /// 현재 Buff의 해시 코드를 반환합니다.
    /// </summary>
    public override int GetHashCode() => id;

    /// <summary>
    /// 문자열로부터 Buff를 암시적으로 가져옵니다. (예: Buff poison = "Poison";)
    /// </summary>
    /// <param name="name">Buff 이름 문자열</param>
    /// <returns>해당 이름을 가진 Buff 인스턴스</returns>
    public static implicit operator Buff(string name) => Get(name);
    
    #endregion

    #region Preload

    // 정의된 계절 목록입니다.
    public static readonly Buff None, Spring, Summer, Autumn, Winter;

    static Buff()
    {
      None = new Buff(nameof(None), Color.white);
      Spring = new Buff(nameof(Spring), Color.blue);
      Summer = new Buff(nameof(Summer), Color.red);
      Autumn = new Buff(nameof(Autumn), Color.yellow);
      Winter = new Buff(nameof(Winter), Color.cyan);
    }

    #endregion
  }
}