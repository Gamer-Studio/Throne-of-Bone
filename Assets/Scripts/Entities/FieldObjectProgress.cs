using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities
{
  public class FieldObjectProgress : MonoBehaviour, IJsonSerializable
  {
    public FieldObjectProgressResetType type;
    public bool saveByName;
    public virtual void LoadJson(JObject json)
    {
      
    }

    public virtual void OnLoad()
    {
      
    }

    public virtual void UpdateJson(JObject json)
    {
      
    }

    public virtual void OnUnLoad()
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

  public enum FieldObjectProgressResetType
  {
    // 방을 나가거나 들어오면 리셋됩니다.
    Room,
    // 저장시 리셋됩니다.
    Save,
    // 리셋되지 않습니다.
    NoReset,
  }
}