using System;
using UnityEngine;

namespace ToB
{
    public class Boss : Enemy
    {
        [field:SerializeField] public BossData BossData { get; private set; }
        [field:SerializeField] public EnemyStrategy EnemyStrategy { get; private set; } // 기본적으로 보스는 전략패턴 기반

        protected override void Awake()
        {
            base.Awake();
            BossData = (BossData)EnemyData;
        }

        private void Start()
        {
            EnemyStrategy.Init();
        }

        public override void OnTakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }

        public override void Die()
        {
            EnemyStrategy.enabled = false;
        }
    }
}
