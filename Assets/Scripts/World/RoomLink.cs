using UnityEngine;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Link")]
  public class RoomLink : MonoBehaviour
  {
    public int connectedStageId = 1;
    public int connectedRoomId = 1;
    public int connectedIndex = 0;
  }
}