using ToB.Player;
using ToB.Scenes.Stage;
using ToB.UI;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class TOBInputManager
    {
        public bool blockUICancel = false;
        public void UIProcess(InputAction.CallbackContext context)
        {
            if (IsRebinding) return;
            if (UIManager.Instance.panelStack.Count == 0) return;

            UIManager.Instance.panelStack.Peek().Process(context);
        }
        public void UICancel(InputAction.CallbackContext context)
        {
            if (IsRebinding) return;
            if (!context.performed) return;
            if (blockUICancel) return;
            
            if (UIManager.Instance.panelStack.Count == 0)
            {
                UIManager.Instance.mainBookUI.SettingUIToggle(context);
                return;
            }

            UIManager.Instance.panelStack.Peek().Cancel(context);
            
        }

        public void StatisticsUIToggle(InputAction.CallbackContext context)
        {
            if (!CanUseUIToggles()) return;
            UIManager.Instance.mainBookUI.StatisticsUIToggle(context);
        }

        public void SoulUIToggle(InputAction.CallbackContext context)
        {
            if (!CanUseUIToggles()) return;
            UIManager.Instance.mainBookUI.SoulUIToggle(context);
        }

        public void SkillUIToggle(InputAction.CallbackContext context)
        {
            if (!CanUseUIToggles()) return;
            UIManager.Instance.mainBookUI.SkillUIToggle(context);
        }

        public void CollectionUIToggle(InputAction.CallbackContext context)
        {
            if (!CanUseUIToggles()) return;
            UIManager.Instance.mainBookUI.CollectionUIToggle(context);
        }
        
        public void WideMapToggle(InputAction.CallbackContext context)
        {
            if (!CanUseUIToggles()) return;
            UIManager.Instance.wideMapUI.WideMapToggle(context);
        }

        public void QuestUIToggle(InputAction.CallbackContext context)
        {

        }

        public bool CanUseUIToggles()
        {
            if (IsRebinding) return false;
            return UIManager.Instance.panelStack.Count == 0 || UIManager.Instance.panelStack.Peek() is MainBookUI;
        }
    }
}
