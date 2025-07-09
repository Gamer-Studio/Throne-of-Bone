using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int DropGold { get; private set; }
        [field: SerializeField] public int DropMana { get; private set; }
    }
}