using System;
using ToB.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB.Entities
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private float width;
        public float Width => width;
        [SerializeField] private float height;
        public float Height => height;

        [SerializeField] private LayerMask mask;
        [SerializeField] private Color gizmoColor;
        [SerializeField] public bool PlayerEntered;

        public event Action OnPlayerEntered;
        public event Action OnPlayerExit;

        private void Reset()
        {
            mask = LayerMask.GetMask("Player");       
        }

        public Vector2 GetRandomPosition(bool fixedX = false, bool fixedY = false)
        {
            float x = fixedX ? transform.position.x : Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
            float y = fixedY ? transform.position.y : Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
            return new Vector2(x, y);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position, new Vector3(width, height, 0));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((mask & 1 << other.gameObject.layer) != 0)
            {
                PlayerEntered = true;
                OnPlayerEntered?.Invoke();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if ((mask & 1 << other.gameObject.layer) != 0)
            {
                PlayerEntered = false;
                OnPlayerExit?.Invoke();
            }
        }
        
        
    }
}
