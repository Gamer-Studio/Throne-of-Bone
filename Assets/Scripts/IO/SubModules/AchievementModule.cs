using Newtonsoft.Json;

namespace ToB.IO.SubModules
{
  public class AchievementModule : SubModule
  {
    public override string ModuleType => nameof(AchievementModule);

    #region Data

    [JsonProperty]
    public bool KillRat = false;
    
    [JsonProperty]
    public bool KillSentinel = false;

    #endregion

    public AchievementModule(string name) : base(name)
    {
    }
  }
}