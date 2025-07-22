using System.Collections.Generic;
using Newtonsoft.Json;
using ToB.Entities.Skills;

namespace ToB.IO.SubModules
{
  public class PlayerStatModule : SubModule
  {
    public override string ModuleType => nameof(PlayerStatModule);

    #region Data

    [JsonProperty]
    public Dictionary<int, SkillState> savedPlayerSkillState = new();

    #endregion

    public PlayerStatModule(string name) : base(name) { }
  }
}