using System;
using TMPro;
using ToB.Core.InputManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ToB.UI.BookPanels
{
    public class KeyRebindButton : MonoBehaviour
    {
        enum BindType
        {
            Button,
            Vector2
        }
        [SerializeField] private InputActionReference inputActionReference;
        [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;
        [SerializeField] BindType bindType;
        [SerializeField] Vector2 bindDirection;
        [SerializeField] private TextMeshProUGUI keyText;
        
        Button button;
        InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        int bindingIndex;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        void OnEnable()
        {
            ShowBindKey();
        }
        
        private void ShowBindKey()
        {
            int index = 0;
            string bindKey = "None";
            
            if (bindType == BindType.Vector2)
            {
                var bindings = inputActionReference.action.bindings;
                
                string partName = bindDirection switch
                {
                    Vector2 v when v == Vector2.up => "up",
                    Vector2 v when v == Vector2.down => "down",
                    Vector2 v when v == Vector2.left => "left",
                    Vector2 v when v == Vector2.right => "right",
                    _ => null
                };
                
                for (int i = 0; i < bindings.Count; i++)
                {
                    var binding = bindings[i]; ;
                    if (binding.isPartOfComposite && binding.name == partName)
                    {
                        bindKey = inputActionReference.action.GetBindingDisplayString(i, displayStringOptions);
                        index = i;
                        break;
                    }
                }

                keyText.text = bindKey;
                bindingIndex = index;
            }
            else
            {
                keyText.text = inputActionReference.action.GetBindingDisplayString(index, displayStringOptions);
            }
        }

        public void StartRebind()
        {
            InputAction action = inputActionReference.action;
            
            action.Disable();
            button.interactable = false;
            keyText.text = "_";

            rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    button.interactable = true;

                    action.Enable();
                    TOBInputManager.Instance.KeyBindManager.SaveCurrentKeySettings();
                    ShowBindKey();
                });
            
            rebindingOperation.Start();
        }

        private void OnDisable()
        {
            rebindingOperation?.Dispose();
            rebindingOperation = null;
        }
    }
}
