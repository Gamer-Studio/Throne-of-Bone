using ToB.Player;
using ToB.UI;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class InputManager
    {
        public void UIProcess(InputAction.CallbackContext context)
        {
            if(context.started)
                UIManager.Instance.panelStack.Peek().Process();
        }
        public void CancelCurrentPanel()
        {
            if (UIManager.Instance.panelStack.Count == 0)
            {
                Debug.Log("열려있는 UI가 없는데 액션 맵이 UI 조작 상태입니다");
                return;
            }
            UIManager.Instance.panelStack.Pop().Cancel();
        }
    }
}
