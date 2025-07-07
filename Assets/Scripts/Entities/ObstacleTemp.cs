using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities
{
  public class ObstacleTemp : MonoBehaviour, IJsonSerializable
  {
    public ObstacleResetType type;
    
    public virtual void LoadJson(JObject json)
    {
      
    }

    public virtual JObject ToJson()
    {
      return new JObject
      {
        ["name"] = name,
      };
    }
  }

  public enum ObstacleResetType
  {
    // 방을 나가거나 들어오면 리셋됩니다.
    Room,
    // 저장시 리셋됩니다.
    Save,
    // 리셋되지 않습니다.
    NoReset,
  }
}