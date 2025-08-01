using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Entities.NPC;
using ToB.IO;
using ToB.Player;
using ToB.UI;
using ToB.Utils;
using ToB.Utils.Singletons;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Scenes.Stage
{
  public enum GameState
  {
    Play,
    UI,
    Dialog
  }

  public class StageManager : ManualSingleton<StageManager>
  {
    #region State
    private const string State = "State";

    [Label("플레이어"), Tooltip("현재 활성화된 Player 태그가 붙은 플레이어 캐릭터입니다."), Foldout(State)] public PlayerCharacter player;
    [Label("현재 플레이어가 있는 방"), Foldout(State)] public Room currentRoom;
    [field: Foldout(State), SerializeField] public GameState CurrentState { get; private set; } = GameState.Play;
    [SerializeField, ReadOnly] private bool unloaded;

    public int CurrentStageIndex => currentRoom.stageIndex;
    public int CurrentRoomIndex => currentRoom.roomIndex;

    #endregion

    #region Binding
    private const string Binding = "Binding";

    [Label("카메라 틀 콜라이더"), Tooltip("시네머신 Confiner 용 콜라이더입니다."), Foldout(Binding), SerializeField] private CompositeCollider2D confinerBorder;
    [Label("로딩된 Confiner 콜라이더 목록"), Foldout(Binding), SerializeField] private SerializableDictionary<Collider2D, GameObject> loadedColliders = new();
    [Label("시네머신 오류방지용 임시 오브젝트"), Foldout(Binding), SerializeField] private GameObject tempObj;
    [Foldout(Binding), SerializeField] private CinemachineConfiner2D confiner;
    [Foldout(Binding), SerializeField] private Transform roomContainer;
    [field: Foldout(Binding), SerializeField] public Camera MainCamera { get; private set; }
    [field: Foldout(Binding), SerializeField] public CinemachineVirtualCamera MainVirtualCamera { get; private set; }
    [Foldout(Binding)] public List<Room> loadedRooms = new();

    #endregion

    #region Event

    /// <summary>
    /// 플레이어가 방에 입장시 이벤트
    /// </summary>
    public UnityEvent<Room> onRoomEnter = new();

    /// <summary>
    /// 플레이어가 방에서 나갔을 때 이벤트. <br />
    /// </summary>
    public UnityEvent<Room> onRoomExit = new();

    public void InvokeRoomExit(Room room)
    {
      if(!unloaded) onRoomExit.Invoke(room);
    }

    #endregion

    #region Unity Event

#if UNITY_EDITOR

    private void Reset()
    {
      player = PlayerCharacter.Instance;
      if (!confiner) confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

#endif

    private void Awake()
    {
      if (!player) player = PlayerCharacter.Instance;

      if (tempObj) tempObj.SetActive(false);

      if (SAVE.Current != null) {
        // 임시 플레이어 소환 코드
        // TODO 방별 로딩형태 전환시 개편 필요

        int stageIndex = SAVE.Current.Player.currentStage,
          roomIndex = SAVE.Current.Player.currentRoom;
        var playerPos = SAVE.Current.Player.savedPosition;

        if (stageIndex != 0 && roomIndex != 0)
        {
          foreach (var room in loadedRooms)
          {
            Debug.Log(room.name);
            if (room.stageIndex != stageIndex || room.roomIndex != roomIndex) continue;
            
            currentRoom = room;
            player.transform.position = room.transform.TransformPoint(playerPos);
            
            break;
          }
        }
      }
    }

    private void Start()
    {
      InputManager.Instance.player = FindAnyObjectByType<PlayerController>();
      if (!InputManager.Instance.player)
        DebugSymbol.Editor.Log("플레이어가 씬에 없습니다.");
      // InputManager.Instance.SetActionMap(InputActionMaps.Player);
    }

    #endregion

    #region Feature

    public void AddCameraCollision(Collider2D coll)
    {
      if (unloaded) return;
      if (loadedColliders.ContainsKey(coll)) return;
      if (!player || player.stat.Hp == 0) return;

      var obj = loadedColliders[coll] = new GameObject(coll.name);
      obj.transform.SetParent(confinerBorder.transform);
      obj.transform.position = coll.transform.position;
      obj.transform.localScale = coll.transform.localScale;

      switch (coll)
      {
        case BoxCollider2D box:
        {
          var collider = obj.AddComponent<BoxCollider2D>();
          collider.size = box.size;
          collider.transform.localScale = collider.transform.localScale.X(v => v * roomContainer.localScale.x)
            .Y(v => v * roomContainer.localScale.y);

          collider.offset = box.offset;
          collider.compositeOperation = Collider2D.CompositeOperation.Merge;

          break;
        }
      }

      if (Camera.main) confiner.InvalidateCache();

      if (Camera.main) confiner.ValidateCache(Camera.main.orthographicSize);
    }

    public void RemoveCameraCollision(Collider2D coll)
    {
      if (unloaded) return;
      if (!player || player.stat.Hp == 0) return;

      if (loadedColliders.TryGetValue(coll, out var obj))
      {
        Destroy(obj);
        loadedColliders.Remove(coll);
      }

      if (Camera.main) confiner.InvalidateCache();
    }
    
    private void OnDestroy()
    {
      unloaded = true;
    }

    public void ChangeGameState(GameState state)
    {
      player.IsMoving = false;
      CurrentState = state;
    }

    #endregion

    #region Dialog
    
    public void BeginDialog(NPCBase npc)
    {
      player.IsMoving = false;
      ChangeGameState(GameState.Dialog);
      UIManager.Instance.panelStack.Push(npc.DialogPanel);
      GameCameraManager.Instance.SetBlendTime(0.5f);
    }
    
    #endregion
    }
}