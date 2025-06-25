using System;
using ToB.Player;
using UnityEngine;

namespace ToB
{
    // [RequireComponent(typeof(EnemyStatHandler))]
    public abstract class Enemy : MonoBehaviour
    {
        [field:SerializeField] public EnemyData EnemyData { get; private set; }
        [field:SerializeField] public Rigidbody2D rb { get; private set; }
        [field:SerializeField] public EnemyPhysics Physics { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        
        // 스탯 핸들러는 스탯이 복잡해지면 다룰 예정. 현재는 HP 밖에 없음
        // [field:SerializeField] public EnemyStatHandler EnemyStatHandler { get; private set; }
        
        public bool IsGrounded => Physics.IsGrounded();
        public bool IsTargetLeft => GetTargetDirection().x < 0;
        
        public Transform target;
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

        /// <summary>
        /// 적군이 데미지를 입는 것을 담당하는 함수입니다.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void OnTakeDamage(float damage)
        {
            ChangeHP(-damage);
        }

        protected virtual void Die()
        {
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("플레이어 대미지(bodyDamage)");
                other.GetComponent<PlayerCharacter>().Damage(bodyDamage);
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
