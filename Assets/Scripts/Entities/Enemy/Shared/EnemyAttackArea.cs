using System;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class EnemyAttackArea : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour owner;
        [SerializeField] private float damage;
        [SerializeField] private LayerMask attackTargetLayers;


        private void Awake()
        {
            attackTargetLayers = LayerMask.GetMask("Player");
        }

        public void Init(MonoBehaviour character, float damage)
        {
            this.owner = character;
            this.damage = damage;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if((attackTargetLayers & 1 << other.gameObject.layer) != 0)
                other.Damage(damage, owner);
        }
    }
}
