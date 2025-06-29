using System;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyAttackArea : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour owner;
        [SerializeField] private float damage;
        [SerializeField] private float knockBackForce;
        
        [Tooltip("오른쪽 바라봤을 때 기준")]
        [SerializeField] private Vector2 knockBackDirection;
        
        [SerializeField] private LayerMask attackTargetLayers;


        private void Awake()
        {
            attackTargetLayers = LayerMask.GetMask("Player");
        }

        public void Init(MonoBehaviour character, float damage, float knockBackForce, Vector2 knockBackDirection)
        {
            this.owner = character;
            this.damage = damage;
            this.knockBackForce = knockBackForce;
            this.knockBackDirection = knockBackDirection;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((attackTargetLayers & 1 << other.gameObject.layer) != 0)
            {
                other.Damage(damage, owner);
                other.KnockBack(knockBackForce, knockBackDirection);
            }
        }
    }
}
