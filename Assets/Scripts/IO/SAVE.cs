using System;
using System.IO;
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
    public const string CurrentVersion = "Alpha-0.1";
    
    [field: SerializeField] public string Version { get; private set; }
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

    public static void Load(string name)
    {
      var rootPath = Path.Combine(SavePath, name);

      
    }
  }
}