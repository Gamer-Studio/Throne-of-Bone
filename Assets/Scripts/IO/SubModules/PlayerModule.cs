using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerModule : SubModule
  {
    public override string ModuleType => nameof(PlayerModule);

    #region Data

    public int currentStage = 0;
    public int currentRoom = 0;
    public Vector3 savedPosition;

    #endregion

    public PlayerModule(string name) : base(name)
    {
    }

    public override JObject BeforeSave()
    {
      var data = base.BeforeSave();
      
      data[nameof(currentStage)] = currentStage;
      data[nameof(currentRoom)] = currentRoom;
      data.Set(nameof(savedPosition), savedPosition);
      
      return data;
    }
    
    public override void Read(JObject data)
    {
      base.Read(data);
      
      currentStage = data.Get(nameof(currentStage), currentStage);
      currentRoom = data.Get(nameof(currentRoom), currentRoom);
      savedPosition = data.Get(nameof(savedPosition), savedPosition);
    }
  }
}