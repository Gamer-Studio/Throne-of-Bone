using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Link")]
  public class RoomLink : MonoBehaviour
  {
    [field: SerializeField, ReadOnly] public int CurrentIndex { get; private set; } = -1;
    public AssetReference connectedRoom;
    public int connectedLinkIndex = 0;
    
    public void Init(Room room)
    {
      CurrentIndex = room.links.FindIndex(link => link == this);
    }
  }
}