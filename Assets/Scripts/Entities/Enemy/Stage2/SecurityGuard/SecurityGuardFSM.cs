using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardFSM : EnemyStrategy
    {
        private SecurityGuard owner;
        public GroundDefaultMovePattern groundMovePattern;
        public GroundDefaultChasePattern groundChasePattern;
        public SecurityGuardAttackPattern attackPattern;
        
        [field:SerializeField] public SavePosition SavePosition { get; private set; }

        private float chaseDestinationX;

        public Vector3 BodyOffset { get; private set; }
        
        float MoveXTick => owner.DataSO.MoveSpeed * Time.fixedDeltaTime;
        public override void Init()
        {
            owner = enemy as SecurityGuard;

            if (!owner)
            {
                Debug.LogError("SecurityGuardFSM owner가 없습니다");
                return;           
            }
            
            groundMovePattern = new GroundDefaultMovePattern(this);
            groundChasePattern = new GroundDefaultChasePattern(this);
            attackPattern = new SecurityGuardAttackPattern(this);
            
            groundMovePattern.AddTransition(()=>enemy.target, groundChasePattern, SetChaseDestinationX);
            groundChasePattern.AddTransition(()=>EnemyBehaviourUtility.TargetMissedUntilDestinationX(enemy,chaseDestinationX), groundMovePattern);
            groundChasePattern.AddTransition(()=>owner.AttackSensor.TargetInArea, attackPattern);
            
            BodyOffset = owner.EnemyBody.transform.localPosition;
            ChangePattern(groundMovePattern);
        }

        private void SetChaseDestinationX()
        {
            chaseDestinationX = enemy.target.transform.position.x;
        }
    }
}
