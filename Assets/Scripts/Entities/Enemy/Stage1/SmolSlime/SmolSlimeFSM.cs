using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class SmolSlimeFSM : EnemyStrategy
    {
        private SmolSlime owner;
        public SmolSlimeMovePattern movePattern;
        public SmolSlimeChasePattern chasePattern;
        
        protected override void Awake()
        {
            base.Awake();
            owner = enemy as SmolSlime;
        }

        public override void Init()
        {
            movePattern = new SmolSlimeMovePattern(owner, owner.DataSO.MoveSpeed);
            chasePattern = new SmolSlimeChasePattern(owner, owner.DataSO.ChaseSpeed);
            
            currentPattern = movePattern;
        }
    }
}
