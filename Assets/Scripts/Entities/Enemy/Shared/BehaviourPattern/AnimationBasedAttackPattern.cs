using System;
using UnityEngine;

namespace ToB.Entities
{
    public class AnimationBasedAttackPattern:EnemyPattern
    {
        public AnimationBasedAttackPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetTrigger(EnemyAnimationString.Attack);
        }
        public override void Execute()
        {
            base.Execute();
            
            // if(enemy.Animator.GetBool(EnemyAnimationString.AttackEnd))
            //     Debug.Log("AttackEnd!");
        }
    }
}