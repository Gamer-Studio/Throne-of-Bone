using System;
using System.Collections;
using ToB.Player;
using UnityEngine;

namespace ToB.Entities
{
    // [RequireComponent(typeof(EnemyStatHandler))]
    public abstract class Enemy : MonoBehaviour
    {
        [Header("기본 참조")]
        [field:SerializeField] public EnemyData EnemyData { get; private set; }
        [field:SerializeField] public Rigidbody2D rb { get; private set; }
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public EnemyKnockback Knockback { get; private set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; private set; }

        private Coroutine damageColorCoroutine;
        // 스탯 핸들러는 스탯이 복잡해지면 다룰 예정. 현재는 HP 밖에 없음
        // [field:SerializeField] public EnemyStatHandler EnemyStatHandler { get; private set; }
        
        public bool IsGrounded => Physics.IsGrounded();
        public bool IsTargetLeft => GetTargetDirection().x < 0;
        
        [Header("타겟")]
        public Transform target;
        
        [Header("속성")]
        public float bodyDamage;    // 충돌 시 데미지. 적군 상태에 따라 너무 유동적이기 쉬워서 public으로 함

        public float BaseHP => EnemyData.HP;
        public float currentHP;
        
        [SerializeField] LayerMask hittableMask;
        protected virtual void Awake()
        {
            hittableMask = LayerMask.GetMask("Player");
            if(!rb) rb = GetComponent<Rigidbody2D>();
            if(!Physics) Physics = GetComponent<EnemyPhysics>();
            if(!Animator) Animator = GetComponentInChildren<Animator>();
            currentHP = EnemyData.HP;
            //if(!EnemyStatHandler) EnemyStatHandler = GetComponent<EnemyStatHandler>();

            //EnemyStatHandler.Init(this);
            bodyDamage = EnemyData.ATK;
        }

        private void Reset()
        {
            hittableMask = LayerMask.GetMask("Player");
            rb = GetComponent<Rigidbody2D>();
            Physics = GetComponent<EnemyPhysics>();
            Animator = GetComponentInChildren<Animator>();
            //EnemyStatHandler = GetComponent<EnemyStatHandler>();
            bodyDamage = EnemyData.ATK;
        }

        
        // 인터페이스화 할 예정
        public virtual void OnTakeDamage(float damage)
        {
            float actualDamage = damage * (1 - EnemyData.DEF / 100);
            ChangeHP(-actualDamage);
            
            if (currentHP <= 0)
            {
                Die();
            }
            
            if(damageColorCoroutine != null) StopCoroutine(damageColorCoroutine);
            damageColorCoroutine = StartCoroutine(DamageColorOverlay());

        }

        IEnumerator DamageColorOverlay()
        {
            Sprite.material.SetFloat("_Alpha", 1);
            float duration = 0.3f;
            float remainedTime = duration;

            while (remainedTime > 0)
            {
                yield return null;
                remainedTime -= Time.deltaTime;
                Sprite.material.SetFloat("_Alpha", remainedTime / duration);
            }
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            Knockback?.ApplyKnockback(direction, force);
        }

        protected virtual void Die()
        {
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("플레이어 대미지(bodyDamage)");
                other.GetComponent<PlayerCharacter>().Damage(bodyDamage, this);
            }
        }


        public float GetTargetDistanceSQR()
        {
            if (!target) return float.MaxValue;
            return Vector3.SqrMagnitude(target.position - transform.position);
        }
        
        public Vector2 GetTargetDirection()
        {
            Vector2 posDiff = target.position - transform.position;
            return posDiff.normalized;
        }
        
        private void OnDrawGizmos()
        {
            if (!target) return;

            Vector3 origin = transform.position;
            Vector3 dir = target.position - transform.position;
            
            Debug.DrawRay(origin, dir, Color.cyan);
        }
        
        public void ChangeHP(float delta)
        {
            currentHP += delta;
            currentHP = Mathf.Clamp(currentHP, 0, BaseHP);
        }
    }
}
