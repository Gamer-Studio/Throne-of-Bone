
using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyChaseState:EnemyPattern
    {
        private readonly Fly owner;
        private readonly FlyFSM fsm;

        private const string CHASE_KEY = "Chase";
        private float Speed => owner.DataSO.FastMovementSpeed;
        public FlyChaseState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Fly;
            fsm = strategy as FlyFSM;
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!owner.target || !owner.Hive.target)
            {
                fsm.ChangePattern(fsm.returnState);
            }
            else if (owner.TargetInAttackRange)
            {
                fsm.ChangePattern(fsm.attackState);
            }
            else
            {
                Vector2 targetPos = owner.target.transform.position;
                Vector2 direction = (targetPos - (Vector2)owner.transform.position).normalized;
                owner.Physics.externalVelocity[CHASE_KEY] = direction * Speed;
                enemy.LookHorizontal(direction.x > 0 ? Vector2.right : Vector2.left);
            }
        }

        public override void Exit()
        {
            base.Exit();
            owner.Physics.externalVelocity.Remove(CHASE_KEY);
        }
    }
}
