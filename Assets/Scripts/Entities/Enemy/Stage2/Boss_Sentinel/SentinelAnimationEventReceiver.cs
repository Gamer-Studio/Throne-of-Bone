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
    }
}
