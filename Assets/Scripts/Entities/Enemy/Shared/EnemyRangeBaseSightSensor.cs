using System;
using UnityEditor;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyRangeBaseSightSensor : MonoBehaviour
    {
        private Enemy enemy;
        
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask rayMask;

        [SerializeField] private Transform targetInRange;
        public Rigidbody2D TargetRB { get; private set; }

        public Transform TargetInRange => targetInRange;
        
        IEnemySightSensorSO sightSensorSO;
        public float SightRange => sightSensorSO.SightRange;
        public float SightAngle => sightSensorSO.SightAngle;
        
        private void Awake()
        {
            if (!circleCollider) circleCollider = GetComponent<CircleCollider2D>();
           
        }

        private void Start()
        {
            playerMask = LayerMask.GetMask("Player");       
            rayMask = LayerMask.GetMask("Player", "Ground");
        }


        private void Reset()
        {
            circleCollider = GetComponent<CircleCollider2D>();
            playerMask = LayerMask.GetMask("Player");       
            rayMask = LayerMask.GetMask("Player", "Ground");
            
            circleCollider.isTrigger = true;
        }

        public void Init(Enemy enemy)
        {
            this.enemy = enemy;
            sightSensorSO = enemy.EnemySO as IEnemySightSensorSO;

            if (sightSensorSO == null)
            {
                Debug.Log("시야 데이터가 없습니다. : " + enemy.gameObject.name);
                return;
            }

            circleCollider.radius = SightRange;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                targetInRange = other.transform;
                TargetRB = other.GetComponent<Rigidbody2D>();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                targetInRange = null;
                TargetRB = null;
            }
        }

        private void Update()
        {
            enemy.target = null;
            
            if (!targetInRange) return;

            Vector2 posDiff = TargetRB.position - (Vector2)transform.position;
            float distance = posDiff.magnitude;

            if (distance < 1f)
            {
                enemy.target = targetInRange;
                return;
            }
            
            Vector2 rayDirection = posDiff.normalized;
            
            Debug.DrawRay(transform.position, rayDirection * SightRange, Color.red);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, SightRange, rayMask);

            if (!hit)
            {
                return;
            }

            if ((1 << hit.collider.gameObject.layer & playerMask) == 0) return;
            
            // 시야 각도 계산
            float angle = Mathf.Abs(Vector2.SignedAngle(enemy.LookDirectionHorizontal, rayDirection));
            if (angle > SightAngle / 2)
            {
                Debug.Log("각도 안에 없음");
                return;
            }
            
            enemy.target = targetInRange;
        }
    }
}
