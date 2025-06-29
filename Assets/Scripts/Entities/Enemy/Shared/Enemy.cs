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
        [field:SerializeField] public Rigidbody2D rb { get; private set; }
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; private set; }
        [field:SerializeField] public EnemyKnockback Knockback { get; protected set; }
        

        private Coroutine damageColorCoroutine;
        // 스탯 핸들러는 스탯이 복잡해지면 다룰 예정. 현재는 HP 밖에 없음
        // [field:SerializeField] public EnemyStatHandler EnemyStatHandler { get; private set; }
        
        public bool IsGrounded => Physics.IsGrounded();
        public bool IsTargetLeft => GetTargetDirection().x < 0;
        
        [Header("타겟")]
        public Transform target;
        
        [Header("속성")]
        public float bodyDamage;    // 충돌 시 데미지. 적군 상태에 따라 너무 유동적이기 쉬워서 public으로 함
        
        [SerializeField] LayerMask hittableMask;

        [SerializeField] private bool isAlive;
        protected virtual void Awake()
        {
            hittableMask = LayerMask.GetMask("Player");
            if(!rb) rb = GetComponent<Rigidbody2D>();
            if(!Physics) Physics = GetComponent<EnemyPhysics>();
            if(!Animator) Animator = GetComponentInChildren<Animator>();
            
            isAlive = true;
        }

        private void Reset()
        {
            hittableMask = LayerMask.GetMask("Player");
            rb = GetComponent<Rigidbody2D>();
            Physics = GetComponent<EnemyPhysics>();
            Animator = GetComponentInChildren<Animator>();

        }

        protected virtual void Die()
        {
            isAlive = false;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isAlive) return;
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                other.Damage(bodyDamage, this);
                other.KnockBack(10, new Vector2(transform.localScale.x, 1));
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

        public virtual void PartDestroyed(EnemyStatHandler _)
        {
            // 대부분은 몸체가 파괴되면 그대로 죽음
            Die();
        }
    }
}
