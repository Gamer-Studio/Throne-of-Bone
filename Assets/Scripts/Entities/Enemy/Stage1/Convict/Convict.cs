using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    
    [RequireComponent(typeof(ConvictFSM))]
    public class Convict : Enemy
    {
        [Header("죄수")] 
        [Expandable]
        [SerializeField] private ConvictSO dataSO;
        public ConvictSO DataSo => dataSO;
        
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemyRangeBaseSightSensor RangeBaseSightSensor {get; private set;}
        [field:SerializeField] public ConvictFSM FSM {get; private set;}
        [field:SerializeField] public EnemyAttackArea AttackArea { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackSensor { get; private set; }
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        
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
            if(!EnemyKnockBack) EnemyKnockBack = GetComponentInChildren<EnemyKnockback>();
            if(!Stat) Stat = GetComponentInChildren<EnemyStatHandler>();
            if(!RangeBaseSightSensor) RangeBaseSightSensor = GetComponentInChildren<EnemyRangeBaseSightSensor>();
            if(!AttackArea) AttackArea = GetComponentInChildren <EnemyAttackArea>();
            if (!AttackSensor) AttackSensor = GetComponentInChildren<EnemySimpleSensor>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            EnemyKnockBack.Init(this, DataSo.KnockbackMultiplier);
            Stat.Init(this, DataSo.HP, 0);
            RangeBaseSightSensor.Init(this, DataSo.SightRange, DataSo.SightAngle);
            AttackArea.Init(this, DataSo.ATK, DataSo.ATKKnockbackForce, DataSo.ATKKnockbackDirection);
            EnemyBody.Init(this, DataSo.BodyDamage);
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
