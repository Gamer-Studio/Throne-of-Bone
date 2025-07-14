using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities
{
  public class FieldObjectProgress : MonoBehaviour, IJsonSerializable
  {
    /// <summary>
    /// 방 로딩시 오브젝트를 로딩할 떄 호출됩니다.
    /// </summary>
    /// <param name="json"></param>
    public virtual void LoadJson(JObject json)
    {
      
    }

    /// <summary>
    /// 플레이어가 방에 입장할 떄 호출됩니다.
    /// </summary>
    public virtual void OnLoad()
    {
      
    }

    public virtual void UpdateJson(JObject json)
    {
      
    }

    /// <summary>
    /// 플레이어가 방에서 나갈 때 호출됩니다.
    /// </summary>
    public virtual void OnUnLoad()
    {
      
    }

    /// <summary>
    /// 오브젝트를 저장할 떄 호출합니다.
    /// </summary>
    /// <returns></returns>
    public virtual JObject ToJson()
    {
      return new JObject
      {
        ["name"] = name,
      };
    }

    #region Utility

    

    #endregion
  }
}