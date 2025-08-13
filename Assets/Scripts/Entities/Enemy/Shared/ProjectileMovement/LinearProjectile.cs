using ToB.Entities.FieldObject;
using ToB.Entities.Projectiles;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(LinearMovement))]
    [RequireComponent(typeof(ContactDamage))]
    public class LinearProjectile : Projectile
    {
        [field:SerializeField] public LinearMovement LinearMovement {get; private set;}
        [field:SerializeField] public ContactDamage ContactDamage {get; private set;}
        
        [SerializeField] LayerMask layerMask;
        
        private void Reset()
        {
            LinearMovement = GetComponent<LinearMovement>();
            ContactDamage = GetComponent<ContactDamage>();
        }

        /// <summary>
        /// Projectile로 api 통합하면서 레이어 필드 통합했어요
        /// </summary>
        private void OnEnable()
        {
            hitLayers = layerMask;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // Projectile.hitLayers + Mathutil.Contains 기반으로 변경했습니다.
            if (hitLayers.Contains(other)) gameObject.Release();
        }
    }
}
