using System;
using UnityEngine;

namespace ToB.Entities
{
    public class SmolSlimeChasePattern:EnemyPattern
    {
        private const string ChaseKey = "Chase";
        private readonly SmolSlime slime;
        private SmolSlimeFSM fsm;
        private float ChaseSpeed => slime.DataSO.ChaseSpeed;
        float destinationX;

        public SmolSlimeChasePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            fsm = strategy as SmolSlimeFSM;;
            slime = fsm.Owner;
            slime = enemy as SmolSlime;
        }



        public override void Enter()
        {
            base.Enter();
            enemy.Physics.externalVelocity[ChaseKey] = enemy.LookDirectionHorizontal * ChaseSpeed;
            destinationX = enemy.target.transform.position.x;
        }

        public override void FixedExecute()
        {
            base.FixedExecute();
            
            if (Mathf.Abs(destinationX - enemy.transform.position.x) < 0.003f || !enemy.target)
            {
                slime.FSM.ChangePattern(slime.FSM.movePattern);
            }

            if (enemy.Physics.IsLedgeBelow())
            {
                enemy.Physics.externalVelocity[ChaseKey] = Vector2.zero;
            }
        }
    

        public override void Exit()
        {
            enemy.Physics.externalVelocity.Remove(ChaseKey);
        }
    }
}