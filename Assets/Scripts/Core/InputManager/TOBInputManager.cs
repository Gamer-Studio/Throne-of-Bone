using System;
using ToB.Player;
using ToB.Utils;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace ToB.Core.InputManager
{
    public enum InputActionMaps
    {
        Player,
        UI,
        Stage,
        MainMenu,
        NULL
    }
    
    public partial class TOBInputManager : DDOLSingleton<TOBInputManager>
    {
        [field:SerializeField] public KeyBindManager KeyBindManager { get; private set; }
        public PlayerInput PlayerInput { get; private set; }

        public PlayerController player;
        
        /// <summary>
        /// 현재 액션 맵을 표시하고, 변경할 수 있습니다.
        /// </summary>
        [SerializeField, GetSet(nameof(CurrentActionMap))] private InputActionMaps currentActionMap;
        public InputActionMaps CurrentActionMap
        {
            get => currentActionMap;
            set => SetActionMap(value);
        }

        public static bool IsRebinding = false;

        protected override void Awake()
        {
            base.Awake();
            PlayerInput = GetComponent<PlayerInput>();
        }
        

        private void Start()
        {
            PlayerInput.enabled = true;
            SetActionMap(InputActionMaps.Player);
            KeyBindManager.LoadKeySettings();
        }

        public void SetActionMap(InputActionMaps map)
        {
            currentActionMap = map;

            PlayerInput.SwitchCurrentActionMap(currentActionMap.ToString());
        }

        public void SetInputActive(bool isActive)
        {
            PlayerInput.enabled = isActive;
        }
    }
}
