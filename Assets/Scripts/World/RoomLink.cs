using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Link")]
  public class RoomLink : MonoBehaviour
  {
    [field: Label("방의 링크 인덱스입니다."), SerializeField, ReadOnly] public int Index { get; private set; } = -1;
    public AssetReference connectedRoom;
    public int connectedIndex = 0;
    
    public void Init(Room room)
    {
      Index = room.links.FindIndex(link => link == this);
    }
  }
}