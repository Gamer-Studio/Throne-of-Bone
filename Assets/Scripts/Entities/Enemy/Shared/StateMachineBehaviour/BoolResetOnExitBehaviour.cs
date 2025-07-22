using UnityEngine;

namespace ToB.Entities
{
    public class BoolResetOnExitBehaviour : StateMachineBehaviour
    {
        [Tooltip("종료 시 false로 바꿀 Animator bool 파라미터 이름")]
        public string parameterName;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!string.IsNullOrEmpty(parameterName))
            {
                animator.SetBool(parameterName, false);
            }
        }
    }
}