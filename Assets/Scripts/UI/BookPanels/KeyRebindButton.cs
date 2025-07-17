using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        void OnEnable()
        {
            ShowBindKey();
        }
        
        private void ShowBindKey()
        {
            int bindingIndex = 0;
            string bindKey = "None";
            
            if (bindType == BindType.Vector2)
            {
                var bindings = inputActionReference.action.bindings;
                
                // Composite의 part name (ex. "up", "down", etc.)
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
                    var binding = bindings[i];
                    Debug.Log(binding.name);
                    if (binding.isPartOfComposite && binding.name == partName)
                    {
                        bindKey = inputActionReference.action.GetBindingDisplayString(i, displayStringOptions);
                        break;
                    }
                }

                keyText.text = bindKey;
            }
            else
            {
                keyText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex, displayStringOptions);
            }
        }
    }
}
