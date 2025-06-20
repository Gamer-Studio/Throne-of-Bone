using NaughtyAttributes;
using ToB.Utils.Singletons;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Scenes.Stage
{
  public class StageManager : Singleton<StageManager>
  {
    [SerializeField, ReadOnly] protected GameObject stage;
    
    public void LoadStage(string stageName, Season season)
    {
      var stageObj = new AssetReference("").LoadAssetAsync<GameObject>().WaitForCompletion();
    }
  }
}