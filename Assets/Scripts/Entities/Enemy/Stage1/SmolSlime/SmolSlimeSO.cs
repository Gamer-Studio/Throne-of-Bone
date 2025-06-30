using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SlimeSO", menuName = "Scriptable Objects/SlimeSO")]
    public class SmolSlimeSO : EnemySO
    {
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField] public float ATK { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float ChaseSpeed { get; private set; }
        [field: SerializeField] public float KnockbackApplier { get; private set; }
        
        [field: SerializeField] public float SightRange { get; private set; }
        [field: SerializeField] public float SightAngle { get; private set; }
        
    }
}
