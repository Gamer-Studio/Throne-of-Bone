using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room")]
  public class Room : MonoBehaviour
  {
    [Label("일반 적 소환 정보")] public SerializableDictionary<Vector2, GameObject> normalEnemyTable = new();
    [ContextMenuItem("내부 링크 찾기", nameof(FindLinks))]

    #region Binding

    [Foldout("Binding")] public List<RoomLink> links = new();
    [Foldout("Binding"), SerializeField] private Transform entityContainer; 
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
    }
    
    #endif

    private void Awake()
    {
      if (Background)
      {
        Background.onEnter.AddListener(OnEnterRoom);
        Background.onExit.AddListener(OnExitRoom);
      }
    }

    private void OnEnable()
    {
      StageManager.Instance?.AddCameraCollision(Background.backgroundCollider);
      onLoad?.Invoke();
    }

    private void OnDisable()
    {
      StageManager.Instance?.RemoveCameraCollision(Background.backgroundCollider);
      onUnload?.Invoke();
    }

    #endregion
    
    #region Feature

    private void OnEnterRoom()
    {
      onEnter?.Invoke();
      foreach (var pair in normalEnemyTable)
      {
        entities.Add(Instantiate(pair.Value, pair.Key, Quaternion.identity, entityContainer));
      }
    }

    private void OnExitRoom()
    {
      foreach (var entity in entities.Where(entity => entity))
        Destroy(entity);
      
      entities.Clear();
      
      onExit?.Invoke();
    }
    
    #endregion
  }
}