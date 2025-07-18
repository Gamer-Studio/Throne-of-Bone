using System;
using ToB.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB.Entities
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleLocation : MonoBehaviour
    {
        [SerializeField] CircleCollider2D circleCollider;
        [SerializeField] private float radius;

        [SerializeField] private LayerMask mask;
        [SerializeField] private Color gizmoColor;
        [SerializeField] public bool PlayerEntered;

        public event Action<GameObject> OnPlayerEntered;

        private void Reset()
        {
            circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.radius = radius;
            mask = LayerMask.GetMask("Player");       
        }

        public void Init(float radius)
        {
            this.radius = radius;
            circleCollider.radius = radius;
        }

        private void OnValidate()
        {
            circleCollider.radius = radius;
        }

        public Vector2 GetRandomPosition(bool fixedX = false, bool fixedY = false)
        {
            float randomRadius = Mathf.Sqrt(Random.value) * radius;
            float randomAngle = Random.Range(0, 2 * Mathf.PI);
            
            float x = fixedX ? transform.position.x : randomRadius * Mathf.Cos(randomAngle) + transform.position.x;
            float y = fixedY ? transform.position.y : randomRadius * Mathf.Sin(randomAngle) + transform.position.y;
            
            return new Vector2(x, y);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, radius);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((mask & 1 << other.gameObject.layer) != 0)
            {
                PlayerEntered = true;
                OnPlayerEntered?.Invoke(other.gameObject);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if ((mask & 1 << other.gameObject.layer) != 0)
            {
                PlayerEntered = false;
            }
        }
        
        
    }
}
