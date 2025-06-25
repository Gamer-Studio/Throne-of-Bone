using System;
using NaughtyAttributes;
using UnityEngine;

namespace ToB
{
    public class EnemyPhysics : MonoBehaviour
    {
        private Enemy enemy;
        Rigidbody2D rb;
        [SerializeField] private float skinWidth = 0.02f;
        [SerializeField] BoxCollider2D terrainSensor;

        private float bodyWidth; // 임시
        
        [SerializeField] LayerMask terrainLayer;
        [SerializeField] bool isGrounded;
        
        // 개성에 맞게 수동으로 다룰 것
        public bool gravityEnabled;
        public bool collisionEnabled;
        
        bool hasFixed;
        private Vector2 fixPos;

        [Header("현재 속도")] 
        public Vector2 velocity;
        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            rb = GetComponent<Rigidbody2D>();
            terrainSensor = GetComponentInChildren<BoxCollider2D>();

            terrainLayer = LayerMask.GetMask("Ground");
        }

        void Update()
        {
            enemy.Animator.SetFloat(EnemyAnimationString.VelocityY,rb.linearVelocityY);
            hasFixed = false;
            if (gravityEnabled)
            {
                if (IsGrounded() && rb.linearVelocity.y <= 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                }
                else rb.linearVelocity += new Vector2(0, Physics2D.gravity.y * Time.deltaTime);
            }

            if (collisionEnabled)
            {
                FixPenetratedCollision();
            }
        }

        private void FixPenetratedCollision()
        {
            Vector2 center = (Vector2)transform.position +
                             (Vector2)terrainSensor.gameObject.transform.localPosition +
                             terrainSensor.offset;

            hasFixed = false;
            fixPos = Vector2.zero;
            FixSide(Vector2.down, terrainSensor.size, center);
            FixSide(Vector2.up, terrainSensor.size, center);
            FixSide(Vector2.left, terrainSensor.size, center);
            FixSide(Vector2.right, terrainSensor.size, center);

            if (hasFixed)
            {
                rb.MovePosition(rb.position + fixPos);
                if (fixPos.x != 0) rb.linearVelocityX = 0;
                else if (fixPos.y != 0) rb.linearVelocityY = 0;
                Debug.Log("충돌 픽스 : " + Time.frameCount) ;
            }
        }

        private void FixSide(Vector2 direction, Vector2 terrainSensorSize, Vector2 center)
        {
            Vector2 castSize = terrainSensorSize;
            float distance;
            if (direction.x != 0) {
                castSize.x = skinWidth; // 좌우 검사시 x축 얇게
                distance = terrainSensorSize.x / 2;
            }
            else {
                castSize.y = skinWidth; // 상하 검사시 y축 얇게
                distance = terrainSensorSize.y / 2;
            }
            
            RaycastHit2D hit = Physics2D.BoxCast(center, castSize, 0, direction, distance, terrainLayer);

            
            if (hit.collider)
            {
                float penetration = distance - hit.distance;   

                if (penetration > 0.002f)
                {
                    Vector2 penetrationVec = -direction * penetration;
                    if (!hasFixed)
                    {
                        fixPos = penetrationVec;
                        hasFixed = true;
                    }
                    else if (fixPos.sqrMagnitude > penetrationVec.sqrMagnitude) fixPos = penetrationVec;
                }
            }
        }

        public bool IsGrounded()
        {
            isGrounded = CheckCollision(Vector2.down) && rb.linearVelocityY <= 0;
            return isGrounded;
        }
        
        private bool CheckCollision(Vector2 direction)
        {
            Vector2 center = (Vector2)transform.position +
                             (Vector2)terrainSensor.gameObject.transform.localPosition +
                             terrainSensor.offset;
            
            Vector2 castSize = terrainSensor.size;
            if(direction.x != 0) castSize.y -= skinWidth;
       
            RaycastHit2D hit = Physics2D.BoxCast(center, castSize, 0, direction, skinWidth, terrainLayer);
       
            return hit.collider;
        }
    }
}