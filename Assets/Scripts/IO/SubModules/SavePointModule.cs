using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ToB.Entities.FieldObject;
using UnityEngine;

namespace ToB.IO.SubModules
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

  [Serializable]
  public struct SavePointData : IEquatable<SavePointData>
  {
    public static SavePointData Default => new(0, 0, 0);
    
    [JsonProperty]
    public int stageIndex;
    [JsonProperty]
    public int roomIndex;
    [JsonProperty]
    public int pointIndex;

    public SavePointData(int stageIndex, int roomIndex, int pointIndex)
    {
      this.stageIndex = stageIndex;
      this.roomIndex = roomIndex;
      this.pointIndex = pointIndex;
    }

    public bool Equals(SavePointData other)
    {
      return stageIndex == other.stageIndex && roomIndex == other.roomIndex && pointIndex == other.pointIndex;
    }

    public override bool Equals(object obj)
    {
      return obj is SavePointData other && Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(stageIndex, roomIndex, pointIndex);
    }
  }
}