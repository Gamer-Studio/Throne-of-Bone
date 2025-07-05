using UnityEngine;

namespace ToB.Entities
{
    public class ConvictFSM : EnemyStrategy
    {
        public Convict Owner { get; private set; }
        
        public ConvictAttackState attackState;
        public ConvictChaseState chaseState;
        public ConvictPatrolState patrolState;
        public ConvictIdleState IdleState;
        
        protected override void Awake()
        {
            base.Awake();
            Owner = enemy as Convict;
        }

        public override void Init()
        {
            attackState = new ConvictAttackState(this);
            chaseState = new ConvictChaseState(this, Owner.DataSo.ChaseSpeed);
            patrolState = new ConvictPatrolState(this, Owner.DataSo.MoveSpeed);
            IdleState = new ConvictIdleState(this);
            
            ChangePattern(patrolState);
        }
    }
}
