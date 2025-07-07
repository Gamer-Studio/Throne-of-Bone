using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyPhysics : MonoBehaviour
    {
        private Enemy enemy;
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
        public readonly Dictionary<string, Vector2> externalVelocity = new Dictionary<string, Vector2>();
        
        public bool HasCollided { get; private set; }

        public bool IsWallLeft;
        public bool IsWallRight;

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
            ClearCollideResults();
            if (collisionEnabled)
            {
                FixPenetratedCollision();
            }
            MoveToNextPosition();
        }

        private void ClearCollideResults()
        {
            HasCollided = false;
            IsWallLeft = false;
            IsWallRight = false;
        }

        private void MoveToNextPosition()
        {
            totalVelocity = velocity;

            foreach (var element in externalVelocity)
            {
                totalVelocity += element.Value;
            }
            
            // 속도가 거의 0이면 이동하지 않음
            if (totalVelocity.sqrMagnitude < 0.001f)
            {
                return;
            }

            // 벽에 박히지 않기 위한 다음 위치 계산 박스 캐스트 시작
            if(collisionEnabled)
            {
                PerformBoxCastMovement();
            }
            else enemy.transform.position += (Vector3)totalVelocity * Time.fixedDeltaTime; 
        }

        private void PerformBoxCastMovement()
        {
            Vector2 castBoxSize = terrainSensor.size;
                
            Vector2 moveDelta = totalVelocity * Time.fixedDeltaTime;
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)enemy.transform.position + terrainSensor.offset, castBoxSize, 0, totalVelocity.normalized, moveDelta.magnitude, terrainLayer);

            if (hit.collider)
            {
                HasCollided = true;
                Vector2 resultMoveDelta = hit.distance * moveDelta.normalized;

                if (hit.normal.y < 0.5f) // 벽에 닿은 경우
                {
                    if(totalVelocity.x > 0) IsWallRight = true;
                    else IsWallLeft = true;
                    
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
                    
                enemy.transform.position += (Vector3)resultMoveDelta;
            }
            else
            {
                enemy.transform.position += (Vector3)totalVelocity * Time.fixedDeltaTime; // MovePosition 함수 테스트
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
                enemy.transform.position += (Vector3)(fixPos - fixDirection * skinWidth); 
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
            Vector2 origin = (Vector2)terrainSensor.gameObject.transform.position +
                             terrainSensor.offset;
            
            Vector2 castSize = terrainSensor.size;
            
            castSize.x -= skinWidth;
            castSize.y -= skinWidth;
       
            RaycastHit2D hit = Physics2D.BoxCast(origin, castSize, 0, direction, skinWidth*2f, terrainLayer);
            
            if(direction == Vector2.down)  // 디버그 시각화
                DrawBoxCast(origin, castSize, direction.normalized, skinWidth*2f, Color.Lerp(Color.red, Color.yellow, 0.5f));

       
            return hit.collider;
        }
        
        private void DrawBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance, Color color)
        {
            // 중심점에서 방향으로 이동한 target 위치
            Vector2 castCenter = origin + direction * distance;

            // 네 개의 꼭짓점 계산 (회전 없음)
            Vector2 halfSize = size * 0.5f;

            Vector2[] cornersStart = new Vector2[4]
            {
                origin + new Vector2(-halfSize.x, -halfSize.y),
                origin + new Vector2(-halfSize.x,  halfSize.y),
                origin + new Vector2( halfSize.x,  halfSize.y),
                origin + new Vector2( halfSize.x, -halfSize.y)
            };

            Vector2[] cornersEnd = new Vector2[4]
            {
                castCenter + new Vector2(-halfSize.x, -halfSize.y),
                castCenter + new Vector2(-halfSize.x,  halfSize.y),
                castCenter + new Vector2( halfSize.x,  halfSize.y),
                castCenter + new Vector2( halfSize.x, -halfSize.y)
            };

            // 사각형 테두리 그리기
            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(cornersStart[i], cornersStart[(i + 1) % 4], color);
                Debug.DrawLine(cornersEnd[i], cornersEnd[(i + 1) % 4], color);
                Debug.DrawLine(cornersStart[i], cornersEnd[i], color);
            }
        }

        public bool IsLedgeBelow()
        {
            Vector2 origin = (Vector2)transform.position +
                             new Vector2(0, skinWidth);

            float rayDistance = 0.1f;
            
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, terrainLayer);

            return !hit.collider;
        }
        
        public bool IsLedgeOnSide(Vector2 direction)
        {
            Vector2 origin = (Vector2)transform.position +
                             direction * terrainSensor.size.x / 2f +
                             new Vector2(0, skinWidth);

            float rayDistance = 0.1f;
            
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, terrainLayer);
            return !hit.collider;
        }
    }
}