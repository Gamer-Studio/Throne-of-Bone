using UnityEngine;

namespace ToB.Entities
{
    public class AttackMotionEndBehaviour:StateMachineBehaviour
    {
        public bool isFinal;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
                animator.SetBool(EnemyAnimationString.AttackEnd, false);
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime < 1) return;
            
            if(isFinal || !animator.GetBool(EnemyAnimationString.Attack))
                animator.SetBool(EnemyAnimationString.AttackEnd, true);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyAnimationString.AttackEnd, false);
            
        }
    }
}