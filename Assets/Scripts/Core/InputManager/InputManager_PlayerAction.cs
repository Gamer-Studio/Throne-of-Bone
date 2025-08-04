using ToB.Player;
using ToB.Scenes.Stage;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class TOBInputManager
    {
        public void PlayerMove(InputAction.CallbackContext context)
        {
            if (!CanMove()) return;
            player?.Move(context);
        }

        public void PlayerMeleeAttack(InputAction.CallbackContext context)
        {
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState == GameState.CutScene
                && context.performed)
            {
                StageManager.Instance.cutSceneProcessCall = true;
            }
            
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
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState == GameState.UI || StageManager.Instance.CurrentState == GameState.Dialog)
            {
                UIProcess(context); // 같은 키 할당됨
                return;
            }
            else if (StageManager.Instance.CurrentState == GameState.CutScene
                     && context.performed)
            {
                StageManager.Instance.cutSceneProcessCall = true;
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
