using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "MutantRatSO", menuName = "Scriptable Objects/Enemy/Stage1/MutantRatSO")]
    public class MutantRatSO :
        EnemySO,
        IEnemyMainBodySO,
        IEnemyKnockbackSO,
        IEnemySightSensorSO,
        IEnemyHittableSO
    {
        [field: SerializeField] public float HP { get; private set; }
        public float DEF { get; private set; } = 0;
        [field: SerializeField]public float BodyDamage { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; }
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }
        
        [field: SerializeField] public float RollDamage { get; private set; }
        [field: SerializeField] public float MoveSpeedWhileRoll { get; private set; }
        [field: SerializeField] public float DecelerationSpeed { get; private set; }
        
        [field: SerializeField] public float AwakeTime { get; private set; }
        
    }
}