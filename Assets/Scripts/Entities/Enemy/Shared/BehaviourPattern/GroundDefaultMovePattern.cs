using System;
using UnityEngine;

namespace ToB.Entities
{
    public class GroundDefaultMovePattern:EnemyPattern
    {
        private readonly string MoveKey = "Move";
        private float BaseMoveSpeed => ((IEnemyGroundMoveSO)enemy.EnemySO).MoveSpeed;
        public GroundDefaultMovePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {

        }
        
        public override void FixedExecute()
        {
            if (!enemy.IsGrounded) return;
            // 벽 판단
            if (enemy.Physics.IsWallLeft)
            {
                enemy.LookHorizontal(Vector2.right);
            }
            else if (enemy.Physics.IsWallRight)
            {
                enemy.LookHorizontal(Vector2.left);
            }
            
            // 낭떠러지 판단
            if (enemy.Physics.IsLedgeBelow())
            {
                if (enemy.Physics.IsLedgeOnSide(Vector2.left))
                {
                    enemy.LookHorizontal(Vector2.right);
                }
                else if(enemy.Physics.IsLedgeOnSide(Vector2.right))
                {
                    enemy.LookHorizontal(Vector2.left);
                }
            }
            enemy.Physics.externalVelocity[MoveKey] = enemy.LookDirectionHorizontal * BaseMoveSpeed;   
            
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Physics.externalVelocity.Remove(MoveKey);
        }
    }
}