using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using UnityEngine;

namespace ToB.IO
{
  [Serializable]
  public class SAVE
  {
    public static readonly string SavePath = Path.Combine(Application.persistentDataPath, "Saves");
    public static SAVE Current = null;
    public const string RootName = "root";
    public const int CurrentVersion = 1;
    
    [field: SerializeField] public int Version { get; private set; }
    [field: SerializeField] public SAVEModule Data { get; private set; }
    public string name;
    
    public SAVE(string name)
    {
      this.name = name;
      Version = CurrentVersion;
      Data = new SAVEModule(RootName);
    }

    public void Save()
    {
      var rootPath = Path.Combine(SavePath, name);
      var dir = new DirectoryInfo(rootPath);

      // 폴더가 이미 존재할 시 원활한 저장을 위해 초기화
      if (Directory.Exists(rootPath))
      {
        foreach (var file in dir.GetFiles())
        {
          if (file.IsReadOnly)
          {
            file.IsReadOnly = false;
          }
          file.Delete();
        }

        foreach (var info in dir.GetDirectories())
        {
          info.Delete(true);
        }
      }
      else dir.Create();
      
      Data.MetaData["version"] = CurrentVersion;

      Data.Save(rootPath);
    }
    
    public SAVEModule Node(string key, bool force = false) => Data.Node(key, force);

    /// <summary>
    /// 저장 파일의 유효성 체크
    /// </summary>
    /// <param name="path"></param>
    public static bool Validate(string savePath)
    {
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static SAVE Update(SAVE data)
    {
      return data;
    }
    
    public static async Task<SAVE> Load(string name)
    {
      var rootPath = Path.Combine(SavePath, name);
      // 저장 데이터 유효성 검사
      if (!Directory.Exists(rootPath)) throw new Exception($"Save file not found: {rootPath}");
      if (!Validate(rootPath)) throw new Exception($"Save file is invalid: {rootPath}");
      
      // 데이터 로드
      
      var result = new SAVE(name);
      await result.Data.Load(rootPath, true);

      return result;
    }
  }
}