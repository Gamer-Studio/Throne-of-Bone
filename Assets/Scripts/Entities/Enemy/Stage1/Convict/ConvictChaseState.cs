using System;
using ToB.Core;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities
{
    public class ConvictChaseState : GroundDefaultChasePattern
    {
        private readonly Convict convict;

        public ConvictChaseState(EnemyStrategy strategy, float chaseSpeed, Action EndCallback = null) : base(strategy,
            chaseSpeed, EndCallback)
        {
            convict = enemy as Convict;
        }

        public override void Enter()
        {
            base.Enter();
            convict.Animator.SetTrigger(EnemyAnimationString.Move);
            //AudioManager.Play("env_chains_03",enemy.gameObject);
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
            else
            {
                if (enemy.Physics.IsLedgeBelow())
                {
                    enemy.Physics.externalVelocity[ChaseKey] = Vector2.zero;
                }
                else
                {
                    enemy.Physics.externalVelocity[ChaseKey] = chaseSpeed * enemy.LookDirectionHorizontal;
                }
            }
        }
    }
}