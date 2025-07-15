using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities
{
  public class Entity : MonoBehaviour, IJsonSerializable
  {
    public EntityType type;
    
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

  public enum EntityType
  {
    Common,
    Elite,
    Boss,
    Npc
  }
}