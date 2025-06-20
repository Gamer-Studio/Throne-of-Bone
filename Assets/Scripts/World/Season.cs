using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace ToB.Worlds
{
  [Serializable]
  public struct Season : IEquatable<Season>
  {
    /// <summary>
    /// 현재 선언된 계절의 목록입니다.
    /// </summary>
    public static readonly List<Season> ActiveSeasons = new();
    private static int nextId;

    #region Fields

    [SerializeField] private Color color;
    [SerializeField, ReadOnly] private string name;
    [SerializeField, ReadOnly] private int id;
    
    /// <summary>
    /// 계절의 색상을 반환합니다.
    /// </summary>
    public Color Color => color;
    /// <summary>
    /// 계절의 이름을 반환합니다.
    /// </summary>
    public string Name => name;
    /// <summary>
    /// 계절의 고유 식별자를 반환합니다.
    /// </summary>
    public int Id => id;

    #endregion
    
    private Season(string name, Color color)
    {
      this.name = name;

      var index = ActiveSeasons.FindIndex(team => team.name == name);

      if (index == -1)
      {
        id = nextId++;
        this.color = color;
        ActiveSeasons.Add(this);
      }
      else
      {
        id = index;
        this.color = ActiveSeasons[index].color;
      }
    }

    /// <summary>
    /// 지정한 이름에 해당하는 Season 인스턴스를 반환합니다.
    /// 등록되지 않은 이름일 경우 None을 반환합니다.
    /// </summary>
    /// <param name="name">찾고자 하는 계절의 이름</param>
    /// <returns>해당 이름을 가진 Season 인스턴스</returns>
    public static Season Get(string name)
    {
      var team = None;

      foreach (var activeTeam in ActiveSeasons.Where(activeTeam => activeTeam.name == name))
      {
        team = activeTeam;
        break;
      }
      
      return team;
    }
    
    #region Operators

    /// <summary>
    /// 두 Season이 같은 ID를 가지는지 비교합니다.
    /// </summary>
    public static bool operator ==(Season left, Season right) => left.id == right.id;

    /// <summary>
    /// 두 Season이 다른 ID를 가지는지 비교합니다.
    /// </summary>
    public static bool operator !=(Season left, Season right) => !(left == right);

    /// <summary>
    /// 다른 Season과 현재 Season이 동일한지 확인합니다.
    /// </summary>
    public bool Equals(Season other) => id == other.id;

    /// <summary>
    /// 다른 오브젝트가 현재 Season과 동일한지 확인합니다.
    /// </summary>
    public override bool Equals(object obj) => obj is Season other && Equals(other);

    /// <summary>
    /// 현재 Season의 해시 코드를 반환합니다.
    /// </summary>
    public override int GetHashCode() => id;

    /// <summary>
    /// 문자열로부터 Season을 암시적으로 가져옵니다. (예: Season s = "Spring";)
    /// </summary>
    /// <param name="name">Season 이름 문자열</param>
    /// <returns>해당 이름을 가진 Season 인스턴스</returns>
    public static implicit operator Season(string name) => Get(name);
    
    #endregion

    #region Preload

    // 정의된 계절 목록입니다.
    public static readonly Season None, Spring, Summer, Autumn, Winter;

    static Season()
    {
      None = new Season(nameof(None), Color.white);
      Spring = new Season(nameof(Spring), Color.blue);
      Summer = new Season(nameof(Summer), Color.red);
      Autumn = new Season(nameof(Autumn), Color.yellow);
      Winter = new Season(nameof(Winter), Color.cyan);
    }

    #endregion
  }
}