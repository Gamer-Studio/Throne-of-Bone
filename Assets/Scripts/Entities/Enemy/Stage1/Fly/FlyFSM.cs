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

        public override void Init()
        {
            Owner = enemy as Fly;

            idleState = new FlyIdleState(this);
            attackState = new FlyAttackState(this);
            chaseState = new FlyChaseState(this);
            returnState = new FlyReturnState(this);
            wanderState = new FlyWanderState(this);
            
            currentPattern = idleState;
        }
    }
}
