using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ToB.Entities.Skills;
using UnityEngine;

namespace ToB.IO.SubModules
{
  public class PlayerStatModule : SubModule
  {
    public override string ModuleType => nameof(PlayerStatModule);

    #region Data

    public Dictionary<int, SkillState> SavedPlayerSkillState = new();

    #endregion

    public PlayerStatModule(string name) : base(name)
    {
      
    }

    public override JObject BeforeSave()
    {
      var data = base.BeforeSave();
      
      var States = new JObject();
      
      foreach (var(key,value) in SavedPlayerSkillState)
      {
        States.Set(key.ToString(), value);
      }
      
      data[nameof(SavedPlayerSkillState)] = States;
      
      return data;
    }
    
    public override void Read(JObject data)
    {
      base.Read(data);
      
      var States = data.Get(nameof(SavedPlayerSkillState), JsonUtil.Blank);
      
      foreach (var (key, value) in States)
      {
        if (value is JObject skillState)
        {
          SavedPlayerSkillState.Add(int.Parse(key), skillState.GetEnum<SkillState>(key));
        }
      }
    }

  }
}