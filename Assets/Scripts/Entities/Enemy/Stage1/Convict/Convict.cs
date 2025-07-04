using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    
    [RequireComponent(typeof(ConvictFSM))]
    public class Convict : Enemy
    {
        [Header("죄수")] 
        [Expandable]
        [SerializeField] private ConvictEnemyKnockbackEnemyMainBodySo dataEnemyKnockbackEnemyMainBodySo;
        public ConvictEnemyKnockbackEnemyMainBodySo DataEnemyKnockbackEnemyMainBodySo => dataEnemyKnockbackEnemyMainBodySo;
        
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemySightSensor SightSensor {get; private set;}
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
            if(!SightSensor) SightSensor = GetComponentInChildren<EnemySightSensor>();
            if(!AttackArea) AttackArea = GetComponentInChildren <EnemyAttackArea>();
            if (!AttackSensor) AttackSensor = GetComponentInChildren<EnemySimpleSensor>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            EnemyKnockBack.Init(this, DataEnemyKnockbackEnemyMainBodySo.KnockbackMultiplier);
            Stat.Init(this, DataEnemyKnockbackEnemyMainBodySo.HP, 0);
            SightSensor.Init(this, DataEnemyKnockbackEnemyMainBodySo.SightRange, DataEnemyKnockbackEnemyMainBodySo.SightAngle);
            AttackArea.Init(this, DataEnemyKnockbackEnemyMainBodySo.ATK, DataEnemyKnockbackEnemyMainBodySo.ATKKnockbackForce, DataEnemyKnockbackEnemyMainBodySo.ATKKnockbackDirection);
            EnemyBody.Init(this, DataEnemyKnockbackEnemyMainBodySo.BodyDamage);
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
