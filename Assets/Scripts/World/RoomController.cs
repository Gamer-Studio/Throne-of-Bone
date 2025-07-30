using System.Collections.Generic;
using NaughtyAttributes;
using ToB.Worlds;
using UnityEngine;

namespace ToB.World
{
  public class RoomController : MonoBehaviour
  {
    #region State
    private const string State = "State";
    
    [Label("로딩된 방 목록"), Foldout(State)] public List<Room> loadedRooms = new();
    
    #endregion

  }
}