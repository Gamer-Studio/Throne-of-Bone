using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(EnemyKnockback))]
    [RequireComponent(typeof(EnemyStatHandler))]
    [RequireComponent(typeof(ConvictFSM))]
    public class Convict : Enemy
    {
        [Header("죄수")] 
        [Expandable]
        [SerializeField] private ConvictSO dataSO;
        public ConvictSO DataSO => dataSO;
        
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemySightSensor SightSensor {get; private set;}
        [field:SerializeField] public ConvictFSM FSM {get; private set;}
        [field:SerializeField] public EnemyAttackArea AttackArea { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackSensor { get; private set; }
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        
        public float lastAttackTime;
        
        protected override void Awake()
        {
            base.Awake();
            InitProperties();
        }

        private void Start()
        {
            FSM.Init();
        }

        protected override void Reset()
        {
            base.Reset();
            InitProperties();
        }
        private void InitProperties()
        {
            if(!EnemyKnockBack) EnemyKnockBack = GetComponent<EnemyKnockback>();
            if(!Stat) Stat = GetComponent<EnemyStatHandler>();
            if(!SightSensor) SightSensor = GetComponentInChildren<EnemySightSensor>();
            if(!AttackArea) AttackArea = GetComponentInChildren <EnemyAttackArea>();
            if (!AttackSensor) AttackSensor = GetComponentInChildren<EnemySimpleSensor>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            EnemyKnockBack.Init(this, DataSO.KnockbackApplier);
            Stat.Init(this, DataSO.HP, 0);
            SightSensor.Init(this, DataSO.SightRange, DataSO.SightAngle);
            AttackArea.Init(this, DataSO.ATK, DataSO.ATKKnockbackForce, DataSO.ATKKnockbackDirection);
            EnemyBody.Init(this, DataSO.BodyDamage);
        }

        protected override void Die()
        {
            base.Die();
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hitbox.enabled = false;
            FSM.ChangePattern(null);
        }
    }
}
