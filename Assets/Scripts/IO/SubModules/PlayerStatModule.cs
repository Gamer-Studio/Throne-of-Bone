using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerStatModule : SubModule
  {
    public override string ModuleType => nameof(PlayerStatModule);

    #region Data

    // 예시 데이터
    public int additionalHp = 0;

    #endregion

    public PlayerStatModule(string name) : base(name)
    {
    }

    public override JObject BeforeSave()
    {
      var result = new JObject
      {
        [nameof(additionalHp)] = additionalHp,
      };
      
      return result;
    }
    
    public override void Read(JObject data)
    {
      additionalHp = data.Get(nameof(additionalHp), additionalHp);
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