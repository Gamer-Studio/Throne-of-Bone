using System;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class Boss : Enemy, IDamageable
    {
        [field:SerializeField] public BossData BossData { get; private set; }
        [field:SerializeField] public EnemyStrategy EnemyStrategy { get; private set; } // 기본적으로 보스는 전략패턴 기반
        
        protected override void Awake()
        {
            base.Awake();
            BossData = (BossData)EnemyData;

            if (!target)
            {
                Debug.LogWarning("플레이어를 직접 참조하거나 시스템을 마련해야 합니다");
            }
        }
        private void Start()
        {
            EnemyStrategy.Init();
        }

        protected override void Die()
        {
            base.Die();
            EnemyStrategy.enabled = false;
            Destroy(gameObject);
        }

        public void Damage(float damage)
        {
            OnTakeDamage(damage);
        }
    }
}
