using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.UI
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        public abstract void Process(InputAction.CallbackContext context);
        public abstract void Cancel(InputAction.CallbackContext context);

    }
}