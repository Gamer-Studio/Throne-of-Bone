using System;
using UnityEngine;

namespace ToB
{
    public abstract class Enemy : MonoBehaviour
    {
        [field:SerializeField] public EnemyData EnemyData { get; private set; }
        [field:SerializeField] public Rigidbody2D rb { get; private set; }
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        public bool IsGrounded => Physics.IsGrounded();
        public bool IsTargetLeft => GetTargetDirection().x < 0;
        
        public Transform target;
        public float bodyDamage;    // 충돌 시 데미지. 적군 상태에 따라 너무 유동적이기 쉬워서 public으로 함
        
        [SerializeField] LayerMask hittableMask;
        protected virtual void Awake()
        {
            hittableMask = LayerMask.GetMask("Player");
            rb = GetComponent<Rigidbody2D>();
            Physics = GetComponent<EnemyPhysics>();
            bodyDamage = EnemyData.ATK;
        }
 
        // FSM|전략패턴|BT 무엇을 쓸지는 각자(ex: Slime.cs) 안에서
        public abstract void OnTakeDamage(float damage);
        public abstract void Die();
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("플레이어 대미지(bodyDamage)");
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
    }
}
