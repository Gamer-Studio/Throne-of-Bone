using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace ToB.Entities
{
    public class AccelerateMovement:MonoBehaviour
    {
        Rigidbody2D rb;
        
        [SerializeField] private float maxSpeed = 10f;
        Vector2 accelerateDirection;
        Tween delayTween;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        public void SetAcceleration(Vector2 newAccelerateDirection)
        {
            accelerateDirection = newAccelerateDirection;
        }
        


        private void Update()
        {
            rb.AddForce(accelerateDirection);
            if(rb.linearVelocity.magnitude > maxSpeed) 
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        private void OnDestroy()
        {
            delayTween?.Kill();
        }

        private void OnEnable()
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}