using System;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class ContactDamage : MonoBehaviour
    {
        
        public float damage;
        public float knockBackPower;
        public Vector2 knockBackDirection;
        
        public LayerMask playerMask;

        private bool directional;

        private void Reset()
        {
            playerMask = LayerMask.GetMask("Player");
        }

        public void Init(float damage, float knockBackPower, Vector2 knockBackDirection = default, bool fixedDirection = true)
        {
            this.damage = damage;
            this.knockBackPower = knockBackPower;
            this.knockBackDirection = knockBackDirection;
            directional = fixedDirection;
        }
        
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                other.Damage(damage);
                if(directional) other.KnockBack(knockBackPower, knockBackDirection);
                else other.KnockBack(knockBackPower, gameObject);
            }
        }
    }
}
