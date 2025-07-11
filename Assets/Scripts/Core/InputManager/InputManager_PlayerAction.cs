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

        public void PlayerMeleeAttack(InputAction.CallbackContext context)
        {
            player?.MeleeAttack(context);
        }

        public void PlayerJump(InputAction.CallbackContext context)
        {
            player?.Jump(context);
        }

        public void PlayerDash(InputAction.CallbackContext context)
        {
            player?.Dash(context);
        }

        public void PlayerRangedAttack(InputAction.CallbackContext context)
        {
            player?.RangedAttack(context);
        }

        public void PlayerInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
                player?.Interact();
        }

        public void PlayerBlock(InputAction.CallbackContext context)
        {
            player?.Block(context);
        }
    }
}
