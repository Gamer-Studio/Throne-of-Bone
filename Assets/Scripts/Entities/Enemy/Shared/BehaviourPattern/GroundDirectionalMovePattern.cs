using System;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class GroundDirectionalMovePattern:EnemyPattern
    {
        public float moveSpeed;
        public float moveDuration;
        public float enterTime;
        
        private readonly Action toNextPattern;
        
        public GroundDirectionalMovePattern(EnemyStrategy strategy, Action toNextPattern, Action EndCallback = null) : base(strategy, EndCallback)
        {
            this.toNextPattern = toNextPattern;
        }

        public override void Enter()
        {
            base.Enter();
            enterTime = Time.time;
            enemy.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = new Vector2(moveSpeed, 0f);
        }

        public override void Execute()
        {
            base.Execute();

            if (Time.time - enterTime > moveDuration)
            {
                toNextPattern.Invoke();
            }
            
            if(enemy.Physics.IsLedgeOnSide(DirectionUtil.GetHorizontalDirection(moveSpeed)))
                enemy.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = Vector2.zero;
            
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Physics.externalVelocity.Remove(EnemyPhysicsKeys.MOVE);
        }
    }
}