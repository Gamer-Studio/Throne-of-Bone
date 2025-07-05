using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SmolSlimeSO", menuName = "Scriptable Objects/Enemy/Stage1/SmolSlimeSO")]
    public class SmolSlimeSO : 
        EnemySO,
        IEnemyHittableSO,
        IEnemyGroundMoveSO,
        IEnemySightSensorSO,
        IEnemyKnockbackSO
    {
        [field: SerializeField] public float HP { get; private set; }
        public float DEF { get; } = 0;
        [field: SerializeField] public float ATK { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float ChaseSpeed { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; }
        
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }
        
    }
}
