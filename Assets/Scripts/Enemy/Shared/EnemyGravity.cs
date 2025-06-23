using System;
using UnityEngine;

namespace ToB
{
    public class EnemyGravity : MonoBehaviour
    {
        Rigidbody2D rb;
        [SerializeField] private float skinWidth = 0.02f;
        [SerializeField] BoxCollider2D groundSensor;

        private float bodyWidth; // 임시

        [SerializeField] LayerMask groundLayer;
        [SerializeField] bool isGrounded;
        public bool isActive;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundSensor = GetComponentInChildren<BoxCollider2D>();

            groundLayer = LayerMask.GetMask("Ground");
        }

        void Update()
        {
            if(!isActive) return;
            if (IsGrounded() && rb.linearVelocity.y <= 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
            else rb.linearVelocity += new Vector2(0, Physics2D.gravity.y * Time.deltaTime);
        }

        public bool IsGrounded()
        {
            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(0, skinWidth); // transform.position이 바닥면이도록
            RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, new Vector2(groundSensor.size.x, skinWidth), 0, Vector2.down,
                skinWidth, groundLayer);
            
            // 땅에 박힌 경우 정상 위치로 끌어올리기
            float correction = skinWidth - hit.distance;
            if (correction > 0.002f)
            {
                transform.position += new Vector3(0, correction, 0);
            }
            
#if UNITY_EDITOR
            isGrounded = hit.collider;
#endif
            return hit.collider;
        }
    }
}