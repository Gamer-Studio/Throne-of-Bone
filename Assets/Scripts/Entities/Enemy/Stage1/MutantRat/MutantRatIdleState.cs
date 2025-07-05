using System;
using UnityEngine;

namespace ToB.Entities
{
    public class MutantRatIdleState : EnemyPattern
    {
        private readonly MutantRat owner;
        private MutantRatFSM fsm;
        
        private float stateElapsedTime;
        private static readonly int IDLE_STATE_HASH = Animator.StringToHash("Base Layer.Idle");
        
        public MutantRatIdleState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as MutantRat;
            fsm = strategy as MutantRatFSM;
        }

        public override void Enter()
        {
            base.Enter();
            stateElapsedTime = 0;
        }

        public override void Execute()
        {
            if (enemy.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != IDLE_STATE_HASH)
            {
                return;
            }
            base.Execute();
            stateElapsedTime += Time.deltaTime;
            enemy.LookTarget();

            if(owner.Stat.OnDamageEffect)
                stateElapsedTime = 0;
            
            if (owner.target)
            {
                if(owner.AttackSensor.TargetInArea)
                    fsm.ChangePattern(fsm.rollState);
            }
            else if (stateElapsedTime >= owner.DataSO.AwakeTime)
            {
                enemy.Animator.SetTrigger(EnemyAnimationString.Sleep);
                fsm.ChangePattern(fsm.sleepState);
            }
        }
    }
}
