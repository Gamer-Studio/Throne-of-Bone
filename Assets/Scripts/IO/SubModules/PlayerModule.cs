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
      var result = new JObject
      {
        [nameof(currentStage)] = currentStage,
        [nameof(currentRoom)] = currentRoom,
      };
      
      result.Set(nameof(savedPosition), savedPosition);
      
      return result;
    }
    
    public override void Read(JObject data)
    {
      currentStage = data.Get(nameof(currentStage), currentStage);
      currentRoom = data.Get(nameof(currentRoom), currentRoom);
      savedPosition = data.Get(nameof(savedPosition), savedPosition);
    }

    public override void Save(string parentPath)
    {
      throw new System.NotImplementedException();
    }

    public override async Task Load(string path, bool chainLoading)
    {
      throw new System.NotImplementedException();
    }

    public override SAVEModule Node(string key, bool force = false)
    {
      throw new System.NotImplementedException();
    }

    public override T Node<T>(string key, bool force = false)
    {
      throw new System.NotImplementedException();
    }
  }
}