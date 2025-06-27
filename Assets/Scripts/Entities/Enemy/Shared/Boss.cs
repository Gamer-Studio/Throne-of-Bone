using System;
using System.Collections;
using DG.Tweening;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    // 보스별 개성적인 연출 특성상 클래스 명이 SewerRat이 될 가능성이 높습니다
    public class Boss : Enemy, IDamageable
    {
        [field:SerializeField] public BossData BossData { get; private set; }
        [field:SerializeField] public EnemyStrategy EnemyStrategy { get; private set; } // 기본적으로 보스는 전략패턴 기반

        [field: SerializeField] private ParticleSystem deathBleed;
        [field: SerializeField] private ParticleSystem deathExplode;
        protected override void Awake()
        {
            base.Awake();
            BossData = (BossData)EnemyData;

            if (!target)
            {
                Debug.LogWarning("플레이어를 직접 참조하거나 시스템을 마련해야 합니다");
            }
            
            deathBleed.gameObject.SetActive(false);
            deathExplode.gameObject.SetActive(false);
        }
        private void Start()
        {
            EnemyStrategy.Init();
        }

        protected override void Die()
        {
            base.Die();
            Animator.SetTrigger(EnemyAnimationString.Die);
            EnemyStrategy.enabled = false;
            
            transform.DOKill();
            transform.DOShakePosition(3f, 0.5f, 18,fadeOut:false);

            StartCoroutine(DieEffect());
        }

        IEnumerator DieEffect()
        {
            deathBleed.transform.SetParent(null);
            deathExplode.transform.SetParent(null);
            
            deathBleed.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            
            var emission = deathBleed.emission;
            emission.enabled = false;
            
            transform.DOKill();
            Destroy(gameObject);
            deathExplode.gameObject.SetActive(true);
            
            Destroy(deathBleed.gameObject, 5f);
            Destroy(deathExplode.gameObject, 5f);
        }

        public void Damage(float damage, MonoBehaviour sender = null)
        {
            OnTakeDamage(damage);
        }
    }
}
