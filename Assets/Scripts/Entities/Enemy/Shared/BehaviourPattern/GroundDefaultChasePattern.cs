using System;
using UnityEngine;

namespace ToB.Entities
{
    public class GroundDefaultChasePattern:EnemyPattern
    {
        protected readonly string ChaseKey = "Chase";

        protected float ChaseSpeed => ((IEnemyChaserSO)enemy.EnemySO).ChaseSpeed;
        public GroundDefaultChasePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Physics.externalVelocity[ChaseKey] = enemy.LookDirectionHorizontal * ChaseSpeed;
        }

        public override void FixedExecute()
        {
            base.FixedExecute();
            if (enemy.Physics.IsLedgeBelow())
            {
                enemy.Physics.externalVelocity[ChaseKey] = Vector2.zero;
            }
            else
            {
                enemy.Physics.externalVelocity[ChaseKey] = ChaseSpeed * enemy.LookDirectionHorizontal;
            }
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Physics.externalVelocity.Remove(ChaseKey);
        }
    }
}