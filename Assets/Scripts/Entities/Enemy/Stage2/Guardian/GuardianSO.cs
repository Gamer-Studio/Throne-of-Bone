using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "GuardianSO", menuName = "Scriptable Objects/Enemy/Stage2/GuardianSO")]
    public class GuardianSO : EnemySO
        , IEnemyGroundMoveSO
        , IEnemyHittableSO
        , IEnemyMainBodySO
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2;
        [field: SerializeField] public float HP { get; private set; } = 250;
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float ShieldDEF { get; private set; } = 90;
        
        [field: SerializeField] public float BodyDamage { get; private set; }
        
        [field:SerializeField] public float ShieldRechargeTime { get; private set; } = 10;
        [field:SerializeField] public float ShieldDuration { get; private set; } = 2;
        
        [field:SerializeField] public float TeleportRechargeTime { get; private set; } = 8;
        [field:SerializeField] public float BlastRechargeTime { get; private set; } = 3;
        [field:SerializeField] public float LaserRechargeTime { get; private set; } = 7;
        [field:SerializeField] public float LaserTickDamage { get; private set; } = 5;
        
    }
}