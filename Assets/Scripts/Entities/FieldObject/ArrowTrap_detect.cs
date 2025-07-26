using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class ArrowTrap_detect : MonoBehaviour
    {

        private ArrowTrap arrowTrap;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            arrowTrap = GetComponentInParent<ArrowTrap>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                arrowTrap.IsDetected = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                arrowTrap.IsDetected = false;
            }
        }
    }
}