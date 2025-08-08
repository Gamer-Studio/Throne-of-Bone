using System;
using Newtonsoft.Json;
using ToB.Player;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.IO.SubModules.SavePoint
{
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

    public void Teleport()
    {
      var player = PlayerCharacter.Instance;
      var roomController = StageManager.RoomController;
      
      var room = roomController.LoadRoom(stageIndex, roomIndex, true);
      var bonfire = room.bonfires[pointIndex];
      player.transform.position = bonfire.TPTransform.position;
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
    
    public static bool operator ==(SavePointData data, SavePointData target) => data.Equals(target);
    public static bool operator !=(SavePointData data, SavePointData target) => !data.Equals(target);
  }
}