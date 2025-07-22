using System.Collections.Generic;
using ToB.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToB.Entities.Buffs
{
  public partial class Buff
  {
    private const string Label = "Buff";
    public static readonly Dictionary<string, Buff> All = new();

    private static Buff poison = null;
    /// <summary>
    /// 독 디버프입니다. 디버프 레벨만큼 고정 피해를 줍니다.
    /// </summary>
    public static Buff Poison => poison ??= All["Poison"];

    public static AsyncOperationHandle Load()
    {
      var handle = Addressables.LoadAssetsAsync<Buff>(new AssetLabelReference { labelString = Label }, buff =>
      {
        All[buff.name] = buff;
        
#if UNITY_EDITOR
        GameManager.Instance.loadedBuffs[buff.name] = buff;
#endif
      });

      return handle;
    }
  }
}