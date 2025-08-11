using Newtonsoft.Json;
using UnityEngine;

namespace ToB.IO.SubModules.Players
{
  public class PlayerModule : SubModule
  {
    public override string ModuleType => nameof(PlayerModule);

    #region Data


    #endregion

    public PlayerModule(string name) : base(name)
    {
    }
  }
}