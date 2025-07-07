using ToB.Entities;
using UnityEngine;

namespace ToB
{
    [CreateAssetMenu(fileName = "HiveSO", menuName = "Scriptable Objects/Enemy/Stage1/HiveSO")]
    public class HiveSO : 
        EnemySO,
        IEnemyHittableSO,
        IEnemySightSensorSO
    {
        [field:SerializeField] public float HP { get; private set; }
        public float DEF { get; } = 0;
        [field:SerializeField] public float FlyRegenTimeInterval { get; private set; }
        [field:SerializeField] public int FlyRegenAmount { get; private set; }
        [field:SerializeField] public float PatrolRange { get; private set; }
        [field:SerializeField] public float ChaseRange { get; private set; }

        public float SightRange => ChaseRange;
        public float SightAngle { get; } = 360;
    }
}
