using System;
using Cinemachine;
using NaughtyAttributes;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Player;
using ToB.Utils;
using ToB.Utils.Singletons;
using UnityEngine;

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
        [Label("플레이어"), Tooltip("현재 활성화된 Player 태그가 붙은 플레이어 캐릭터입니다.")]
        public PlayerCharacter player;

        [field: Label("스테이지 인덱스"), Tooltip("현재 스테이지 인덱스입니다."), SerializeField, ReadOnly]
        public int CurrentStageIndex { get; private set; } = 1;

        [field: Label("방 인덱스"), Tooltip("현재 방의 인덱스입니다."), SerializeField, ReadOnly]
        public int CurrentRoomIndex { get; private set; } = 1;

        [Tooltip("시네머신 Confiner 용 콜라이더입니다."), SerializeField]
        private CompositeCollider2D confinerBorder;
        [Label("로딩된 Confiner 콜라이더 목록"), SerializeField]
        private SerializableDictionary<Collider2D, GameObject> loadedColliders = new();

        [SerializeField] private GameObject tempObj;
        [SerializeField] private CinemachineConfiner2D confiner;
        [SerializeField] private Transform roomContainer;
        [Header("카메라")]
        [field:SerializeField] public Camera MainCamera { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera MainVirtualCamera{ get; private set; }

        [field: SerializeField] public GameState State { get; private set; } = GameState.Play;

        #region Unity Event

#if UNITY_EDITOR

        private void Reset()
        {
            player = PlayerCharacter.GetInstance();
            if (!confiner) confiner = FindAnyObjectByType<CinemachineConfiner2D>();
        }

#endif

        private void Awake()
        {
            if (!player)
            {
                player = PlayerCharacter.GetInstance();
            }
            
            if(tempObj) tempObj.SetActive(false);
        }

        private void Start()
        {
            InputManager.Instance.player = FindAnyObjectByType<PlayerController>();
            if(!InputManager.Instance.player)
                Debug.LogError("플레이어가 씬에 없습니다.");
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

            if (Camera.main)
            {
                confiner.InvalidateCache();
            }

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

            if (Camera.main)
            {
                confiner.InvalidateCache();
            }
        }

        private bool unloaded = false;

        private void OnDestroy()
        {
            unloaded = true;
        }


        public void ChangeGameState(GameState state)
        {
            State = state;
        }

        #endregion
    }
}