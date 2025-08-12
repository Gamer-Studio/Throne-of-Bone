using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.IO.Converters;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToB.World.Structures
{
  public class Structure : MonoBehaviour, IJsonSerializable
  {
    public const string Label = "Structure";
    public static Dictionary<string, GameObject> prefabs = new();
    [field:Label("프리팹 이름"), SerializeField, ReadOnly] public string PrefabName { get; private set; }
    
    #region Binding

    private const string Binding = "Binding";
    [Label("구조물이 배치된 방"), SerializeField, Foldout(Binding)] public Room room;
    
    #endregion
    
    #region Unity Event

    protected void OnDestroy()
    {
      if (room)
        room.structures.Remove(this);
    }

    #endregion
    
    
    public virtual void LoadJson(JObject json)
    {
    }

    public virtual JObject ToJson()
    {
      var json = new JObject
      {
        [nameof(PrefabName)] = PrefabName,
        ["position"] = transform.localPosition.ToJValue(),
      };

      return json;
    }

    /// <summary>
    /// ContentLoader에서 로딩용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public static AsyncOperationHandle Load()
    {
      var structureLoader = Addressables.LoadAssetsAsync<GameObject>(new AssetLabelReference {labelString = Label},
        obj =>
        {
          prefabs[obj.name] = obj;
        });
      
      return structureLoader;
    }

    /// <summary>
    /// 방에 생성형 구조물을 소환합니다.
    /// </summary>
    /// <param name="room">구조물을 소환할 방입니다.</param>
    /// <param name="structureName">구조물의 프리팹 명칭입니다.</param>
    /// <returns>불러오는 중 오류가 발생할 경우 Null을 반환합니다.</returns>
    public static Structure Spawn(Room room, string structureName)
    {
      if (!room)
      {
        DebugSymbol.ETC.Log("방이 null입니다.");
        return null;
      }
      if (!prefabs.TryGetValue(structureName, out var prefab))
      {
        DebugSymbol.ETC.Log($"프리팹 {structureName}이 정의되어있지 않습니다.");
        return null;
      }

      var obj = Instantiate(prefab);

      if (!obj.TryGetComponent(out Structure structure))
      {
        DebugSymbol.ETC.Log($"프리팹 {structureName}이 정상적인 구조물 오브젝트가 아닙니다.");
        return null;
      }
      
      room.structures.Add(structure);
      structure.room = room;
      structure.transform.SetParent(room.transform);
      structure.PrefabName = structureName;
      structure.name += "_" + structure.GetInstanceID();
      
      structure.transform.localPosition = Vector3.zero;
      structure.transform.localRotation = Quaternion.identity;

      return structure;
    }
  }
}