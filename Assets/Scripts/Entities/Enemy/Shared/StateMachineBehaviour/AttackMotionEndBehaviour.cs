using UnityEngine;

namespace ToB.Entities
{
    public class AttackMotionEndBehaviour:StateMachineBehaviour
    {
        public bool isFinal;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(isFinal || !animator.GetBool(EnemyAnimationString.Attack))
                animator.SetBool(EnemyAnimationString.AttackEnd, true);
        }
    }
}