using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "MutantRatSO", menuName = "Scriptable Objects/Enemy/Stage1/MutantRatSO")]
    public class MutantRatSO :
        ScriptableObject,
        IEnemyMainBodySO,
        IEnemyKnockbackSO,
        IEnemySightSensorSO
    {
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField]public float BodyDamage { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; }
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }
        
        [field: SerializeField] public float RollDamage { get; private set; }
        // [field: SerializeField] public float DefaultMoveSpeed { get; private set; }
        [field: SerializeField] public float MoveSpeedWhileRoll { get; private set; }
        
        [field: SerializeField] public float AwakeTime { get; private set; }
        
    }
}