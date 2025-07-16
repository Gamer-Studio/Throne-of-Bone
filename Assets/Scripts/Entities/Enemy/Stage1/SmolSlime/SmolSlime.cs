using System;
using NaughtyAttributes;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(SmolSlimeFSM))]
    public class SmolSlime : Enemy
    {
        [Header("슬라임")] 
        public SmolSlimeSO DataSO { get; private set; }
        [field:SerializeField] public EnemyKnockback EnemyKnockBack { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public SmolSlimeFSM FSM {get; private set;}
        [field:SerializeField] public EnemyRangeBaseSightSensor RangeBaseSightSensor {get; private set;}
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            DataSO = enemySO as SmolSlimeSO;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InitProperties();
            
            FSM.Init();
        }

        protected override void Reset()
        {
            base.Reset();
            InitProperties();
        }

        private void InitProperties()
        {
            if (!EnemyKnockBack) EnemyKnockBack = GetComponentInChildren<EnemyKnockback>();
            if (!Stat) Stat = GetComponentInChildren<EnemyStatHandler>();
            if (!RangeBaseSightSensor) RangeBaseSightSensor = GetComponentInChildren<EnemyRangeBaseSightSensor>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            EnemyKnockBack.Init(this);
            Stat.Init(this, DataSO);
            RangeBaseSightSensor.Init(this);
            EnemyBody.Init(this, DataSO.ATK);
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
