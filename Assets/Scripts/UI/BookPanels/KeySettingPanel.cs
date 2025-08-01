using System;
using TMPro;
using ToB.Core.InputManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.UI.BookPanels
{
    public class KeySettingPanel : MonoBehaviour
    {
        private PlayerInput playerInput;
        private InputActionMap actionAsset;
        
        [SerializeField] private TextMeshProUGUI upButtonText;
        [SerializeField] private TextMeshProUGUI downButtonText;
        [SerializeField] private TextMeshProUGUI leftButtonText;
        [SerializeField] private TextMeshProUGUI rightButtonText;
        [SerializeField] private TextMeshProUGUI meleeButtonText;
        [SerializeField] private TextMeshProUGUI rangedButtonText;
        [SerializeField] private TextMeshProUGUI guardButtonText;
        [SerializeField] private TextMeshProUGUI interactionButtonText;

        private void OnEnable()
        {
            if (!playerInput) playerInput = TOBInputManager.Instance.PlayerInput;
            actionAsset ??= playerInput.currentActionMap;
        }
    }
}
