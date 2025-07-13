using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using ToB.Entities;
using ToB.IO;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room")]
  public class Room : MonoBehaviour, IJsonSerializable
  {
    [Label("스테이지 인덱스"), Foldout("State")] public int stageIndex;
    [Label("방 인덱스"), Foldout("State")] public int roomIndex;
    [Label("일반 적 소환 정보"), Foldout("State")] public SerializableDictionary<Transform, GameObject> normalEnemyTable = new();
    [Label("오브젝트"), Foldout("State")] public SerializableDictionary<string, FieldObjectProgress> fieldObjects = new();
    [Label("데이터 모듈"), Foldout("State"), SerializeField] private SAVEModule saveModule;
    
    #region Binding

    #if UNITY_EDITOR
    [ContextMenuItem("내부 링크 찾기", nameof(FindLinks))]
    #endif
    [Foldout("Binding")] public List<RoomLink> links = new();
    [Foldout("Binding"), SerializeField] private Transform entityContainer; 
    [Foldout("Binding"), SerializeField] private Transform fieldObjectContainer;
    [field: Foldout("Binding"), SerializeField] public RoomBackground Background { get; private set; }
    
    #endregion
    
    #region Event
    
    [Foldout("이벤트")] public UnityEvent onLoad = new();
    [Foldout("이벤트")] public UnityEvent onUnload = new();
    [Foldout("이벤트")] public UnityEvent onEnter = new();
    [Foldout("이벤트")] public UnityEvent onExit = new();
    
    #endregion
    
    #region Managed
    
    [SerializeField, ReadOnly] private List<GameObject> entities = new();
    
    #endregion
    
    #if UNITY_EDITOR

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
          Debug.Log(link.name);
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
      if (fieldObjectContainer)
      {
        foreach (Transform child in fieldObjectContainer)
        {
          if(child.TryGetComponent<FieldObjectProgress>(out var fieldObjectProgress))
            fieldObjects[child.name] = fieldObjectProgress;
        }
      }
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
      saveModule = SAVE.Current.Node("Stage", true).Node($"Room_{stageIndex}_{roomIndex}", true);
      
      LoadJson(saveModule);
      
      onLoad?.Invoke();
    }

    /// <summary>
    /// 방을 언로딩할 때 데이터를 저장 이후 호출됩니다. <br/>
    /// 이후 방을 Destroy 합니다.
    /// </summary>
    public virtual void Unload()
    {
      Save();
      
      onUnload?.Invoke();
      Destroy(gameObject);
    }
    
    /// <summary>
    /// 플레이어가 방에 들어올 떄 호출됩니다.
    /// </summary>
    protected virtual void Enter()
    {
      foreach (var pair in normalEnemyTable)
      {
        entities.Add(Instantiate(pair.Value, pair.Key.position, Quaternion.identity, entityContainer));
      }
      
      foreach (var obj in fieldObjects)
        obj.Value.OnLoad();

      onEnter?.Invoke();
    }

    /// <summary>
    /// 플레이어가 방에서 나갈 떄 호출됩니다.
    /// </summary>
    protected virtual void Exit()
    {
      foreach (var entity in entities.Where(entity => entity))
        Destroy(entity);
      
      entities.Clear();
      
      foreach (var obj in fieldObjects)
        obj.Value.OnUnLoad();
      
      onExit?.Invoke();
    }
    
    #endregion
    
    #region Serialization

    public void LoadJson(JObject json)
    {
      var objects = json.Get<JObject>(nameof(fieldObjects), null);
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