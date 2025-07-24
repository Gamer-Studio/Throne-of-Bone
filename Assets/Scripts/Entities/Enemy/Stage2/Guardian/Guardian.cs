using DG.Tweening;
using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Guardian : Enemy
    {
        public GuardianSO GuardianSO => enemySO as GuardianSO;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaInnerSensor { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaOuterSensor { get; private set; }

        [SerializeField] private BehaviorGraphAgent agent;

        public Tween ShieldRecharger;
        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
        }
    }
}
