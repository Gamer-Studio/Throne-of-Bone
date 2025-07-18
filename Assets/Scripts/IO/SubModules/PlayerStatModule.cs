using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerStatModule : SAVEModule
  {
    protected override string ModuleType => nameof(PlayerStatModule);
    
    #region Data

    // 예시 데이터
    public int additionalHp = 0;
    
    #endregion

    public PlayerStatModule(string name) : base(name)
    {
    }

    protected override void BeforeSave()
    {
      base.BeforeSave();
      
      this[nameof(additionalHp)] = additionalHp;
    }
    
    public override void Read(JObject data)
    {
      base.Read(data);

      additionalHp = data.Get(nameof(additionalHp), additionalHp);
    }
  }
}