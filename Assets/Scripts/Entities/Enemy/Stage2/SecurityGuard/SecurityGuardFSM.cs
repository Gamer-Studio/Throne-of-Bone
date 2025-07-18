using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardFSM : EnemyStrategy
    {
        private SecurityGuard owner;
        private GroundDefaultMovePattern groundMovePattern;
        private GroundDefaultChasePattern groundChasePattern;

        private float chaseDestinationX;
        
        float MoveXTick => owner.DataSO.MoveSpeed * Time.fixedDeltaTime;
        public override void Init()
        {
            owner = enemy as SecurityGuard;
            
            groundMovePattern = new GroundDefaultMovePattern(this);
            groundChasePattern = new GroundDefaultChasePattern(this);
            
            groundMovePattern.AddTransition(()=>enemy.target, groundChasePattern, SetChaseDestinationX);
            groundChasePattern.AddTransition(()=>EnemyBehaviourUtility.TargetMissedUntilDestinationX(enemy,chaseDestinationX), groundMovePattern);
            
            ChangePattern(groundMovePattern);
        }

        private void SetChaseDestinationX()
        {
            chaseDestinationX = enemy.target.transform.position.x;
        }
        
    }
}
