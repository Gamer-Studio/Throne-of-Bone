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
        private PlayerInput playerInput;

        public PlayerController player;
        
        [field:SerializeField] public string CurrentActionMap { get; set; }
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            SetActionMap(InputActionMaps.Player);
        }

        public void SetActionMap(InputActionMaps map)
        {
            if (map == InputActionMaps.NULL) CurrentActionMap = null;
            else CurrentActionMap = map.ToString();
            
            playerInput.SwitchCurrentActionMap(CurrentActionMap);
        }

    }
}
