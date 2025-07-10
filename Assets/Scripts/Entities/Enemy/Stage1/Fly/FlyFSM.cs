using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class FlyFSM : EnemyStrategy
    {
        public Fly Owner { get; private set; }

        public FlyIdleState idleState;
        public FlyAttackState attackState;
        public FlyChaseState chaseState;
        public FlyReturnState returnState;
        public FlyWanderState wanderState;

        public float lastAttackTime;
        
        public const string FLY_KEY = "Fly";

        public override void Init()
        {
            Owner = enemy as Fly;

            if (!Owner) return;

            idleState = new FlyIdleState(this);
            attackState = new FlyAttackState(this);
            chaseState = new FlyChaseState(this);
            returnState = new FlyReturnState(this);
            wanderState = new FlyWanderState(this);

            ChangePattern(idleState);
            lastAttackTime = float.MaxValue;
        }

        protected override void Update()
        {
            if (lastAttackTime < Owner.DataSO.AttackDelay)
                lastAttackTime += Time.deltaTime;

            if (currentPattern is FlyIdleState or FlyWanderState)
            {
                if (Owner.target && Owner.target == Owner.Hive.target) ChangePattern(chaseState);
                else base.Update();
            }
            else
            {
                base.Update();
            }
        }
    }
}