using System;

using UnityEngine;

namespace ToB.Entities
{
    public class MutantRatSleepState : EnemyPattern
    {
        private readonly MutantRat owner;
        private readonly MutantRatFSM fsm;
        
        private static readonly int SLEEP_STATE_HASH = Animator.StringToHash("Base Layer.Sleep");
        
        public MutantRatSleepState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as MutantRat;
            fsm = strategy as MutantRatFSM;
        }

        public override void Execute()
        {
            base.Execute();

            if (enemy.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != SLEEP_STATE_HASH)
            {
                return;
            }

            if (enemy.target)
            {
                enemy.Animator.SetTrigger(EnemyAnimationString.WakeUp);
                fsm.ChangePattern(fsm.idleState);
            }
            else if (owner.Stat.OnDamageEffect)
            {
                enemy.Animator.SetTrigger(EnemyAnimationString.Bark);
                fsm.ChangePattern(fsm.idleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            
        }
    }
}
