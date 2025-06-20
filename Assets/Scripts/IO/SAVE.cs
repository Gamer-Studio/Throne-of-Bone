using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO
{
  /// <summary>
  /// 임시로 단일 JSON으로 구현하였습니다. <br/>
  /// TODO 네임스페이스(폴더)단위 파일 저장 시스템 구축 필요
  /// </summary>
  [Serializable]
  public class SAVE : JObject
  {
    public const string Version = "1.0";
    
    public readonly string name;
    [SerializeField] private bool initialized = false;
    public bool IsInitialized => initialized;
    
    #region Modules
    
    #endregion

    /// <summary>
    /// SAVE 객체를 초기화합니다.
    /// </summary>
    /// <param name="name">저장 파일의 이름</param>
    /// <param name="isRoot">루트 저장 객체 여부</param>
    public SAVE(string name, bool isRoot = true)
    {
      this.name = name;
      this[nameof(Version)] = Version;
    }

    /// <summary>
    /// SAVE 객체를 초기화합니다. 이미 초기화된 경우 아무 동작도 하지 않습니다.
    /// </summary>
    public void Init()
    {
      if(initialized) return;
      initialized = true;
    }

    /// <summary>
    /// 현재 SAVE 객체를 비동기적으로 파일에 저장합니다.
    /// </summary>
    /// <returns>저장 작업 완료를 나타내는 Task</returns>
    public async Task Save()
    {
      var path = System.IO.Path.Combine(SavePath, name + ".json");
      
      if (!Directory.Exists(SavePath))
        Directory.CreateDirectory(SavePath);
      
      var writer = File.CreateText(path);
      var jsonWriter = new JsonTextWriter(writer);
      jsonWriter.Formatting = Formatting.Indented;
      
      // 모듈 데이터를 여따 때려박기
      // 데이터를 json 문자열 데이터로 변환
      var writeData = new JObject(this)
      {
      };

      await writeData.WriteToAsync(jsonWriter);
      
#if UNITY_EDITOR
      Debug.Log($"Save as {path} Complete!");
#endif
      await jsonWriter.CloseAsync();
      writer.Close();
    }

    /// <summary>
    /// 현재 SAVE 객체를 동기적으로 파일에 저장합니다.
    /// </summary>
    public void SaveSync()
    {
      Save().Wait();
    }
    
    #region Static Methods

    public static readonly string SavePath = System.IO.Path.Combine(Application.persistentDataPath, "Saves");
    public static SAVE Current = null;

    /// <summary>
    /// 현재 활성화된 SAVE 객체를 비동기적으로 저장합니다.
    /// </summary>
    /// <returns>저장 작업 완료를 나타내는 Task</returns>
    public static async Task SaveCurrent()
    {
      if(Current != null)
        await Current.Save();
    }

    /// <summary>
    /// 지정된 이름의 저장 파일을 비동기적으로 로드합니다.
    /// </summary>
    /// <param name="name">로드할 저장 파일의 이름</param>
    /// <param name="create">파일이 존재하지 않을 경우 새로 생성할지 여부</param>
    /// <returns>로드된 SAVE 객체를 포함한 Task</returns>
    public static async Task<SAVE> Load(string name, bool create = false)
    {
      var path = System.IO.Path.Combine(SavePath, name + ".json");
      if (Directory.Exists(SavePath) && File.Exists(path))
      {
        try
        {
          var reader = new JsonTextReader(File.OpenText(path));
          var result = new SAVE(name);

          if (await ReadFromAsync(reader) is JObject obj
              && obj.TryGetValue(nameof(Version), out var version) && version.Value<string>() == Version)
          {
            foreach (var (key, value) in obj)
            {
              if (value is null) continue;

              // 모듈 데이터 불러오는 코드는 여따 때려박기
              switch (key)
              {
                default:
                  result[key] = value;
                  break;
              }
            }

            result.Init();
            reader.Close();

            return result;
          }

          reader.Close();
        }
        catch (JsonReaderException e)
        {
          #if UNITY_EDITOR
          Debug.LogWarning($"Error while loading {path}.");
          Debug.LogException(e);
          #endif
        }
      }
      
      if(create)
      {
        var save = new SAVE(name);
        save.Init();
        await save.Save();
        return save;
      }
      
      return null;
    }

    /// <summary>
    /// 지정된 이름의 저장 파일을 동기적으로 로드합니다.
    /// </summary>
    /// <param name="name">로드할 저장 파일의 이름</param>
    /// <param name="create">파일이 존재하지 않을 경우 새로 생성할지 여부</param>
    /// <returns>로드된 SAVE 객체</returns>
    public static SAVE LoadSync(string name, bool create = false)
    {
      var task = Load(name, create);
      task.Wait();
      return task.Result;
    }

    /// <summary>
    /// 모든 저장 파일의 정보를 가져옵니다.
    /// </summary>
    /// <returns>저장 파일들의 FileInfo 배열</returns>
    public static FileInfo[] GetSaveNames()
    {
      FileInfo[] result = null;
      
      if (Directory.Exists(SavePath))
      {
        result = new DirectoryInfo(SavePath).GetFiles("*.json");
      }
      
      return result;
    }

    /// <summary>
    /// 지정된 이름의 저장 파일이 존재하는지 확인합니다.
    /// </summary>
    /// <param name="name">확인할 저장 파일의 이름</param>
    /// <returns>파일 존재 여부</returns>
    public static bool Exist(string name)
    {
      var path = System.IO.Path.Combine(SavePath, name + ".json");
      return Directory.Exists(SavePath) && File.Exists(path);
    }

    /// <summary>
    /// 지정된 이름의 저장 파일을 로드하여 SAVE 객체로 반환을 시도합니다.
    /// </summary>
    /// <param name="name">로드할 저장 파일의 이름</param>
    /// <param name="save">로드된 SAVE 객체</param>
    /// <returns>로드 성공 여부</returns>
    public static bool TryGet(string name, out SAVE save)
    {
      save = null;
      if (Exist(name))
      {
        var task = Load(name);
        task.Start();
        task.Wait();
        save = task.Result;
        return true;
      }
      
      return false;
    }
    
    #endregion
  }
}