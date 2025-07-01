using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class SmolSlimeFSM : EnemyStrategy
    {
        public SmolSlime Owner { get; private set; }
        public SmolSlimeMovePattern movePattern;
        public SmolSlimeChasePattern chasePattern;
        
        protected override void Awake()
        {
            base.Awake();
            Owner = enemy as SmolSlime;
        }

        public override void Init()
        {
            movePattern = new SmolSlimeMovePattern(this, Owner.DataSO.MoveSpeed);
            chasePattern = new SmolSlimeChasePattern(this, Owner.DataSO.ChaseSpeed);
            
            currentPattern = movePattern;
        }
    }
}
