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
        [SerializeField] private float sightRange;
        [SerializeField] private float sightAngle;

        [SerializeField] private Transform targetInRange;
        public Rigidbody2D TargetRB { get; private set; }

        public Transform TargetInRange => targetInRange;
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

        public void Init(Enemy enemy, float sightRange, float sightAngle)
        {
            this.enemy = enemy;
            this.sightRange = sightRange;
            this.sightAngle = sightAngle;
            
            circleCollider.radius = sightRange;
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

            Vector2 posDiff = (Vector2)targetInRange.position - TargetRB.position;
            float distance = posDiff.magnitude;

            if (distance < 1f)
            {
                enemy.target = targetInRange;
                return;
            }
            
            Vector2 rayDirection = posDiff.normalized;
            
            Debug.DrawRay(transform.position, rayDirection * sightRange, Color.red);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, sightRange, rayMask);

            if (!hit)
            {
                EditorApplication.isPaused = true;
                return;
            }

            if ((1 << hit.collider.gameObject.layer & playerMask) == 0) return;
            
            // 시야 각도 계산
            float angle = Mathf.Abs(Vector2.SignedAngle(enemy.LookDirectionHorizontal, rayDirection));
            if (angle > sightAngle / 2)
            {
                Debug.Log("각도 안에 없음");
                return;
            }
            
            enemy.target = targetInRange;
        }
    }
}
