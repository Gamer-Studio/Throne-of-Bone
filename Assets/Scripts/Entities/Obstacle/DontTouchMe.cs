using System;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class DontTouchMe : MonoBehaviour
    {
        public float damage;
        public float knockBackPower;
        public Vector2 knockBackDirection;
        
        public LayerMask playerMask;

        private void Reset()
        {
            playerMask = LayerMask.GetMask("Player");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                other.Damage(damage);
                other.KnockBack(knockBackPower, knockBackDirection);
            }
        }
    }
}
