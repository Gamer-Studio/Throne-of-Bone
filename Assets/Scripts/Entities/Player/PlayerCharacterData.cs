using NaughtyAttributes;
using ToB.IO;
using UnityEngine;

namespace ToB.Player
{
  public partial class PlayerCharacter : ISaveable
  {
    [Foldout("SAVE"), SerializeField] private SAVEModule data;
    
    /// <summary>
    /// 캐릭터 로딩시 데이터 불러오기
    /// </summary>
    public void Load()
    {
      data = SAVE.Current.Player.Node("character", true);
    }

    /// <summary>
    /// 저장시 캐릭터 데이터 json으로 변환하기
    /// </summary>
    public void Save()
    {
      
    }
  }
}