using System;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Entities.Interface;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities
{
    public enum KnockbackType
    {
        Directional,
        Sender,
        FromEnemy
    }
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAttackArea : MonoBehaviour, IAttacker
    {
        [SerializeField, ReadOnly] private Enemy owner;
        [SerializeField] private float damage;
        [SerializeField] private float knockBackForce;
        
        [Tooltip("오른쪽 바라봤을 때 기준")]
        [SerializeField] private Vector2 knockBackDirection;
        
        [SerializeField] private LayerMask attackTargetLayers;

        private Rigidbody2D rb;
        
        [SerializeField, ReadOnly] KnockbackType knockbackType;
        
        [field:SerializeField] public bool Blockable { get; private set; }
        public bool Effectable => false;
        public Vector3 Position => transform.position;
        public Team Team => Team.Enemy;
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
            this.knockBackDirection = knockBackDirection.normalized;
            knockbackType = KnockbackType.Directional;
        }
        
        public void Init(Enemy character, float damage, float knockBackForce, KnockbackType knockbackType = KnockbackType.Sender)
        {
            owner = character;
            this.damage = damage;
            this.knockBackForce = knockBackForce;
            this.knockbackType = knockbackType;
        }

        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((attackTargetLayers & 1 << other.gameObject.layer) != 0)
            {
                other.Damage(damage, owner);
                if (knockbackType == KnockbackType.Directional)
                {
                    other.KnockBack(knockBackForce, knockBackDirection * owner.LookDirectionHorizontal);
                }
                else if (knockbackType == KnockbackType.FromEnemy)
                    other.KnockBack(knockBackForce,  other.transform.position - owner.transform.position);
                else
                    other.KnockBack(knockBackForce, gameObject);
            }
        }

        
    }
}
