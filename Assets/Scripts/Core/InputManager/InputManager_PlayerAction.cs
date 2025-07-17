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
            Debug.Log("PlayerMove!");
            if (!CanMove()) return;
            player?.Move(context);
        }

        public void PlayerMeleeAttack(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.MeleeAttack(context);
        }

        public void PlayerJump(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.Jump(context);
        }

        public void PlayerDash(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.Dash(context);
        }

        public void PlayerRangedAttack(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.RangedAttack(context);
        }

        public void PlayerInteract(InputAction.CallbackContext context)
        {
            if (!CanMove())
            {
                UIProcess(context); // 같은 키 할당됨
                return;
            }
            
            if(context.performed)
                player?.Interact();
        }

        public void PlayerBlock(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.Block(context);
        }

        public bool CanMove()
        {
            return StageManager.Instance && StageManager.Instance.CurrentState == GameState.Play;
        }
    }
}
