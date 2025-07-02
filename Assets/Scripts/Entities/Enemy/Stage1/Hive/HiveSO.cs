using ToB.Entities;
using UnityEngine;

namespace ToB
{
    [CreateAssetMenu(fileName = "HiveSO", menuName = "Scriptable Objects/Enemy/Stage1/HiveSO")]
    public class HiveSO : EnemySO
    {
        [field:SerializeField] public float HP { get; private set; }
        [field:SerializeField] public float FlyRegenTimeInterval { get; private set; }
        [field:SerializeField] public float FlyRegenAmount { get; private set; }
        [field:SerializeField] public float PatrolRange { get; private set; }
        [field:SerializeField] public float ChaseRange { get; private set; }
        
    }
}
