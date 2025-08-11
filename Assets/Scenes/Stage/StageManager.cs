using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using NaughtyAttributes;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Entities.FieldObject;
using ToB.Entities.NPC;
using ToB.IO;
using ToB.IO.SubModules;
using ToB.IO.SubModules.SavePoint;
using ToB.Player;
using ToB.UI;
using ToB.Utils;
using ToB.Utils.Singletons;
using ToB.World;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using AudioType = ToB.Core.AudioType;

namespace ToB.Scenes.Stage
{
  public enum GameState
  {
    Play,
    UI,
    Dialog,
    CutScene
  }

  [RequireComponent(typeof(RoomController))]
  public class StageManager : ManualSingleton<StageManager>
  {
    public static RoomController RoomController => Instance.roomController;
    
    #region State
    private const string State = "State";

    [Label("플레이어"), Tooltip("현재 활성화된 Player 태그가 붙은 플레이어 캐릭터입니다."), Foldout(State)] public PlayerCharacter player;
    [field: Foldout(State), SerializeField] public GameState CurrentState { get; private set; } = GameState.Play;
    [SerializeField, ReadOnly] private bool unloaded;

    public bool cutSceneProcessCall = false;

    public int CurrentStageIndex => roomController.currentRoom.stageIndex;
    public int CurrentRoomIndex => roomController.currentRoom.roomIndex;

    public bool isTestScene = false;

    #endregion

    #region Binding
    private const string Binding = "Binding";

    [Label("카메라 틀 콜라이더"), Tooltip("시네머신 Confiner 용 콜라이더입니다."), Foldout(Binding), SerializeField] private CompositeCollider2D confinerBorder;
    [Label("로딩된 Confiner 콜라이더 목록"), Foldout(Binding), SerializeField] private SerializableDictionary<Collider2D, GameObject> loadedColliders = new();
    [Label("시네머신 오류방지용 임시 오브젝트"), Foldout(Binding), SerializeField] private GameObject tempObj;
    [field: Foldout(Binding), SerializeField] public CinemachineVirtualCamera MainVirtualCamera { get; private set; }
    [Foldout(Binding)] public RoomController roomController;
    [Foldout(Binding), SerializeField] private CinemachineConfiner2D confiner;
    [Foldout(Binding), SerializeField] private Transform roomContainer;
    [Foldout(Binding), SerializeField] private Camera mainCamera;

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

    #endregion

    #region Unity Event

#if UNITY_EDITOR

    private void Reset()
    {
      player = PlayerCharacter.Instance;
      confiner = FindAnyObjectByType<CinemachineConfiner2D>();
      roomController = GetComponent<RoomController>();
    }

#endif

    private void Awake()
    {
      // 필드 초기화
      {
        if (!player) player = PlayerCharacter.Instance;
        if (!confiner) confiner = FindAnyObjectByType<CinemachineConfiner2D>();
        if (!roomController) roomController = GetComponent<RoomController>();
        if (tempObj) tempObj.SetActive(false);
      }

      if (isTestScene) return;
      // 시작 방 로딩
      var savedInfo = SAVE.Current.SavePoints.GetLastSavePoint();

      if (!savedInfo.Equals(SavePointData.Default))
      {
        // 저장기록이 있을 경우 마지막 저장지점에서 소환
        savedInfo.Teleport();
      }
      else
      {
        // 저장기록이 없을 경우 초기 지점에서 소환
        var room = RoomController.LoadRoom(1, 1, true);
        player.transform.position = room.transform.position.X(v => v + 12).Y(v => v - 11);
      }
    }

    private void Start()
    {
      TOBInputManager.Instance.player = FindAnyObjectByType<PlayerController>();
      if (!TOBInputManager.Instance.player)
        DebugSymbol.Editor.Log("플레이어가 씬에 없습니다.");
      
      UIManager.Instance?.gamePlayUI.InitGages();
      UIManager.Instance?.InitsAfterStageAwake();
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

    public void UpdateRoomData()
    {
      var loadedRooms = roomController.loadedRooms;
      
    }

    public static void Save(Bonfire bonfire = null)
    {
      if (!HasInstance) return;
      
      foreach (var pair in RoomController.loadedRooms.Where(pair => pair.Value)) pair.Value.Save();
      
      if(bonfire != null)
        SAVE.Current.SavePoints.UpdateSavePoint(bonfire);
      
      SAVE.Current.Save();
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

    public void EndGame()
    {
      SceneManager.LoadScene("Outro");
    }
  }
}