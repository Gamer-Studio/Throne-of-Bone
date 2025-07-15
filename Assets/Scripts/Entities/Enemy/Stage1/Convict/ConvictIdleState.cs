using System;

namespace ToB.Entities
{
    public class ConvictIdleState:EnemyPattern
    {
        private readonly Convict owner;
        public ConvictIdleState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Convict;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetBool(EnemyAnimationString.Idle, true);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!enemy.target)
            {
                owner.FSM.ChangePattern(owner.FSM.patrolState);
            }
            else if (owner.AttackSensor.TargetInArea)
            {
                owner.FSM.ChangePattern(owner.FSM.attackState);
            }
            else
            {
                owner.FSM.ChangePattern(owner.FSM.chaseState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Animator.SetBool(EnemyAnimationString.Idle, false);
        }
    }
}