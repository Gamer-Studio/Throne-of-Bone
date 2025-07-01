using NaughtyAttributes;
using ToB.Player;
using ToB.Utils.Singletons;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Scenes.Stage
{
  public class RoomManager : Singleton<RoomManager>
  {
    [Tooltip("현재 활성화된 Player 태그가 붙은 플레이어 캐릭터입니다.")] public PlayerCharacter player;
    [Tooltip("현재 스테이지 인덱스입니다.")] [field: SerializeField, ReadOnly] public int CurrentStageIndex { get; private set; } = 1;
    [Tooltip("현재 방의 인덱스입니다.")] [field: SerializeField, ReadOnly] public int CurrentRoomIndex { get; private set; } = 1;

    #region Unity Event
    
#if UNITY_EDITOR

    private void Reset()
    {
      player = PlayerCharacter.GetInstance();
    }
    
#endif

    private void Awake()
    {
      if (!player) player = PlayerCharacter.GetInstance();
    }

    #endregion
    
    #region Feature
    
    public void LoadRoom(string roomName, Season season)
    {
      var stageObj = new AssetReference("").LoadAssetAsync<GameObject>().WaitForCompletion();
    }
    
    #endregion
  }
}