using System;
using System.Collections;
using DG.Tweening;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class SewerRat : Enemy
    {
        [field: SerializeField] public SewerRatSO DataSO { get; private set; }
        [field: SerializeField] public SewerRatStrategy Strategy { get; private set; }
       
        [SerializeField] private EnemyStatHandler stat;
        
        [SerializeField] private ParticleSystem deathBleed;
        [SerializeField] private ParticleSystem deathExplode;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }

        [SerializeField] private GameObject specialPrefab;

        protected override void Awake()
        {
            base.Awake();

            if (!target)
            {
                Debug.LogWarning("플레이어를 직접 참조하거나 시스템을 마련해야 합니다");
            }

            if (!stat) stat = GetComponentInChildren<EnemyStatHandler>();
            if (!Knockback) Knockback = GetComponentInChildren<EnemyKnockback>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            deathBleed.gameObject.SetActive(false);
            deathExplode.gameObject.SetActive(false);
        }

        private void Start()
        {
            Strategy.Init();
            Knockback.Init(this, DataSO.KnockbackMultiplier);
            stat.Init(this, DataSO.HP, DataSO.DEF);
            EnemyBody.Init(this, DataSO.BodyDamage);
        }

        protected override void Die()
        {
            base.Die();
            
            Animator.SetTrigger(EnemyAnimationString.Die);
            Strategy.enabled = false;

            transform.DOKill();
            transform.DOShakePosition(3f, 0.5f, 18, fadeOut: false);

            StartCoroutine(DieEffect());
        }

        IEnumerator DieEffect()
        {
            Strategy.CancelEffects();
            deathBleed.transform.SetParent(null);
            deathExplode.transform.SetParent(null);

            deathBleed.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);

            var emission = deathBleed.emission;
            emission.enabled = false;

            transform.DOKill();
            Destroy(gameObject);
            deathExplode.gameObject.SetActive(true);
            Instantiate(specialPrefab, transform.position + new Vector3(0,1), Quaternion.identity);
            
            Destroy(deathBleed.gameObject, 5f);
            Destroy(deathExplode.gameObject, 5f);
        }
    }
}