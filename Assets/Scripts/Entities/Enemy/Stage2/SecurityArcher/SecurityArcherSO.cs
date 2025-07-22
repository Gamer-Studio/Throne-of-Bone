using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SecurityArcherSO", menuName = "Scriptable Objects/Enemy/Stage2/SecurityArcherSO")]
    public class SecurityArcherSO : EnemySO
        , IEnemyGroundMoveSO
        , IEnemyHittableSO
        , IEnemyKnockbackSO
        , IEnemyMainBodySO
        , IEnemySightSensorSO
        , IEnemyChaserSO
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float ChaseSpeed { get; private set; }
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; }
        [field: SerializeField] public float BodyDamage { get; private set; }
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }

        [field: SerializeField] public float BeamTickDamage { get; private set; }
        [field: SerializeField] public float MoveInterval { get; private set; }
        [field: SerializeField] public float PerimeterInterval { get; private set; }
        [field: SerializeField] public float PerimeterTimeRandomRange { get; private set; }
        
        [field: SerializeField] public float DodgeMoveSpeed { get; private set; } = 5f;
    }
}