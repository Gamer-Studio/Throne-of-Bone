using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using ToB.Entities;
using ToB.Entities.FieldObject;
using ToB.IO;
using ToB.Scenes.Stage;
using ToB.Utils;
using ToB.World.Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room")]
  public class Room : MonoBehaviour, IJsonSerializable
  {
    #region State
    private const string State = "State";
    
    [Label("스테이지 인덱스"), Foldout(State)] public int stageIndex;
    [Label("방 인덱스"), Foldout(State)] public int roomIndex;
    [Label("데이터 모듈"), Foldout(State), SerializeField] public SAVEModule saveModule = null;
    [Label("일반 적 소환 정보"), Foldout(State)] public SerializableDictionary<Transform, AssetReference> normalEnemyTable = new();
    [Label("오브젝트"), Foldout(State)] public SerializableDictionary<string, FieldObjectProgress> fieldObjects = new();
    [Label("모닥불 목록"), Foldout(State)] public List<Bonfire> bonfires = new();
    [Label("인스턴스된 적"), Foldout(State), SerializeField, ReadOnly] private SerializableDictionary<Transform, Enemy> enemies = new();
    [Label("생성형 구조물"), Foldout(State)] public List<Structure> structures = new();
    [SerializeField] private List<Enemy> enemiesList = new();
    
    #endregion
    
    #region Binding
    private const string Binding = "Binding"; 

#if UNITY_EDITOR
    [ContextMenuItem("내부 링크 찾기", nameof(FindLinks))]
#endif
    [Foldout(Binding)] public List<RoomLink> links = new();
    [Foldout(Binding), SerializeField] private Transform entityContainer; 
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
    public void FindStructures()
    {
      Undo.RecordObject(this, nameof(FindStructures));
      
      fieldObjects.Clear();
      bonfires.Clear();
      
      FindStructure(transform);
      FindLinks();
      
      EditorUtility.SetDirty(this);
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
          
          Undo.RecordObject(structure, nameof(FindStructure));
          EditorUtility.SetDirty(structure);
          
          if(structure is Bonfire bonfire)
            bonfires.Add(bonfire);
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
          link.Init(this);
          
          Undo.RecordObject(link, nameof(FindLink));
          EditorUtility.SetDirty(link);
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

    [SerializeField] private Vector3 pos;

    private void OnDrawGizmos()
    {
      if (Background && Background.tilemap)
      {
        var tilemap = Background.tilemap;
        var bounds = tilemap.cellBounds;
        var centerCell = (bounds.min + bounds.max) / 2;

        // 타일맵의 중심을 월드 좌표로 변환
        pos = bounds.position + transform.position;

        // 부모 오브젝트 기준 보정 (tilemap이 자식이므로 위치 보정 필요 없음)
        Handles.Label(pos, $"{stageIndex} Stage / {roomIndex} Room");

      }
    }

#endif

    protected virtual void Awake()
    {
      if (Background)
      {
        Background.onEnter.AddListener(Enter);
        Background.onExit.AddListener(Exit);
      }
    }

    protected virtual void Start()
    {
      Load();
    }

    protected virtual void OnEnable()
    {
      StageManager.Instance?.AddCameraCollision(Background.CameraCollider);
    }

    private void OnDisable()
    {
      StageManager.Instance?.RemoveCameraCollision(Background.CameraCollider);
    }

    private void OnDestroy()
    {
      Unload();
    }

    #endregion
    
    #region Feature

    /// <summary>
    /// 링크로 연결되어있는 방들을 가져옵니다.
    /// </summary>
    /// <param name="onlyActive">참일 경우 활성화된 방만 가져옵니다.</param>
    public List<Room> GetLinkedRooms(bool onlyActive)
    {
      var result = new List<Room>();

      foreach (var link in links)
      {
        if (link.IsLoaded || !onlyActive)
        {
          result.Add(link.connectedRoom);
        }
      }
      
      return result;
    }

    /// <summary>
    /// Root/Stage/Room_{stageIndex}_{roomIndex} 경로로
    /// 방 데이터를 명시적으로 저장합니다.
    /// </summary>
    public void Save()
    {
      try
      {
        var data = ToJson();
        saveModule.Read(data);
      }
      catch (NullReferenceException)
      {
        DebugSymbol.Save.Log($"Error while saving room data: {stageIndex} / {roomIndex}");
      }
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
        
        foreach (var pair in normalEnemyTable)
        {
          if(pair.Key == null || pair.Value == null || (enemies.ContainsKey(pair.Key) && enemies[pair.Key].IsAlive)) continue;
        
          var enemy = (Enemy)pair.Value.Pooling();
        
          enemy.transform.SetParent(entityContainer);
          enemy.transform.position = pair.Key.position;
          
          enemies[pair.Key] = enemy;
          enemiesList.Add(enemy);
        }
      }

      onLoad?.Invoke();
    }

    /// <summary>
    /// 방을 언로딩할 때 데이터를 저장 이후 호출됩니다. <br/>
    /// 이후 방을 Destroy 합니다.
    /// </summary>
    public virtual void Unload()
    {
      foreach (var enemy in enemiesList)
        enemy.Release();
      
      enemies.Clear();
      
      Save();
      
      onUnload?.Invoke();
    }
    
    /// <summary>
    /// 플레이어가 방에 들어올 떄 호출됩니다.
    /// </summary>
    protected virtual void Enter()
    {
      foreach (var obj in fieldObjects)
        obj.Value.OnLoad();

      onEnter?.Invoke();
      StageManager.Instance.onRoomEnter.Invoke(this);
      StageManager.RoomController.currentRoom = this;
    }

    /// <summary>
    /// 플레이어가 방에서 나갈 떄 호출됩니다.
    /// </summary>
    protected virtual void Exit()
    {
      // foreach (var pair in enemies)
      // {
      //   if (!pair.Value.IsAlive)
      //   {
      //     pair.Value.Release();
      //     enemies.Remove(pair.Key);
      //   }
      // }

      foreach (var obj in fieldObjects)
        obj.Value.OnUnLoad();
      
      onExit?.Invoke();
      if(StageManager.HasInstance) StageManager.Instance.onRoomExit.Invoke(this);
    }
    
    #endregion
    
    #region Serialization
    
    /// <summary>
    /// 방 로딩 전 호출됩니다.
    /// </summary>
    /// <param name="json"></param>
    public void LoadJson(JObject json)
    {
      var dummy = new JObject();
      var objects = json.Get(nameof(fieldObjects), dummy);
      var structureData = json.Get(nameof(structures), dummy);

      foreach (var pair in fieldObjects)
      {
        pair.Value.room = this;
        pair.Value.LoadJson(objects.Get(pair.Key, dummy));
      }

      foreach (var (key, token) in structureData)
      {
        var data = token.Get(dummy);

        if (data != dummy)
        {
          var obj = Structure.Spawn(this, data.Get("PrefabName", "dummy"));
          obj.name = key;
          obj.LoadJson(data);
          var objPosition = data.Get("position", Vector3.zero);
          obj.transform.localPosition = objPosition;
          obj.room = this;
        }
      }
    }

    /// <summary>
    /// 방 저장 전 호출됩니다.
    /// </summary>
    /// <returns></returns>
    public JObject ToJson()
    {
      var objects = new JObject();
      var result = new JObject { };
      
      foreach (var pair in fieldObjects)
      {
        objects[pair.Key] = pair.Value.ToJson();
      }
      
      var structureData = new JObject();
      
      foreach (var structure in structures)
        structureData[structure.name] = structure.ToJson();

      result[nameof(fieldObjects)] = objects;
      result[nameof(structures)] = structureData;
      
      return result;
    }
    
    /// <summary>
    /// 다른 방의 오브젝트 데이터에 접근합니다.
    /// </summary>
    /// <param name="stageIndex">받아올 데이터가 있는 스테이지입니다.</param>
    /// <param name="roomIndex">받아올 데이터가 있는 방입니다.</param>
    /// <param name="objectName">받아올 데이터의 오브젝트 이름입니다.</param>
    /// <returns></returns>
    public static JObject GetData(int stageIndex, int roomIndex, string objectName)
    {
      if (StageManager.RoomController.loadedRooms.TryGetValue($"Stage{stageIndex}/Room{roomIndex}", out var room))
      {
        // 방이 로딩되어있을경우
        if (room.fieldObjects.TryGetValue(objectName, out var obj) && obj)
          return obj.ToJson();
        else return JsonUtil.Blank;
      }
      else
      {
        // 방이 로딩되어있지 않을 경우
        var node = SAVE.Current.Node("Stage", true).Node($"Room_{stageIndex}_{roomIndex}", true);
        return node.Get(nameof(fieldObjects), JsonUtil.Blank).Get(objectName, JsonUtil.Blank);
      }
    }

    /// <summary>
    /// 다른 방의 오브젝트 데이터를 설정할 수 있습니다.
    /// </summary>
    /// <param name="stageIndex">설정할 데이터의 스테이지입니다.</param>
    /// <param name="roomIndex">설정할 데이터의 방입니다.</param>
    /// <param name="objectName">설정할 데이터의 오브젝트 이름입니다.</param>
    /// <param name="data">저장할 데이터입니다.</param>
    public static void SetData(int stageIndex, int roomIndex, string objectName, JObject data)
    {
      if (StageManager.RoomController.loadedRooms.TryGetValue($"Stage{stageIndex}/Room{roomIndex}", out var room))
      {
        // 방이 로딩되어있을경우
        if (room.fieldObjects.TryGetValue(objectName, out var obj) && obj)
        {
          obj.LoadJson(data);
        }
      }
      else
      {
        // 방이 로딩되어있지 않을 경우
        var node = SAVE.Current.Node("Stage", true).Node($"Room_{stageIndex}_{roomIndex}", true);

        var roomData = node.Get(nameof(fieldObjects), JsonUtil.Blank);
        roomData[objectName] = data;
        node[nameof(fieldObjects)] = roomData;
      }
    }
    
    #endregion
  }
}