using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SpiderInCageSO", menuName = "Scriptable Objects/Enemy/Stage2/SpiderInCageSO")]
    public class CagedSpiderSO : EnemySO
    ,IEnemyGroundMoveSO
    ,IEnemyHittableSO
    ,IEnemyKnockbackSO
    ,IEnemyChaserSO
    ,IEnemyMainBodySO
    ,IEnemySightSensorSO
    {
        [field:SerializeField] public float HP { get; private set; } = 10;
        [field:SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5;
        [field: SerializeField] public float ChaseSpeed { get; private set; } = 7;
        [field: SerializeField] public float KnockbackMultiplier { get; private set; } = 0.95f;

        [field: SerializeField] public float BodyDamage { get; private set; } = 10;
        [field: SerializeField] public float ExplodeDamage { get; private set; } = 30;
        [field: SerializeField] public float SightRange { get; private set; } = 7;
        [field: SerializeField] public float SightAngle { get; private set; } = 90;
    }
}
