using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardFSM : EnemyStrategy
    {
        private SecurityGuard owner;
        public GroundDefaultMovePattern groundMovePattern;
        public GroundDefaultChasePattern groundChasePattern;
        public SecurityGuardAttackPattern attackPattern;

        private float chaseDestinationX;
        
        float MoveXTick => owner.DataSO.MoveSpeed * Time.fixedDeltaTime;
        public override void Init()
        {
            owner = enemy as SecurityGuard;
            
            groundMovePattern = new GroundDefaultMovePattern(this);
            groundChasePattern = new GroundDefaultChasePattern(this);
            attackPattern = new SecurityGuardAttackPattern(this);
            
            groundMovePattern.AddTransition(()=>enemy.target, groundChasePattern, SetChaseDestinationX);
            groundChasePattern.AddTransition(()=>EnemyBehaviourUtility.TargetMissedUntilDestinationX(enemy,chaseDestinationX), groundMovePattern);
            groundChasePattern.AddTransition(()=>owner.AttackSensor.TargetInArea, attackPattern);
            
            ChangePattern(groundMovePattern);
        }

        private void SetChaseDestinationX()
        {
            chaseDestinationX = enemy.target.transform.position.x;
        }
        
    }
}
