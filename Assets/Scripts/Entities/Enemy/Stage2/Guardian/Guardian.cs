using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Guardian : Enemy
    {
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }

        [SerializeField] private BehaviorGraphAgent agent;

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
        }
    }
}
