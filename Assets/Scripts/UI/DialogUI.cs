using ToB.Core;

namespace ToB.UI
{
    public class DialogUI:UIPanelBase
    {
        public override void Cancel()
        {
            DialogManager.Instance.CancelDialog();
        }
    }
}