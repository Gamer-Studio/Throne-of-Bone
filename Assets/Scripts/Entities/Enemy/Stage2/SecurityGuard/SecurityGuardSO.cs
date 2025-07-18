using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SecurityGuardSO", menuName = "Scriptable Objects/Enemy/Stage2/SecurityGuardSO")]
    public class SecurityGuardSO : EnemySO
        , IEnemyGroundMoveSO
        , IEnemyHittableSO
        , IEnemyKnockbackSO
        , IEnemyChaserSO
        , IEnemyMainBodySO
        , IEnemySightSensorSO
    {
        [field: SerializeField] public float HP { get; private set; } = 40;
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; } = 0.3f;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3;
        [field: SerializeField] public float ChaseSpeed { get; private set; } = 9;
        [field: SerializeField] public float BodyDamage { get; private set; } = 5;
        [field: SerializeField] public float SightRange { get; private set; } = 12.5f;
        [field: SerializeField] public float SightAngle { get; private set; } = 130;
    }
}