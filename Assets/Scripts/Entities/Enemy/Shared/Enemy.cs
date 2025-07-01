using System;
using System.Collections;
using ToB.Player;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(EnemyPhysics))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Enemy : MonoBehaviour
    {
        [Header("기본 참조")]      
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rb => rb;
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; private set; }
        [field:SerializeField] public EnemyKnockback Knockback { get; protected set; }
        [field:SerializeField] public BoxCollider2D Hitbox { get; private set; }

        private Coroutine damageColorCoroutine;
        
        public bool IsGrounded => Physics.IsGrounded();
        
        public bool IsTargetLeft => GetTargetDirection().x < 0;
        public Vector2 LookDirectionHorizontal => transform.localScale.x < 0 ? Vector2.left : Vector2.right;
        
        [Header("타겟")]
        public Transform target;
        
        [Header("속성")]
        public float bodyDamage;    // 충돌 시 데미지. 적군 상태에 따라 너무 유동적이기 쉬워서 public으로 함
        [SerializeField] LayerMask hittableMask;
        [SerializeField] private bool isAlive;
        public bool IsAlive => isAlive;
        
        protected virtual void Awake()
        {
            hittableMask = LayerMask.GetMask("Player");
            if(!Rb) rb = GetComponent<Rigidbody2D>();
            if(!Physics) Physics = GetComponent<EnemyPhysics>();
            if(!Animator) Animator = GetComponentInChildren<Animator>();
            if(!Sprite) Sprite = GetComponent<SpriteRenderer>();
            if(!Hitbox) Hitbox = GetComponent<BoxCollider2D>();
            
            isAlive = true;
        }

        protected virtual void Reset()
        {
            hittableMask = LayerMask.GetMask("Player");
            rb = GetComponent<Rigidbody2D>();
            Physics = GetComponent<EnemyPhysics>();
            Animator = GetComponentInChildren<Animator>();
            Sprite = GetComponent<SpriteRenderer>();
            Hitbox = GetComponent<BoxCollider2D>();
            
            isAlive = true;
        }

        public void LookTarget()
        {
            Vector3 localScale = transform.localScale;
            localScale.x = LookDirectionHorizontal.x;
            transform.localScale = localScale;
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
                other.KnockBack(5, new Vector2(transform.localScale.x, 0.5f));
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

        public void LookHorizontal(Vector2 direction)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction.x;
            transform.localScale = scale;
        }
    }
}
