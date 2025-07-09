using UnityEngine;

namespace ToB.UI
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        public abstract void Process();
        public abstract void Cancel();

    }
}