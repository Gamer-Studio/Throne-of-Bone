


using UnityEngine;

namespace ToB.Entities
{
    public class Fly : Enemy
    {
        [field:SerializeField] public FlySO DataSO { get; private set; }
        [field:SerializeField] public Hive Hive { get; private set; }

        [field:SerializeField] public EnemyBody Body { get; private set; }
        [field:SerializeField] public EnemySightSensor SightSensor { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Body.Init(this, DataSO.ATK);
            SightSensor.Init(this, DataSO.SightRange, DataSO.SightAngle);
        }

        public void Init(Hive hive)
        {
            Hive = hive;
        }
        
        
    }
}
