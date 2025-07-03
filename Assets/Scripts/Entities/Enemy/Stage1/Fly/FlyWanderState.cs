using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyWanderState:EnemyPattern
    {
        public Vector2 destination;
        private const string WANDER_KEY = "Wander";
        
        private readonly float moveTick;
        private Vector2 prevPosition;

        private readonly Fly owner;
        private readonly FlyFSM fsm;
        private float WanderSpeed => owner.DataSO.DefaultMovementSpeed;
        public FlyWanderState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            fsm = strategy as FlyFSM;
            owner = enemy as Fly;
            moveTick = WanderSpeed * Time.fixedDeltaTime;
        }

        public override void Enter()
        {
            base.Enter();
            destination = owner.Hive.PatrolRange.GetRandomPosition();
            Vector2 direction = (destination - (Vector2)enemy.transform.position).normalized;
            enemy.Physics.externalVelocity[WANDER_KEY] = direction * WanderSpeed;

            prevPosition = enemy.transform.position;
            enemy.LookHorizontal(direction.x > 0 ? Vector2.right : Vector2.left);
            
        }
        public override void FixedExecute()
        {
            base.FixedExecute();

            float moveDelta = ((Vector2)enemy.transform.position - prevPosition).magnitude;

            if ((destination - (Vector2)enemy.transform.position).magnitude <= moveTick         // 도착지점에 갔는가
                || moveDelta < moveTick - 0.002f                                                // 벽에 비비는지 않는가(눈에 띄게 심한 감속 체크)
                || !owner.IsInPatrolArea)                                                       // 영역 이탈을 하려 하는가
            {
                strategy.ChangePattern(fsm.idleState);
            }

            prevPosition = enemy.transform.position;
            
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Physics.externalVelocity.Remove(WANDER_KEY);
        }
    }
}
