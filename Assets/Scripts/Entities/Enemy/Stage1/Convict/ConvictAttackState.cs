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
            owner.lastAttackTime = 0;
            owner.Animator.SetBool(EnemyAnimationString.Attack, true);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!owner.Animator.GetBool(EnemyAnimationString.Attack))
            {
                owner.FSM.ChangePattern(owner.FSM.IdleState);
            }
        }
    }
}