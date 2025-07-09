using ToB.Core;

namespace ToB.UI
{
    // UI 조작에 반응해 다이얼로그 매니저의 메소드를 다뤄줄 뿐인 대리인
    public class DialogUI:UIPanelBase
    {
        public override void Process()
        {
            DialogManager.Instance.ProcessNPC();
        }

        public override void Cancel()
        {
            DialogManager.Instance.CancelDialog();
        }
    }
}