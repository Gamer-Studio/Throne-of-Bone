using UnityEngine;

namespace ToB.Entities
{
    public class SentinelAnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] Sentinel sentinel;

        public void SetRangeAttackDirection()
        {
            sentinel.rangeAttackDirection = (Vector2)sentinel.target.position - sentinel.BodyCenter;
        }
        
        public void OnRangeAttack()
        {
            sentinel.RangeAttack();
        }

        public void Dodge()
        {
            sentinel.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = -10 * sentinel.LookDirectionHorizontal;
        }

        public void LongSprint()
        {
            sentinel.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = 80 * sentinel.LookDirectionHorizontal;
        }
        public void LongSprintNext()
        {
            sentinel.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = 15 * sentinel.LookDirectionHorizontal;
        }

        public void Stop()
        {
            sentinel.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = Vector2.zero;
        }
    }
}
