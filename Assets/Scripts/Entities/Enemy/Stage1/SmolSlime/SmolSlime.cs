using System;
using NaughtyAttributes;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(EnemyKnockback))]
    [RequireComponent(typeof(EnemyStatHandler))]
    [RequireComponent(typeof(SmolSlimeFSM))]
    public class SmolSlime : Enemy
    {
        [Header("슬라임")] 
        [Expandable]
        [SerializeField] private SmolSlimeSO dataSO;
        public SmolSlimeSO DataSO => dataSO;
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public SmolSlimeFSM FSM {get; private set;}
        [field:SerializeField] public EnemySightSensor SightSensor {get; private set;}
        
        
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
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hitbox.enabled = false;
            FSM.ChangePattern(null);
        }
    }
}
