using System;
using UnityEngine;

namespace ToB
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SimpleRotate : MonoBehaviour
    {
        [SerializeField] private float rotSpeed = 90f;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        public void SetRotationSpeed(float newSpeed)
        {
            rotSpeed = newSpeed;
            rb.angularVelocity = rotSpeed;
        }
    }
}
