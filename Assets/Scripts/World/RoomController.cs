using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ToB.IO;
using ToB.Scenes.Stage;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.World
{
  public class RoomController : MonoBehaviour
  {
    #region State

    [Label("현재 플레이어가 있는 방")] public Room currentRoom;
    [Label("로딩된 방 목록")] public SerializableDictionary<string, Room> loadedRooms = new();
    
    #endregion
    
    #region Binding
    private const string Binding = "Binding";
    
    [Foldout(Binding), SerializeField] private Transform roomContainer;
    
    #endregion
    
    #region Unity Event

    private void Awake()
    {
      StageManager.Instance.onRoomEnter.AddListener(LoadLinkedRoom);
    }
    
    #endregion

    public Room LoadRoom(int stageIndex, int roomIndex, bool initActive = false)
      => LoadRoom(new ReferenceWrapper($"Stage{stageIndex}/Room{roomIndex}"), initActive);

    public Room LoadRoom(ReferenceWrapper reference, bool initActive = false)
    {
      if (loadedRooms.TryGetValue(reference.path, out var room) && room)
        return room;
      
      var roomObject = Instantiate(reference.assetReference.LoadAssetAsync<GameObject>().WaitForCompletion(), roomContainer, true);
      roomObject.SetActive(initActive);
      roomObject.name = reference.path;
      room = roomObject.GetComponent<Room>();
      roomObject.transform.position = Vector3.zero;

      loadedRooms[reference.path] = room;
      
      reference.assetReference.ReleaseAsset();
      return room;
    }

    private void LoadLinkedRoom(Room room)
    {
      foreach (var link in room.links)
      {
        if (!link.IsLoaded) link.LoadRoom(room);
      }

      var currentLinkedRooms = room.GetLinkedRooms(true);
      var removes = new List<string>();

      foreach (var pair in loadedRooms)
      {
        if (!currentLinkedRooms.Contains(pair.Value) && pair.Value && pair.Value != room)
        {
          removes.Add(pair.Key);
          Destroy(pair.Value.gameObject);
        }
      }

      foreach (var key in removes)
        loadedRooms.Remove(key);
    }
  }
}