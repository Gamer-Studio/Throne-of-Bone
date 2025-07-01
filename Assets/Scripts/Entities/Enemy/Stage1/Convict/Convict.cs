using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(EnemyKnockback))]
    [RequireComponent(typeof(EnemyStatHandler))]
    public class Convict : Enemy
    {
        [Header("죄수")] 
        [Expandable]
        [SerializeField] private SimpleChaserSO dataSO;
        public SimpleChaserSO DataSO => dataSO;
        
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        // [field:SerializeField] public SmolSlimeFSM FSM {get; private set;}
        [field:SerializeField] public EnemySightSensor SightSensor {get; private set;}
        protected override void Awake()
        {
            base.Awake();
            InitProperties();
        }

        private void Start()
        {
            // FSM.Init();
        }

        protected override void Reset()
        {
            base.Reset();
            InitProperties();
        }
        private void InitProperties()
        {
            EnemyKnockBack = GetComponent<EnemyKnockback>();
            Stat = GetComponent<EnemyStatHandler>();
            SightSensor = GetComponentInChildren<EnemySightSensor>();
            
            EnemyKnockBack.Init(this, DataSO.KnockbackApplier);
            Stat.Init(this, DataSO.HP, 0);
            SightSensor.Init(this, DataSO.SightRange, DataSO.SightAngle);
            bodyDamage = DataSO.ATK;
        }

        protected override void Die()
        {
            base.Die();
        }
    }
}
