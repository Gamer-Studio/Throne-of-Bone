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
    [JsonProperty]
    public Dictionary<int, bool> savedMemoryStates = new();

    [JsonProperty] public int playerGold;
    [JsonProperty] public int playerMana;
    [JsonProperty] public int playerUsedGold;
    [JsonProperty] public int playerUsedMana;
    [JsonProperty] public int playerKey;

    #endregion

    public PlayerStatModule(string name) : base(name) { }
  }
}