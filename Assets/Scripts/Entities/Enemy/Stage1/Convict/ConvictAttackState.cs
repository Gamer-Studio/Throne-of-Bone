using System;
using UnityEngine;

namespace ToB.Entities
{
    public class ConvictAttackState:EnemyPattern
    {
        private readonly Convict owner;
        public ConvictAttackState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Convict;
        }

        public override void Enter()
        {
            base.Enter();
            owner.Animator.SetBool(EnemyAnimationString.Attack, true);
        }

        public override void Execute()
        {
            base.Execute();
            
            // Attack bool 값 변경은 애니메이터에서 StateMachineBehaviour 스크립트가 직접 핸들링합니다
            if (!owner.Animator.GetBool(EnemyAnimationString.Attack))
            {
                owner.FSM.ChangePattern(owner.FSM.IdleState);
            }
        }
    }
}