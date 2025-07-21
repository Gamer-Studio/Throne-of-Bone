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
      var data = base.BeforeSave();
      
      data[nameof(additionalHp)] = additionalHp;
      
      return data;
    }
    
    public override void Read(JObject data)
    {
      base.Read(data);
      
      additionalHp = data.Get(nameof(additionalHp), additionalHp);
    }

  }
}