using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerModule : SubModule
  {
    public override string ModuleType => nameof(PlayerModule);

    #region Data

    [JsonProperty]
    public int currentStage = 0;
    [JsonProperty]
    public int currentRoom = 0;
    [JsonProperty]
    public Vector3 savedPosition;

    #endregion

    public PlayerModule(string name) : base(name)
    {
    }
  }
}