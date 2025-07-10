using ToB.Core;
using UnityEngine.InputSystem;

namespace ToB.UI
{
    // UI 조작에 반응해 다이얼로그 매니저의 메소드를 다뤄줄 뿐인 대리인
    public class DialogUI:UIPanelBase
    {
        public override void Process(InputAction.CallbackContext context)
        {
            if(context.performed)
                DialogManager.Instance.ProcessNPC();
        }

        public override void Cancel(InputAction.CallbackContext context)
        {
            if(context.performed)
              DialogManager.Instance.CancelDialog();
        }
    }
}