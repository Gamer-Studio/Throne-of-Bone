using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Link")]
  public class RoomLink : MonoBehaviour
  {
    public int connectedStageId = 1;
    public int connectedRoomId = 1;
    public int connectedIndex = 0;

    public AssetReference RoomRef => new AssetReference($"Stage{connectedStageId}/Room{connectedRoomId}");

#if UNITY_EDITOR
    [ShowNativeProperty] private string refString => RoomRef.AssetGUID;
#endif
  }
}