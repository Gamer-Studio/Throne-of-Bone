using System;
using System.Collections;
using DG.Tweening;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class SewerRat : Enemy
    {
        public SewerRatSO DataSO { get; private set; }
        [field: SerializeField] public SewerRatStrategy Strategy { get; private set; }
       
        [SerializeField] private EnemyStatHandler stat;
        
        [SerializeField] private ParticleSystem deathBleed;
        [SerializeField] private ParticleSystem deathExplode;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }

        [SerializeField] private GameObject specialPrefab;
        [SerializeField] private Location bossRoomLocation;

        protected override void Awake()
        {
            base.Awake();
            
            DataSO = enemySO as SewerRatSO;

            if (!stat) stat = GetComponentInChildren<EnemyStatHandler>();
            if (!Knockback) Knockback = GetComponentInChildren<EnemyKnockback>();
            if (!EnemyBody) EnemyBody = GetComponentInChildren<EnemyBody>();
            
            deathBleed.gameObject.SetActive(false);
            deathExplode.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Strategy.Init();
            Knockback.Init(this);
            stat.Init(this, DataSO);
            EnemyBody.Init(this, DataSO.BodyDamage);
        }
        

        protected override void Die()
        {
            base.Die();
            
            Animator.SetTrigger(EnemyAnimationString.Die);
            Strategy.enabled = false;

            transform.DOKill();
            Sprite.transform.DOShakePosition(3f, 0.5f, 18, fadeOut: false);

            StartCoroutine(DieEffect());
        }

        IEnumerator DieEffect()
        {
            Strategy.CancelEffects();

            deathBleed.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(3f);

            var emission = deathBleed.emission;
            emission.enabled = false;

            transform.DOKill();
            
            Sprite.enabled = false;
            Hitbox.enabled = false;
            EnemyBody.enabled = false;
            isAlive = false;
            
            deathExplode.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);
            
            gameObject.SetActive(false);
            deathBleed.gameObject.SetActive(false);
            deathExplode.gameObject.SetActive(false);
        }
        
        

    }
}