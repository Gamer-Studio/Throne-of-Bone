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
            base.Execute();
            if(!owner.target)
                fsm.ChangePattern(fsm.returnState);
            
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
