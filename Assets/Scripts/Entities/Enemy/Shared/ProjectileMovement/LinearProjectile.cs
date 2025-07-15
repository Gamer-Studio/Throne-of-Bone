using System;
using ToB.Entities.FieldObject;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(LinearMovement))]
    [RequireComponent(typeof(ContactDamage))]
    public class LinearProjectile : MonoBehaviour
    {
        [field:SerializeField] public LinearMovement LinearMovement {get; private set;}
        [field:SerializeField] public ContactDamage ContactDamage {get; private set;}
        
        [SerializeField] LayerMask layerMask;
        private void Reset()
        {
            LinearMovement = GetComponent<LinearMovement>();
            ContactDamage = GetComponent<ContactDamage>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if ((layerMask & 1 << other.gameObject.layer) != 0)
            {
                gameObject.Release();
            }
        }
    }
}
