using ToB.Player;
using ToB.Scenes.Stage;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class InputManager
    {
        public void PlayerMove(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.Move(context);
        }

        public void PlayerMeleeAttack(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.MeleeAttack(context);
        }

        public void PlayerJump(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.Jump(context);
        }

        public void PlayerDash(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.Dash(context);
        }

        public void PlayerRangedAttack(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.RangedAttack(context);
        }

        public void PlayerInteract(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            
            if(context.performed)
                player?.Interact();
        }

        public void PlayerBlock(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState != GameState.Play) return;
            player?.Block(context);
        }
    }
}
