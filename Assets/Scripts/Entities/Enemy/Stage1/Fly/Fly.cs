


using UnityEngine;

namespace ToB.Entities
{
    public class Fly : Enemy
    {
        [field:SerializeField] public Hive Hive { get; private set; }

        public void Init(Hive hive)
        {
            Hive = hive;
        }
    }
}
