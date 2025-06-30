using System;
using UnityEngine;

namespace ToB.Entities
{
    public class SmolSlimeChasePattern:EnemyPattern
    {
        private const string ChaseKey = "Chase";
        private SmolSlime slime;
        private readonly float chaseSpeed;
        float destinationX;
        float prevPosX;
        public SmolSlimeChasePattern(Enemy enemy, float moveSpeed, Action EndCallback = null) : base(enemy, EndCallback)
        {
            slime = enemy as SmolSlime;;
            chaseSpeed = moveSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Physics.externalVelocity[ChaseKey] = enemy.LookDirectionHorizontal * chaseSpeed;
            destinationX = enemy.target.transform.position.x;
            prevPosX = enemy.transform.position.x;
        }

        public override void FixedExecute()
        {
            float deltaPosX = enemy.transform.position.x - prevPosX;
            base.FixedExecute();
            if (destinationX - enemy.transform.position.x < 0.01f || deltaPosX < 0.003f)
            {
                slime.FSM.ChangePattern(slime.FSM.movePattern);
            }
            
            prevPosX = enemy.transform.position.x;
        }
    

        public override void Exit()
        {
            enemy.Physics.externalVelocity.Remove(ChaseKey);
        }
    }
}