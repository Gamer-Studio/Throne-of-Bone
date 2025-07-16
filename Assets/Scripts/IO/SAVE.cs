using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ToB.Core;
using ToB.Player;
using ToB.Utils;
using UnityEngine;

namespace ToB.IO
{
  [Serializable]
  public class SAVE
  {
    /// <summary>
    /// SAVE.Current에 저장 파일이 로딩될 때 호출됩니다.
    /// </summary>
    public static event Action<SAVE> OnCurrentLoad = save =>
    {
      ResourceManager.Instance.LoadJson(save.Node(nameof(ResourceManager), true));
    };

    /// <summary>
    /// SAVE.Current가 저장될 때 호출됩니다.
    /// </summary>
    public static event Action<SAVE> OnCurrentSave = save =>
    {
      save.Node(nameof(ResourceManager)).Read(ResourceManager.Instance);
    };
    
    public static readonly string SavePath = Path.Combine(Application.persistentDataPath, "Saves");
    public static SAVE Current = null;
    public const string RootName = "root";
    public const int CurrentVersion = 1;

    /// <summary>
    /// 해당 저장파일이 저장될 때 호출됩니다.
    /// </summary>
    public event Action OnSave;
    
    [field: SerializeField] public SAVEModule Data { get; private set; }
    
    #region MetaData
    
    public JObject MetaData => Data.MetaData;
    public string fileName;
    public string name = "empty";
    public int gold = 0;
    [field: SerializeField] public string SaveTime { get; private set; }
    [field: SerializeField] public int Version { get; private set; }

    #endregion
    
    #region Defines

    /// <summary>
    /// Root/Player 경로의 데이터입니다.
    /// </summary>
    public SAVEModule Player => Node(nameof(Player), true);

    #endregion
    
    public SAVE(string fileName)
    {
      this.fileName = fileName;
      Version = CurrentVersion;
      Data = new SAVEModule(RootName);
      
      SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      OnSave += () => { if (Current == this) OnCurrentSave.Invoke(this); };
      InitMetaData();
    }

    public void Save()
    {
      {
        // 저장 전 액션
        
        if(PlayerCharacter.HasInstance)
          PlayerCharacter.Instance.Save();
      }
      
      
      SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      
      var rootPath = Path.Combine(SavePath, fileName);
      DebugSymbol.Save.Log($"Save file saved: {rootPath}");

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

      InitMetaData();

      Data.Save(rootPath);
      
      OnSave?.Invoke();
    }

    private void InitMetaData()
    {
      MetaData[nameof(name)] = name;
      MetaData[nameof(gold)] = gold;
      MetaData[nameof(SaveTime)] = SaveTime;
      MetaData[nameof(Version)] = Version;
    }
    
    public SAVEModule Node(string key, bool force = false) => Data.Node(key, force);

    /// <summary>
    /// TODO 저장 파일의 유효성 체크
    /// </summary>
    /// <param name="path"></param>
    public static bool Validate(string savePath)
    {
      return true;
    }

    /// <summary>
    /// 모든 저장 파일을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public static async Task<SAVE[]> GetAllSaves()
    {
      var result = new SAVE[3];

      for (var i = 0; i < result.Length; i++)
      {
        var name = "save_" + i;
        var target = result[i] = Exists(name) ? await Load(name) : new SAVE(name);
        
        // 메타데이터 불러오기
        
        target.name = target.MetaData.Get(nameof(name), target.name);
        target.gold = target.MetaData.Get(nameof(gold), target.gold);
        target.SaveTime = target.MetaData.Get(nameof(SaveTime), target.SaveTime);
        target.Version = target.MetaData.Get(nameof(Version), CurrentVersion);
      }
      
      return result;
    }
    
    /// <summary>
    /// 구버전 데이터일 경우 최신버전으로 업데이트시킵니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static SAVE Update(SAVE data)
    {
      return data;
    }

    /// <summary>
    /// 해당 세이브로 게임을 시작하기 전에 호출해주세요.
    /// </summary>
    public async UniTask LoadAll()
    {
      var rootPath = Path.Combine(SavePath, fileName);
      if (Directory.Exists(rootPath) && Validate(rootPath))
      {
        await Data.Load(rootPath, true);
      }
        
      Current = this;
      
      OnCurrentLoad.Invoke(this);
    }

    public static bool Exists(string name)
    {
      var rootPath = Path.Combine(SavePath, name);
      return Directory.Exists(rootPath);
    }
    
    public static async Task<SAVE> Load(string name)
    {
      var rootPath = Path.Combine(SavePath, name);
      // 저장 데이터 유효성 검사
      if (!Directory.Exists(rootPath)) throw new Exception($"Save file not found: {rootPath}");
      if (!Validate(rootPath)) throw new Exception($"Save file is invalid: {rootPath}");
      
      // 데이터 로드
      
      var result = new SAVE(name);
      await result.Data.Load(rootPath, false);
      
      // 메타 데이터 세팅
      var meta = result.MetaData;
      
      result.name = meta.Get("name", "empty");
      result.gold = meta.Get("gold", 0);
      result.SaveTime = meta.Get("saveTime", "not saved");
      result.Version = meta.Get("version", CurrentVersion);

      return result;
    }
  }
}