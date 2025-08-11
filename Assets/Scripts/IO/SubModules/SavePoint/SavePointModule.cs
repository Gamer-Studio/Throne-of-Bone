using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ToB.Entities.FieldObject;
using ToB.Player;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.IO.SubModules.SavePoint
{
  public class SavePointModule : SubModule
  {
    public override string ModuleType => nameof(SavePointModule);

    #region Data
    
    [JsonProperty]
    public List<SavePointData> activeSavePoints = new ();
    
    [JsonProperty]
    public int lastSavePoint = -1;
    
    #endregion
    
    public SavePointModule(string name) : base(name) { }

    public void UpdateSavePoint(Bonfire bonfire)
    {
      int stage = bonfire.room.stageIndex,
        room = bonfire.room.roomIndex,
        bonfireIndex = bonfire.room.bonfires.IndexOf(bonfire);

      var index = activeSavePoints.FindIndex(data =>
        data.stageIndex == stage && data.roomIndex == room && data.pointIndex == bonfireIndex);

      if (index != -1)
        lastSavePoint = index;
      else
      {
        activeSavePoints.Add(new SavePointData
        {
          stageIndex = stage,
          roomIndex = room,
          pointIndex = bonfireIndex,
        });
        
        lastSavePoint = activeSavePoints.Count - 1;
      }
    }

    public SavePointData GetLastSavePoint()
    {
      if(lastSavePoint == -1) return SavePointData.Default;
      else return activeSavePoints[lastSavePoint];
    }
  }

}