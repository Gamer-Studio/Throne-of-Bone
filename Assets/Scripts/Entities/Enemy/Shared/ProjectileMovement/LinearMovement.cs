using System;
using UnityEngine;

namespace ToB
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LinearMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        [SerializeField] private Vector2 direction;
        [SerializeField]private float speed;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Init(Vector2 direction, float speed)
        {
            this.direction = direction.normalized;
            this.speed = speed;
            rb.linearVelocity = this.direction * speed;
            
            transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
}
