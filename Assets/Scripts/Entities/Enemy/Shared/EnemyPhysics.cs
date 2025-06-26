using System;
using System.Collections.Generic;
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
        private Vector2 fixDirection;
        private Vector2 totalVelocity;

        [Header("현재 속도")] 
        public Vector2 velocity;
        
        public Dictionary<string, Vector2> externalVelocity = new Dictionary<string, Vector2>();
        
        public bool HasCollided { get; private set; }

        public float velocityX
        {
            get => velocity.x;
            set => velocity = new Vector2(value, velocity.y);
        }
        
        public float velocityY
        {
            get => velocity.y;
            set => velocity = new Vector2(velocity.x, value);
        }
        
        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            rb = GetComponent<Rigidbody2D>();

            terrainLayer = LayerMask.GetMask("Ground");
        }

        void Update()
        {
            enemy.Animator.SetFloat(EnemyAnimationString.VelocityY,velocityY);
            
            if (gravityEnabled)
            {
                if (IsGrounded())
                {
                    velocity = new Vector2(velocityX, 0);
                }
                else velocity += new Vector2(0, Physics2D.gravity.y * Time.deltaTime);
                
            }
           
        }

        void FixedUpdate()
        {
            if (collisionEnabled)
            {
                FixPenetratedCollision();
            }
            MoveToNextPosition();
        }

        private void MoveToNextPosition()
        {
            HasCollided = false;
            totalVelocity = velocity;

            foreach (var element in externalVelocity)
            {
                totalVelocity += element.Value;
            }
            
            // 속도가 거의 0이면 이동하지 않음
            if (velocity.sqrMagnitude < 0.001f)
            {
                return;
            }

            // 벽에 박히지 않기 위한 다음 위치 계산 박스 캐스트 시작
            if(collisionEnabled)
            {
                PerformBoxCastMovement();
            }
            else rb.position += velocity * Time.fixedDeltaTime; 
        }

        private void PerformBoxCastMovement()
        {
            Vector2 castBoxSize = terrainSensor.size;
                
            Vector2 moveDelta = totalVelocity * Time.fixedDeltaTime;
            RaycastHit2D hit = Physics2D.BoxCast(rb.position + terrainSensor.offset, castBoxSize, 0, totalVelocity.normalized, moveDelta.magnitude, terrainLayer);

            if (hit.collider)
            {
                HasCollided = true;
                Vector2 resultMoveDelta = hit.distance * moveDelta.normalized;

                if (hit.normal.y < 0.5f) // 벽에 닿은 경우
                {
                    resultMoveDelta.y = moveDelta.y;
                    int leftRightNum = totalVelocity.x > 0 ? 1 : -1;
                    resultMoveDelta.x -= leftRightNum * skinWidth;
                    totalVelocity.x = 0;
                }
                else // 땅에 닿은 경우
                {
                    resultMoveDelta.x = moveDelta.x;
                    int upDownNum = totalVelocity.y > 0 ? 1 : -1;
                    resultMoveDelta.y -= upDownNum * skinWidth;
                    totalVelocity.y = 0;
                }
                    
                rb.MovePosition(rb.position + resultMoveDelta);
            }
            else
            {
                rb.MovePosition(rb.position + totalVelocity * Time.fixedDeltaTime); // MovePosition 함수 테스트
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
                rb.position += fixPos - fixDirection * skinWidth; 
                // Debug.Log("충돌 픽스 : " + Time.frameCount + velocity) ;
                // if (fixPos.x != 0) velocityX = 0;
                // else if (fixPos.y != 0) velocityY = 0;
            }
        }

        private void FixSide(Vector2 direction, Vector2 terrainSensorSize, Vector2 center)
        {
            Vector2 castSize = terrainSensorSize;
            float distance;
            if (direction.x != 0) {
                castSize.x = skinWidth; // 좌우 검사시 x축 얇게
                distance = terrainSensorSize.x / 2 + skinWidth/2;
            }
            else {
                castSize.y = skinWidth; // 상하 검사시 y축 얇게
                distance = terrainSensorSize.y / 2+ skinWidth/2;
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
                        fixDirection = direction;
                    }
                    else if (fixPos.sqrMagnitude > penetrationVec.sqrMagnitude)
                    {
                        fixPos = penetrationVec;
                        fixDirection = direction;
                        
                    }
                }
            }
        }

        public bool IsGrounded()
        {
            isGrounded = CheckCollision(Vector2.down) && velocityY <= 0;
            return isGrounded;
        }


        private bool CheckCollision(Vector2 direction)
        {
            Vector2 center = (Vector2)transform.position +
                             (Vector2)terrainSensor.gameObject.transform.localPosition +
                             terrainSensor.offset;
            
            Vector2 castSize = terrainSensor.size;
            if(direction.x != 0) castSize.y -= skinWidth;
       
            RaycastHit2D hit = Physics2D.BoxCast(center, castSize, 0, direction, skinWidth*2f, terrainLayer);
       
            return hit.collider;
        }
    }
}