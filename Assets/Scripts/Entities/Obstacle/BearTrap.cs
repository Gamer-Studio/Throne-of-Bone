using System;
using UnityEngine;

namespace ToB.Entities
{
    public class BearTrap : MonoBehaviour
    {
        private LayerMask playerMask;

        private void Awake()
        {
            playerMask = LayerMask.GetMask("Player");       
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                Debug.Log("Bear Trap");
                other.Damage(10, this);
                other.KnockBack(5, new Vector2(other.transform.eulerAngles.y == 0 ? 1 : -1, 1));
            }
        }
    }
}
