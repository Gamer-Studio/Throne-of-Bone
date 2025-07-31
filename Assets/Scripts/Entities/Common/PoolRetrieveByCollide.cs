using System;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class PoolRetrieveByCollide : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((layerMask & 1 << other.gameObject.layer) != 0)
            {
                gameObject.Release();
            }
        }
    }
}
