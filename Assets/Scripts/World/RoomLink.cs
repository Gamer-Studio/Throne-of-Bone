using System;
using NaughtyAttributes;
using ToB.Scenes.Stage;
using ToB.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Link")]
  public class RoomLink : MonoBehaviour
  {
    [field: SerializeField, ReadOnly] public int CurrentIndex { get; private set; } = -1;
    
    [FormerlySerializedAs("connectedRoom"), Obsolete] public AssetReference deprecated;
    public ReferenceWrapper connectedRoomReference;
    
    public int connectedLinkIndex = 0;

    [ReadOnly] public Room connectedRoom = null;
    public bool IsLoaded => connectedRoom != null;
    
    /// <summary>
    /// 에디터 전용 링크 인덱스 설정 메소드입니다. <br/>
    /// 호출하지 말아주세요.
    /// </summary>
    /// <param name="room">링크가 존재하는 방입니다.</param>
    public void Init(Room room)
    {
      CurrentIndex = room.links.FindIndex(link => link == this);
    }

    /// <summary>
    /// 방 로딩시 사용하는 메소드입니다. 호출하지 말아주세요
    /// </summary>
    /// <returns></returns>
    public Room LoadRoom(Room currentRoom)
    {
      connectedRoom = StageManager.RoomController.LoadRoom(connectedRoomReference);
      var connectedLink = connectedRoom.links[connectedLinkIndex];
      connectedLink.connectedRoom = currentRoom;
      
      var linkedDistance = connectedRoom.transform.position - connectedLink.transform.position;
      connectedRoom.gameObject.name = connectedRoomReference.path;
      connectedRoom.transform.position = transform.position + linkedDistance;
      connectedRoom.gameObject.SetActive(true);

      return connectedRoom;
    }
    
    /// <summary>
    /// 연결된 방을 언로딩시 사용하는 메소드입니다. 호출하지 말아주세요
    /// </summary>
    public void UnLoadRoom()
    {
      if (IsLoaded)
      {
        StageManager.RoomController.loadedRooms.Remove(connectedRoom.name);
        Destroy(connectedRoom.gameObject);
        connectedRoom = null;
      }
    }
  }
}