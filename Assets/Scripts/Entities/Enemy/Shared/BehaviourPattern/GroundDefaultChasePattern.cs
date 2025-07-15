using System;
using UnityEngine;

namespace ToB.Entities
{
    public class GroundDefaultChasePattern:EnemyPattern
    {
        protected readonly string ChaseKey = "Chase";
        protected readonly float chaseSpeed;
        
        public GroundDefaultChasePattern(EnemyStrategy strategy, float chaseSpeed, Action EndCallback = null) : base(strategy, EndCallback)
        {
            this.chaseSpeed = chaseSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Physics.externalVelocity[ChaseKey] = enemy.LookDirectionHorizontal * chaseSpeed;
        }

        public override void FixedExecute()
        {
            base.FixedExecute();
            if(!enemy.target)
                Exit();
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Physics.externalVelocity.Remove(ChaseKey);
        }
    }
}