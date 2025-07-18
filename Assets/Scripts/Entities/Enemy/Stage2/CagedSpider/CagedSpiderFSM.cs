using UnityEngine;

namespace ToB.Entities
{
    public class CagedSpiderFSM : EnemyStrategy
    {
        private CagedSpider owner;
        private GroundDefaultMovePattern groundMovePattern;
        private GroundDefaultChasePattern groundChasePattern;
        private CagedSpiderSelfDestructPattern selfDestructPattern;

        public override void Init()
        {
            owner = enemy as CagedSpider;
            
            groundMovePattern = new GroundDefaultMovePattern(this);
            groundChasePattern = new GroundDefaultChasePattern(this);
            selfDestructPattern = new CagedSpiderSelfDestructPattern(this);
            
            groundMovePattern.AddTransition(()=>enemy.target, groundChasePattern);
            
            groundChasePattern.AddTransition(()=>!enemy.target, groundMovePattern);
            groundChasePattern.AddTransition(() => enemy.target && owner.Stat.CurrentHP * 2 <= owner.Stat.MaxHP, selfDestructPattern);
            
            ChangePattern(groundMovePattern);
        }
        
        protected override void Update()
        {
            // 공용 상태 스크립트 활용을 위해 fsm 본체에서 상태 관리를 합니다
            // 모든 Move가 추적하고 Chase로 전이하지는 않기 위해
            if (!enemy.IsAlive) return;
            
            if (currentPattern == groundMovePattern && enemy.target)
            {
                ChangePattern(groundChasePattern);
                return;
            }
            if (currentPattern == groundChasePattern && !enemy.target)
            {
                ChangePattern(groundMovePattern);
                return;
            }
            
            base.Update();
        }
    }
}
