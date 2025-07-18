using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToB.IO.SubModules;
using ToB.Utils;
using UnityEngine;

namespace ToB.IO
{
  [Serializable]
  public class SAVEModule : JObject, ISAVEModule
  {
    private static readonly JObject Dummy = new JObject(); 
    private string name;

    public string Name
    {
      get => name;
      set => name = value;
    }

    #region MetaData

    private Dictionary<string, ISAVEModule> children;
    public string[] ChildNames => children.Keys.ToArray();
    public JObject MetaData => (JObject)this["metaData"];
    public string ModuleType => nameof(SAVEModule);

    #endregion

    /// <summary>
    /// 새로운 SAVE 데이터 모듈을 생성합니다.
    /// name으로 root를 명시적으로 지정하지 마세요. 
    /// </summary>
    /// <param name="name">모듈의 명칭입니다.</param>
    public SAVEModule(string name)
    {
      this.name = name;
      children = new Dictionary<string, ISAVEModule>();
      this["metaData"] = new JObject();
    }

    public JObject BeforeSave() => this;

    public void Save(string parentPath)
    {
      var isRoot = name == SAVE.RootName;
      var path = System.IO.Path.Combine(parentPath, !isRoot ? name : "");
      var childModuleInfo = new JObject();

      if (children.Count > 0)
      {
        Directory.CreateDirectory(path);
      
        foreach (var (_, node) in children)
        {
          node.Save(path);
          childModuleInfo[node.Name] = node.ModuleType;
        }
      }

      BeforeSave();
      
      // 자식 정보를 메타데이터로 입력
      MetaData["children"] = childModuleInfo;
      
      // 데이터 저장
      var filename = isRoot ? System.IO.Path.Combine(path, name + ".json") : path + ".json";
      
      using var writer = File.CreateText(filename);
      using (var jsonWriter = new JsonTextWriter(writer))
      {
        jsonWriter.Formatting = Formatting.Indented;
        WriteTo(jsonWriter);

        jsonWriter.Close();
      }

      writer.Close();
    }
    
    /// <summary>
    /// JObject에서 데이터를 읽어옵니다.
    /// </summary>
    /// <param name="data"></param>
    public virtual void Read(JObject data)
    {
      foreach (var (key, value) in data)
      {
        if (value is null) continue;
        
        this[key] = value;
      }
    }
    
    /// <summary>
    /// IJsonSerializable 오브젝트에서 데이터를 읽어옵니다.
    /// </summary>
    /// <param name="data"></param>
    public virtual void Read(IJsonSerializable data) => Read(data.ToJson());

    /// <summary>
    /// 내부 데이터 로딩용 메소드입니다. 호출하지 말아주세요!
    /// </summary>
    /// <param name="path"></param>
    /// <param name="chainLoading"></param>
    public async Task Load(string path, bool chainLoading)
    {
      var isRoot = name == SAVE.RootName;
      var filename = isRoot ? System.IO.Path.Combine(path, name + ".json") : path + ".json";

      try
      {
        using var reader = File.OpenText(filename);
        using var jsonReader = new JsonTextReader(reader);
        var data = await ReadFromAsync(jsonReader);
        Read((JObject)data);
      }
      catch (JsonReaderException e)
      {
        #if UNITY_EDITOR
        if (DebugSymbol.Save)
        {
          Debug.LogWarning($"[SAVE-{name}] Error while loading {filename}.");
          Debug.LogException(e);
        }
        #endif
      }
      
      var childModuleInfo = MetaData.Get("children", Dummy);

      if (chainLoading && Directory.Exists(path))
      {
        foreach (var child in Directory.GetFiles(path))
        {
          var childInfo = new FileInfo(child);
          
          if (!childInfo.Extension.Equals(".json"))
          {
            DebugSymbol.Save.Log($"[SAVE-{name}] {childInfo.Name} is not json. skipping...");
            continue;
          }
          
          var fileName = childInfo.Name.Replace(childInfo.Extension, "");

          if (fileName == name)
          {
            DebugSymbol.Save.Log($"[SAVE-{name}] {childInfo.Name} is root. skipping...");
            continue;
          }

          ISAVEModule childModule = childModuleInfo.Get(fileName, nameof(SAVEModule)) switch
          {
            nameof(PlayerModule) => Node<PlayerModule>(fileName, true),
            nameof(PlayerStatModule) => Node<PlayerStatModule>(fileName, true),
            _ => Node(fileName, true),
          };
          await childModule.Load(System.IO.Path.Combine(path, fileName), true);
        }
      }
    }

    /// <summary>
    /// key값의 자식 노드를 반환합니다.
    /// </summary>
    /// <param name="key">가져올 노드의 키값입니다.</param>
    /// <param name="force">true일시 존재하지 않으면 생성합니다.</param>
    public SAVEModule Node(string key, bool force = false)
    {
      if (children.TryGetValue(key, out var node))
      {
        if (node is SAVEModule value) return value;
        else throw new Exception("Node type is not match");
      }
      
      if (!force) throw new Exception("Node not found");
      
      var resultNode = new SAVEModule(key);
      children.Add(key, resultNode);
      return resultNode;
    }

    /// <summary>
    /// 제네릭 바인딩 기능이 있는 노드를 반환합니다.
    /// 아직 사용하지 말아주세요.. 구현중입니다.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="force"></param>
    /// <typeparam name="T"></typeparam>
    public T Node<T>(string key, bool force = false) where T : ISAVEModule
    {
      if(children.TryGetValue(key, out var node))
      {
        if (node is not T value) throw new Exception("Node type is not match");
        
        return value;
      }
      
      if (!force) throw new Exception("Node not found");
      
      var result = typeof(T) switch
      {
        var type when type == typeof(SAVEModule) => (T) (object) new SAVEModule(key),
        var type when type == typeof(PlayerModule) => (T) (object) new PlayerModule(key),
        var type when type == typeof(PlayerStatModule) => (T) (object) new PlayerStatModule(key),
        _ => (T) Activator.CreateInstance(typeof(T), key),
      };
      
      children.Add(key, result);
      return result;
    }

    /// <summary>
    /// IJsonSerializable 구현 메소드입니다.
    /// </summary>
    public JObject ToJson() => this;
    
    /// <summary>
    /// IJsonSerializable 구현 메소드입니다.
    /// </summary>
    public void LoadJson(JObject json) => Read(json);

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
          
          DebugSymbol.Save.Log($"[SAVE-{name}] value is not string: {key}\n type is {token.Type}");
        }
        
        DebugSymbol.Save.Log($"[SAVE-{name}] key not found: {key} ");
        
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
          
          DebugSymbol.Save.Log($"[SAVE-{name}] value is not integer: {key}\n type is {token.Type}");
        }
        
        DebugSymbol.Save.Log($"[SAVE-{name}] key not found: {key} ");
        
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
          
          DebugSymbol.Save.Log($"[SAVE-{name}] value is not float: {key}\n type is {token.Type}");
        }
        
        DebugSymbol.Save.Log($"[SAVE-{name}] key not found: {key} ");
        
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
          
          DebugSymbol.Save.Log($"[SAVE-{name}] value is not boolean: {key}\n type is {token.Type}");
        }
        
        DebugSymbol.Save.Log($"[SAVE-{name}] key not found: {key} ");
        
        return defaultValue;
      }
    }
    
    #endregion
  }
}