using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "ConvictSO", menuName = "Scriptable Objects/Enemy/Stage1/ConvictSO")]
    public class ConvictSO : EnemySO, 
        IEnemyMainBodySO, 
        IEnemyKnockbackSO,
        IEnemyHittableSO,
        IEnemyGroundMoveSO,
        IEnemySightSensorSO
    {
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField]public float DEF { get; private set; }
        [field: SerializeField] public float ATK { get; private set; }
        [field: SerializeField] public float BodyDamage { get; private set; }
        
        [field: SerializeField] public float ATKKnockbackForce { get; private set; }  
        [field: SerializeField] public Vector2 ATKKnockbackDirection { get; private set; }  
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float ChaseSpeed { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; }
        
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }
    }
}
