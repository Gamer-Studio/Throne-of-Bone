using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using ToB.Entities;
using ToB.IO;
using ToB.Scenes.Stage;
using ToB.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room")]
  public class Room : MonoBehaviour, IJsonSerializable
  {
    [Label("스테이지 인덱스")] public int stageIndex;
    [Label("방 인덱스")] public int roomIndex;
    [Label("데이터 모듈"), SerializeField] private SAVEModule saveModule;
    
    #region Binding
    private const string Binding = "Binding"; 

#if UNITY_EDITOR
    [ContextMenuItem("내부 링크 찾기", nameof(FindLinks))]
#endif
    [Foldout(Binding)] public List<RoomLink> links = new();
    [Foldout(Binding), SerializeField] private Transform entityContainer; 
    [Label("일반 적 소환 정보"), Foldout(Binding)] public SerializableDictionary<Transform, AssetReference> normalEnemyTable = new();
    [Label("오브젝트"), Foldout(Binding)] public SerializableDictionary<string, FieldObjectProgress> fieldObjects = new();
    [Label("인스턴스된 적"), Foldout(Binding), SerializeField, ReadOnly] private SerializableDictionary<Transform, Enemy> enemies = new();
    [field: Foldout(Binding), SerializeField] public RoomBackground Background { get; private set; }
    
    #endregion
    
    #region Event
    
    [Foldout("이벤트")] public UnityEvent onLoad = new();
    [Foldout("이벤트")] public UnityEvent onUnload = new();
    [Foldout("이벤트")] public UnityEvent onEnter = new();
    [Foldout("이벤트")] public UnityEvent onExit = new();
    
    #endregion
    
    
    #if UNITY_EDITOR

    [Button("내부 오브젝트 찾기")]
    private void FindStructures()
    {
      fieldObjects.Clear();
      
      FindStructure(transform);
    }
    
    private void FindStructure(Transform tr)
    {
      for (var i = 0; i < tr.childCount; i++)
      {
        var child = tr.GetChild(i);
        if (child.TryGetComponent<FieldObjectProgress>(out var structure))
        {
          DebugSymbol.Editor.Log(structure.name);
          structure.room = this;
          fieldObjects[child.name] = structure;
        }
        
        FindStructure(child);
      }
    }

    private void FindLinks()
    {
      links.Clear();
      FindLink(transform);
    }
    
    private void FindLink(Transform tr)
    {
      for (var i = 0; i < tr.childCount; i++)
      {
        var child = tr.GetChild(i);
        if (child.TryGetComponent<RoomLink>(out var link))
        {
          DebugSymbol.Editor.Log(link.name);
          links.Add(link);
        }
        
        FindLink(child);
      }
    }
    
    #endif

    #region Unity Event
    
    #if UNITY_EDITOR

    private void Reset()
    {
      if(!Background) Background = transform.GetComponentInChildren<RoomBackground>();
      
      FindLinks();
      FindStructures();
    }
    
    #endif

    private void Awake()
    {
      if (Background)
      {
        Background.onEnter.AddListener(Enter);
        Background.onExit.AddListener(Exit);
      }
    }

    private void Start()
    {
      Load();
    }

    private void OnEnable()
    {
      StageManager.Instance?.AddCameraCollision(Background.backgroundCollider);
    }

    private void OnDisable()
    {
      StageManager.Instance?.RemoveCameraCollision(Background.backgroundCollider);
    }

    private void OnDestroy()
    {
      Unload();
    }

    #endregion
    
    #region Feature

    /// <summary>
    /// Root/Stage/Room_{stageIndex}_{roomIndex} 경로로
    /// 방 데이터를 명시적으로 저장합니다.
    /// </summary>
    public void Save()
    {
      saveModule.Read(this);
    }

    /// <summary>
    /// 방을 불러올 때 데이터를 읽고 난 후 호출됩니다.
    /// </summary>
    public virtual void Load()
    {
      if (SAVE.Current != null)
      {
        saveModule = SAVE.Current.Node("Stage", true).Node($"Room_{stageIndex}_{roomIndex}", true);

        LoadJson(saveModule);
      }
      
      onLoad?.Invoke();
    }

    /// <summary>
    /// 방을 언로딩할 때 데이터를 저장 이후 호출됩니다. <br/>
    /// 이후 방을 Destroy 합니다.
    /// </summary>
    public virtual void Unload()
    {
      foreach (var pair in enemies)
      {
        if(pair.Value)
          pair.Value.Release();
      }
      
      enemies.Clear();
      
      Save();
      
      onUnload?.Invoke();
      // Destroy(gameObject);
    }
    
    /// <summary>
    /// 플레이어가 방에 들어올 떄 호출됩니다.
    /// </summary>
    protected virtual void Enter()
    {
      foreach (var pair in normalEnemyTable)
      {
        if(pair.Value == null || (enemies.ContainsKey(pair.Key) && enemies[pair.Key].IsAlive)) continue;
        
        var enemy = (Enemy)pair.Value.Pooling();
        
        enemy.transform.SetParent(entityContainer);
        enemy.transform.position = pair.Key.position;
        
        enemies[pair.Key] = enemy;
      }
      
      foreach (var obj in fieldObjects)
        obj.Value.OnLoad();

      onEnter?.Invoke();
      StageManager.Instance.onRoomEnter.Invoke(this);
      StageManager.Instance.currentRoom = this;
    }

    /// <summary>
    /// 플레이어가 방에서 나갈 떄 호출됩니다.
    /// </summary>
    protected virtual void Exit()
    {
      DebugSymbol.Save.Log($"Exit room {stageIndex} / {roomIndex}");
      
      foreach (var pair in enemies)
      {
        if (!pair.Value.IsAlive)
        {
          pair.Value.Release();
          enemies.Remove(pair.Key);
        }
      }

      foreach (var obj in fieldObjects)
        obj.Value.OnUnLoad();
      
      onExit?.Invoke();
      if(StageManager.HasInstance) StageManager.Instance.onRoomExit.Invoke(this);
    }
    
    #endregion
    
    #region Serialization

    public void LoadJson(JObject json)
    {
      var dummy = new JObject();
      var objects = json.Get(nameof(fieldObjects), dummy);

      foreach (var pair in fieldObjects)
      {
        pair.Value.room = this;
        pair.Value.LoadJson(objects.Get(pair.Key, dummy));
      }
    }

    public JObject ToJson()
    {
      var objects = new JObject();
      var result = new JObject
      {
      };
      
      foreach (var pair in fieldObjects)
      {
        objects[pair.Key] = pair.Value.ToJson();
      }

      result[nameof(fieldObjects)] = objects;
      
      return result;
    }
    
    #endregion
  }
}