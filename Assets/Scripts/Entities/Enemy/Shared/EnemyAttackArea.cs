using System;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class EnemyAttackArea : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour owner;
        [SerializeField] private float damage;

        public void Init(MonoBehaviour character, float damage)
        {
            this.owner = character;
            this.damage = damage;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            other.Damage(damage, owner);
        }
    }
}
