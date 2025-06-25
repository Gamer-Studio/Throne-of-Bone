using NaughtyAttributes;
using ToB.Player;
using ToB.Utils.Singletons;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Scenes.Stage
{
  public class StageManager : Singleton<StageManager>
  {
    [Tooltip("현재 활성화된 Player 태그가 붙은 플레이어 캐릭터입니다.")] public PlayerCharacter player;
    [SerializeField, ReadOnly] protected GameObject stage;
    
    public void LoadStage(string stageName, Season season)
    {
      var stageObj = new AssetReference("").LoadAssetAsync<GameObject>().WaitForCompletion();
    }

    #region Unity Event
    
#if UNITY_EDITOR

    private void Reset()
    {
      player = PlayerCharacter.GetInstance();
    }
    
#endif

    private void Awake()
    {
      if (!player)
        player = PlayerCharacter.GetInstance();
      
    }

    #endregion
  }
}