using Newtonsoft.Json;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerModule : SubModule
  {
    public override string ModuleType => nameof(PlayerModule);

    #region Data

    /// <summary>
    /// 초기 스테이지 기본값은 1
    /// </summary>
    [JsonProperty] public int currentStage = 1;
    /// <summary>
    /// 초기 방 기본값은 1
    /// </summary>
    [JsonProperty] public int currentRoom = 1;
    [JsonProperty] public Vector3 savedPosition;

    #endregion

    public PlayerModule(string name) : base(name)
    {
    }
  }
}