using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class RockFallTrap_detect : MonoBehaviour
    {
        private RockFallTrap rockFallTrap;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rockFallTrap = GetComponentInParent<RockFallTrap>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                rockFallTrap.IsDetected = true;
            }
        }
    }
}