using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
       
        [field: SerializeField] public float DropGold { get; private set; }
    }
}