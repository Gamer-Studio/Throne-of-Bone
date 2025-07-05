using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO
{
  [Serializable]
  public class SAVEModule : JObject
  {
    public string name;

    #region MetaData

    private Dictionary<string, SAVEModule> children;
    public string[] ChildNames => children.Keys.ToArray();
    public JObject MetaData { get; private set; }

    #endregion

#if UNITY_EDITOR
    [Label("데이터"), SerializeField, ReadOnly] private SerializableDictionary<string, SAVEModule> inspectorChildren = new();
#endif

    /// <summary>
    /// 새로운 SAVE 데이터 모듈을 생성합니다.
    /// name으로 root를 명시적으로 지정하지 마세요. 
    /// </summary>
    /// <param name="name">모듈의 명칭입니다.</param>
    public SAVEModule(string name)
    {
      this.name = name;
      children = new Dictionary<string, SAVEModule>();
      this["metaData"] = MetaData = new JObject();
    }

    public void Save(string parentPath)
    {
      var isRoot = name == SAVE.RootName;
      var path = System.IO.Path.Combine(parentPath, !isRoot ? name : "");

      if (children.Count > 0)
      {
        Directory.CreateDirectory(path);
      
        foreach (var (_, node) in children)
        {
          node.Save(path);
        }
      }

      var filename = isRoot ? System.IO.Path.Combine(path, name + ".json") : path + ".json";
      
      using var writer = File.CreateText(filename);
      using (var jsonWriter = new JsonTextWriter(writer))
      {
        jsonWriter.Formatting = Formatting.Indented;
        WriteTo(jsonWriter);

#if UNITY_EDITOR
        // Debug.Log($"[SAVE-{name}] saved to {path}");
#endif

        jsonWriter.Close();
      }

      writer.Close();
    }

    public void Load(string parentPath)
    {
      
    }

    /// <summary>
    /// key값의 자식 노드를 반환합니다.
    /// </summary>
    /// <param name="key">가져올 노드의 키값입니다.</param>
    /// <param name="force">true일시 존재하지 않으면 생성합니다.</param>
    public SAVEModule Node(string key, bool force = false)
    {
      if(children.TryGetValue(key, out var node))
        return node;
      
      if (!force) throw new Exception("Node not found");
      
      node = new SAVEModule(key);
      children.Add(key, node);
      return node;
    }

    /// <summary>
    /// 제네릭 바인딩 기능이 있는 노드를 반환합니다.
    /// 아직 사용하지 말아주세요.. 구현중입니다.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="force"></param>
    /// <typeparam name="T"></typeparam>
    public GenericSAVEModule<T> Node<T>(string key, bool force = false) where T : IJsonSerializable
    {
      if (children.TryGetValue(key, out var node))
      {
        if (node is GenericSAVEModule<T> result)
          return result;
        
        throw new Exception("Node is not GenericSAVEModule<T>");
      }
      if (!force) throw new Exception("Node not found");
      
      var newNode = new GenericSAVEModule<T>(key);
      children.Add(key, newNode);
      return newNode;
    }

    #region Getter
    
    public string this[string key, string defaultValue]
    {
      get
      {
        if (TryGetValue(key, out var token))
        {
          if (token is JValue { Type: JTokenType.String } value)
          {
            return value.Value<string>();
          }
          
          Debug.LogWarning($"[SAVE-{name}] value is not string: {key}\n type is {token.Type}");
        }
        
        Debug.LogWarning($"[SAVE-{name}] key not found: {key} ");
        
        return defaultValue;
      }
    }

    public int this[string key, int defaultValue]
    {
      get
      {
        if (TryGetValue(key, out var token))
        {
          if (token is JValue { Type: JTokenType.Integer } value)
          {
            return value.Value<int>();
          }
          
          Debug.LogWarning($"[SAVE-{name}] value is not integer: {key}\n type is {token.Type}");
        }
        
        Debug.LogWarning($"[SAVE-{name}] key not found: {key} ");
        
        return defaultValue;
      }
    }

    public float this[string key, float defaultValue]
    {
      get
      {
        if (TryGetValue(key, out var token))
        {
          if (token is JValue { Type: JTokenType.Float } value)
          {
            return value.Value<float>();
          }
          
          Debug.LogWarning($"[SAVE-{name}] value is not float: {key}\n type is {token.Type}");
        }
        
        Debug.LogWarning($"[SAVE-{name}] key not found: {key} ");
        
        return defaultValue;
      }
    }

    public bool this[string key, bool defaultValue]
    {
      get
      {
        if (TryGetValue(key, out var token))
        {
          if (token is JValue { Type: JTokenType.Boolean } value)
          {
            return value.Value<bool>();
          }
          
          Debug.LogWarning($"[SAVE-{name}] value is not boolean: {key}\n type is {token.Type}");
        }
        
        Debug.LogWarning($"[SAVE-{name}] key not found: {key} ");
        
        return defaultValue;
      }
    }
    
    #endregion
  }
}