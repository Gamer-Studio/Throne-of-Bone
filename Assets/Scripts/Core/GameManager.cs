using ToB.Utils.Singletons;
using UnityEngine;

namespace ToB.Core
{
  public class GameManager : DDOLSingleton<GameManager>
  {
    #if UNITY_EDITOR
    // 에디터 전용 로딩되 에셋 뷰어
    [Header("Loaded Assets")]
    
    public readonly SerializableDictionary<string, AudioClip> loadedAudios = new();
    
    #endif
  }
}