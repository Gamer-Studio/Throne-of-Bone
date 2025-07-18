using UnityEngine;

namespace ToB.Entities
{
    public class CagedSpiderFSM : EnemyStrategy
    {
        public GroundDefaultMovePattern groundMovePattern;
        public GroundDefaultChasePattern groundChasePattern;


        public override void Init()
        {
            groundMovePattern = new GroundDefaultMovePattern(this);
            groundMovePattern.AddTransition(()=>enemy.target, groundChasePattern);
            
            groundChasePattern = new GroundDefaultChasePattern(this);
            groundChasePattern.AddTransition(()=>!enemy.target, groundMovePattern);
            
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
