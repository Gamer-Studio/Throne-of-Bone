using Newtonsoft.Json;
using UnityEngine;

namespace ToB.IO.SubModules
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