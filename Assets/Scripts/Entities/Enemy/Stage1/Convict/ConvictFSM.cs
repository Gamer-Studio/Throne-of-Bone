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
            chaseState = new ConvictChaseState(this, Owner.DataEnemyKnockbackEnemyMainBodySo.ChaseSpeed);
            patrolState = new ConvictPatrolState(this, Owner.DataEnemyKnockbackEnemyMainBodySo.MoveSpeed);
            IdleState = new ConvictIdleState(this);
            
            ChangePattern(patrolState);
        }
    }
}
