using System;
using UnityEngine;

namespace ToB
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LinearMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        [SerializeField] private Vector2 direction;
        private float speed;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Init(Vector2 direction, float speed)
        {
            this.direction = direction.normalized;
            this.speed = speed;
        }

        private void Start()
        {
            rb.linearVelocity = direction * speed;
        }
    }
}
