using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerModule : SAVEModule
  {
    protected override string ModuleType => nameof(PlayerModule);
    
    #region Data

    public int currentStage = 0;
    public int currentRoom = 0;
    public Vector3 savedPosition;
    
    #endregion

    public PlayerModule(string name) : base(name)
    {
    }

    protected override void BeforeSave()
    {
      base.BeforeSave();
      
      this[nameof(currentStage)] = currentStage;
      this[nameof(currentRoom)] = currentRoom;
      this.Set(nameof(savedPosition), savedPosition);
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