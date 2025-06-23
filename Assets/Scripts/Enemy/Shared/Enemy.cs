using System;
using UnityEngine;

namespace ToB
{
    public abstract class Enemy : MonoBehaviour
    {
        [field:SerializeField] public EnemyData EnemyData { get; private set; }
        [field:SerializeField] public Rigidbody2D rb { get; private set; }

        private EnemyGravity gravity;
        public bool IsGrounded => gravity.IsGrounded();
        
        public Transform target;

        [SerializeField] LayerMask hittableMask;
        protected virtual void Awake()
        {
            hittableMask = LayerMask.GetMask("Player");
            rb = GetComponent<Rigidbody2D>();
            gravity = GetComponent<EnemyGravity>();
        }
 
        // FSM|전략패턴|BT 무엇을 쓸지는 각자(ex: Slime.cs) 안에서
        public abstract void OnTakeDamage(float damage);
        public abstract void Die();

        public float GetTargetDistanceSQR()
        {
            if (!target) return float.MaxValue;
            return Vector3.SqrMagnitude(target.position - transform.position);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("플레이어 대미지");
            }
        }

        public void SetGravity(bool gravityOption)
        {
            if (gravity) gravity.isActive = gravityOption;
        }


        public Vector2 GetTargetDirection()
        {
            Debug.Log("TargetPos = " + target.position + "|| thisPos = " + transform.position + "");
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
