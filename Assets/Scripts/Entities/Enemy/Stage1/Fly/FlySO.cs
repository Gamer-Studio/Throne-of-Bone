using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "FlySO", menuName = "Scriptable Objects/Enemy/Stage1/FlySO")]
    public class FlySO : EnemySO
    {
        [field:SerializeField] public float HP { get; private set; }
        [field:SerializeField] public float ATK { get; private set; }
        [field:SerializeField] public float AttackDelay { get; private set; }
        [field:SerializeField] public float StingSpeed { get; private set; }
        [field:SerializeField] public float StingKnockbackPower { get; private set; }
        [field:SerializeField] public float DefaultMovementSpeed { get; private set; }
        [field:SerializeField] public float FastMovementSpeed { get; private set; }
        [field:SerializeField] public float KnockbackApplier { get; private set; }
        
        [field:SerializeField] public float SightRange { get; private set; }
        [field:SerializeField] public float SightAngle { get; private set; }
        
        [field:SerializeField] public float AttackRange { get; private set; }
        
    }
}
