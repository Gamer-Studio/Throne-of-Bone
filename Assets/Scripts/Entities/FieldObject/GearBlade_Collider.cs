using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class GearBlade_Collider : MonoBehaviour
    {
        [SerializeField] private GearBlade gearBlade;

        private void Awake()
        {
            if (!gearBlade) gearBlade = GetComponentInParent<GearBlade>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && gearBlade.isActivated)
            {
                other.Damage(gearBlade.damage, gearBlade);
                other.KnockBack(gearBlade.knockBackPower, gameObject);
            }
        }
    }
}