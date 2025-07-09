using System;
using System.Collections;
using NaughtyAttributes;
using ToB.Core;
using ToB.Player;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(EnemyPhysics))]
    public abstract class Enemy : MonoBehaviour
    {
        [Header("기본 참조")] 
        [Expandable, SerializeField] protected EnemySO enemySO;
        public EnemySO EnemySO => enemySO;
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rb => rb;
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; private set; }
        [field:SerializeField] public EnemyKnockback Knockback { get; protected set; }
        [field:SerializeField] public BoxCollider2D Hitbox { get; private set; }
        
        public bool IsGrounded => Physics.IsGrounded();
        
        public bool IsTargetLeft => target && GetTargetDirection().x < 0;
        public Vector2 LookDirectionHorizontal => transform.localScale.x < 0 ? Vector2.left : Vector2.right;

        /// <summary>
        /// 타겟 위치에 따라 Vector2.Right나 Vector2.Left를 반환합니다.
        /// </summary>
        public Vector2 TargetDirectionHorizontal
        {
            get
            {
                if (!target) return Vector2.zero;
                Vector2 posDiff = target.position - transform.position;
                return posDiff.x > 0 ? Vector2.right : Vector2.left;
            }
        }
        
        [Header("타겟")]
        public Transform target;
        
        [Header("속성")]
        [SerializeField] protected bool isAlive;
        [field:SerializeField] public bool ReactOnDamage { get; private set; }
        public bool IsAlive => isAlive;
        
        protected virtual void Awake()
        {
            if(!Rb) rb = GetComponentInChildren<Rigidbody2D>();
            if(!Physics) Physics = GetComponent<EnemyPhysics>();
            if(!Animator) Animator = GetComponentInChildren<Animator>();
            if(!Sprite) Sprite = GetComponent<SpriteRenderer>();
            
            isAlive = true;
        }

        protected virtual void Reset()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            Physics = GetComponent<EnemyPhysics>();
            Animator = GetComponentInChildren<Animator>();
            Sprite = GetComponentInChildren<SpriteRenderer>();

            rb.bodyType = RigidbodyType2D.Kinematic;
            Hitbox.isTrigger = true;
            
            isAlive = true;
        }

        public void OnTakeDamage(MonoBehaviour sender)
        {
            if (!sender || !ReactOnDamage) return;
            bool isSenderLeft = sender.transform.position.x < transform.position.x;
            bool isLookingLeft = LookDirectionHorizontal == Vector2.left;
            
            if (isSenderLeft != isLookingLeft)
                FlipBody();
        }

        private void FlipBody()
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        
        protected virtual void Die()
        {
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Gold,enemySO.DropGold,transform);
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Mana,enemySO.DropMana,transform);
            isAlive = false;
        }

        public float GetTargetDistanceSQR()
        {
            if (!target) return float.MaxValue;
            return Vector3.SqrMagnitude(target.position - transform.position);
        }
        
        public Vector2 GetTargetDirection()
        {
            if(!target) return Vector2.zero;
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
            scale.x = direction.x > 0 ? 1 : -1;;
            transform.localScale = scale;
        }

        public void LookTarget()
        {
            if (!target) return;
            Vector2 dir = target.position - transform.position;
            LookHorizontal(dir);
        }
    }
}
