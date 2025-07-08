using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core
{
    public enum InputActionMaps
    {
        Player,
        UI,
        Stage,
        MainMenu
    }
    
    public class InputManager : DDOLSingleton<InputManager>
    {
        private PlayerInput playerInput;
        
        [field:SerializeField] public string CurrentActionMap { get; set; }
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            SetActionMap(InputActionMaps.Player);
        }

        public void SetActionMap(InputActionMaps map)
        {
            CurrentActionMap = map.ToString();
            playerInput.SwitchCurrentActionMap(CurrentActionMap);
        }
    }
}
