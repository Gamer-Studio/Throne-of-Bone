using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField] public float ATK { get; private set; }
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float BaseKnockbackMultiplier { get; private set; }
        [field: SerializeField] public float DropGold { get; private set; }
    }
}