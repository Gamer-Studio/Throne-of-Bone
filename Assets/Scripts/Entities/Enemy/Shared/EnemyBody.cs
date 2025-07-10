using System;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBody:MonoBehaviour
    {
        [SerializeField] private Enemy owner;
        
        // 몬스터의 상태에따라 가변적이기에 SO 참조를 내려놓고 필드에 담았습니다 ex) 보스 1 구르기
        [SerializeField] private float bodyDamage;      
        
        [SerializeField] LayerMask hittableMask;
        
        [field:SerializeField] public BoxCollider2D BoxCollider { get; private set; }

        public void Init(Enemy enemy, float bodyDamage)
        {
            owner = enemy;
            this.bodyDamage = bodyDamage;
        }

        private void Reset()
        {
            hittableMask = LayerMask.GetMask("Player");
            BoxCollider = GetComponent<BoxCollider2D>();
            BoxCollider.isTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!owner.IsAlive) return;
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                other.Damage(bodyDamage, this);
                other.KnockBack(15, new Vector2(other.transform.position.x < transform.position.x  ? -1 : 1, 0.5f));
            }
        }

        public void ChangeDamage(float damage)
        {
            bodyDamage = damage;
        }
    }
}