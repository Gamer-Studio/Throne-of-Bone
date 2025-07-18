using System;
using ToB.Player;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    public partial class InputManager : DDOLSingleton<InputManager>
    {
        [field:SerializeField] public KeyBindManager KeyBindManager { get; private set; }
        public PlayerInput PlayerInput { get; private set; }

        public PlayerController player;
        
        [field:SerializeField] public string CurrentActionMap { get; set; }

        public static bool IsRebinding = false;

        protected override void Awake()
        {
            base.Awake();
            PlayerInput = GetComponent<PlayerInput>();
            
        }

        private void Start()
        {
            SetActionMap(InputActionMaps.Player);
            KeyBindManager.LoadKeySettings();
        }

        public void SetActionMap(InputActionMaps map)
        {
            if (map == InputActionMaps.NULL) CurrentActionMap = null;
            else CurrentActionMap = map.ToString();
            
            
            PlayerInput.SwitchCurrentActionMap(CurrentActionMap);
        }

        public void SetInputActive(bool isActive)
        {
            PlayerInput.enabled = isActive;
        }
    }
}
