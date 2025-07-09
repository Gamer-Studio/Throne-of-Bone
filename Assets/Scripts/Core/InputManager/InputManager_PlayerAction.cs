using ToB.Player;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class InputManager
    {
        public void PlayerMove(InputAction.CallbackContext context)
        {
            player?.Move(context);
        }

        public void PlayerInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
                player?.Interact();
        }
    }
}
