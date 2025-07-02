using System;
using UnityEngine;

namespace ToB.Entities
{
    public class ConvictChaseState:GroundDefaultChasePattern
    {
        private readonly Convict convict;
        private ConvictFSM fsm;
        public ConvictChaseState(EnemyStrategy strategy, float chaseSpeed, Action EndCallback = null) : base(strategy, chaseSpeed, EndCallback)
        {
            fsm = strategy as ConvictFSM;
            convict = fsm.Owner;
        }

        public override void Enter()
        {
            base.Enter();
            convict.Animator.SetTrigger(EnemyAnimationString.Move);
        }

        public override void FixedExecute()
        {
            base.FixedExecute();
            
            if (!enemy.target)
            {
                convict.FSM.ChangePattern(convict.FSM.patrolState);
            }
            else if (convict.AttackSensor.TargetInArea)
            {
                convict.FSM.ChangePattern(convict.FSM.attackState);
            }
            else if (enemy.Physics.IsLedgeBelow())
            {
                enemy.Physics.externalVelocity[ChaseKey] = Vector2.zero;
            }
        }
    }
}