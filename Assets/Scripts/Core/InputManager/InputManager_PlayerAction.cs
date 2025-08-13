using System;
using ToB.Player;
using ToB.Scenes.Stage;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    
    public partial class TOBInputManager
    {
        public event Action anyInteractionKeyAction;
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
            if (context.performed)
            {
                anyInteractionKeyAction?.Invoke();
            }
            
            if (!StageManager.Instance) return;
            if (StageManager.Instance.CurrentState == GameState.UI || StageManager.Instance.CurrentState == GameState.Dialog)
            {
                UIProcess(context); // 같은 키 할당됨
                return;
            }
            else if (StageManager.Instance.CurrentState == GameState.CutScene
                     && context.performed)
            {
                // 스테이지 매니저에 굳이 이 필드를 만들어놓고 잊고 위에 같은 목적으로 액션도 걸어서 번잡해졌습니다. 개발 일정 관계상 이대로 둡니다. (승화) 
                StageManager.Instance.cutSceneProcessCall = true;       
            }
            
            else if(StageManager.Instance.CurrentState == GameState.Play && context.performed)
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
