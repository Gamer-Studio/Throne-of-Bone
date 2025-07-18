using System;
using NaughtyAttributes;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAttackArea : MonoBehaviour
    {
        enum KnockbackType
        {
            Directional,
            Sender
        }
        
        [SerializeField, ReadOnly] private Enemy owner;
        [SerializeField] private float damage;
        [SerializeField] private float knockBackForce;
        
        [Tooltip("오른쪽 바라봤을 때 기준")]
        [SerializeField] private Vector2 knockBackDirection;
        
        [SerializeField] private LayerMask attackTargetLayers;

        private Rigidbody2D rb;
        
        [SerializeField, ReadOnly] KnockbackType knockbackType;
        
        private void Reset()
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            attackTargetLayers = LayerMask.GetMask("Player");
        }

        public void Init(Enemy character, float damage, float knockBackForce, Vector2 knockBackDirection)
        {
            owner = character;
            this.damage = damage;
            this.knockBackForce = knockBackForce;
            this.knockBackDirection = knockBackDirection;
            knockbackType = KnockbackType.Directional;
        }
        
        public void Init(Enemy character, float damage, float knockBackForce)
        {
            owner = character;
            this.damage = damage;
            this.knockBackForce = knockBackForce;
            knockbackType = KnockbackType.Sender;
        }

        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((attackTargetLayers & 1 << other.gameObject.layer) != 0)
            {
                other.Damage(damage, owner);
                if (knockbackType == KnockbackType.Directional) other.KnockBack(knockBackForce, knockBackDirection * owner.LookDirectionHorizontal);
                else other.KnockBack(knockBackForce, gameObject);
            }
        }
    }
}
