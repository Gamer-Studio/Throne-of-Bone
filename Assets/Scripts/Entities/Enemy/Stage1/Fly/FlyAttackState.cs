using System;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyAttackState:EnemyPattern
    {
        private readonly Fly owner;
        private readonly FlyFSM fsm;
        
        public FlyAttackState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Fly;
            fsm = strategy as FlyFSM;
        }

        public override void Enter()
        {
            fsm.lastAttackTime = owner.DataSO.AttackDelay / 2;     // 발견 직후 적당한 시간 간격
            base.Enter();
        }

        public override void Execute()
        {
            // 공격할 때 관성
            Vector2 vel = owner.Physics.externalVelocity[FlyFSM.FLY_KEY];
            Vector2 dampingForce = -vel * owner.DataSO.FlyDamping;
            owner.Physics.externalVelocity[FlyFSM.FLY_KEY] += dampingForce * Time.deltaTime;
            owner.LookTarget();
            base.Execute();

            if (!owner.target)
            {
                if (!owner.Hive.target)
                    fsm.ChangePattern(fsm.returnState);
                else
                {
                    fsm.ChangePattern(fsm.chaseState);
                }
            }
            else if(!owner.TargetInAttackRange) 
                fsm.ChangePattern(fsm.chaseState);
            
            else if (fsm.lastAttackTime >= owner.DataSO.AttackDelay)
            {
                StingAttack();
            }
        }

        private void StingAttack()
        {
            fsm.lastAttackTime = 0;
            owner.StingAttack();
        }
    }
}
