using System;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBody:MonoBehaviour
    {
        [SerializeField] private Enemy owner;
        [SerializeField] private float bodyDamage;
        [SerializeField] LayerMask hittableMask;

        public void Init(Enemy enemy, float bodyDamage)
        {
            owner = enemy;
            this.bodyDamage = bodyDamage;
        }

        private void Reset()
        {
            hittableMask = LayerMask.GetMask("Player");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!owner.IsAlive) return;
            if ((hittableMask & (1 << other.gameObject.layer)) != 0)
            {
                other.Damage(bodyDamage, this);
                other.KnockBack(5, new Vector2(other.transform.position.x < transform.position.x  ? -1 : 1, 0.5f));
            }
        }

        public void ChangeDamage(float damage)
        {
            bodyDamage = damage;
        }
    }
}